/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Samples.MediaSystem;
using nkast.Aether.Physics2D.Samples.ScreenSystem;

namespace nkast.Aether.Physics2D.Samples
{
    /// <summary>
    /// This is the main type for the samples
    /// </summary>
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private LineBatch _lineBatch;
        private BasicEffect _batchEffect;
        private QuadRenderer _quadRenderer;
        private InputHelper _input;
        private List<GameScreen> _screens = new List<GameScreen>();
        private List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private List<RenderTarget2D> _transitions = new List<RenderTarget2D>();
        private MenuScreen _menuScreen;
        private GraphicsDeviceManager _graphics;

        private bool _isExiting;

        private FrameRateCounter _counter;
        private bool _showFPS;

        public Game1()
        {
            Window.Title = "Samples";

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.Reach;
            _graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            IsFixedTimeStep = true;
            
            _graphics.IsFullScreen = false;

        }

        void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // unlock the 30 fps limit. 60fps (if possible)
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;

            // set HiDef Profile if supported
            if (e.GraphicsDeviceInformation.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            _input = new InputHelper();

            _counter = new FrameRateCounter();
            _showFPS = false;

            _isExiting = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ContentWrapper.Initialize(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _lineBatch = new LineBatch(GraphicsDevice);
            _batchEffect = new BasicEffect(GraphicsDevice);
            _batchEffect.VertexColorEnabled = true;
            _batchEffect.TextureEnabled = true;
            _quadRenderer = new QuadRenderer(GraphicsDevice);

            _input.LoadContent(GraphicsDevice.Viewport);

            _counter.LoadContent();


            // Create rendertarget for transitions
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            _transitions.Add(new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents));

            _menuScreen = new MenuScreen();

            List<Type> DemosToLoad = new List<Type>();
            Assembly samplesFramework = Assembly.GetExecutingAssembly();
            foreach (Type sampleType in samplesFramework.GetTypes())
            {
                if (sampleType.IsSubclassOf(typeof(PhysicsDemoScreen)))
                {
                    DemosToLoad.Add(sampleType);
                }
            }
            DemosToLoad.Add(DemosToLoad[0]); // HACK: Load the first sample two times, since some delayed creation stuff with the rendertargets always breaks the first preview picture.
            Boolean firstPreview = true;
            foreach (Type sampleType in DemosToLoad)
            {
                PhysicsDemoScreen demoScreen = samplesFramework.CreateInstance(sampleType.ToString()) as PhysicsDemoScreen;
#if WINDOWS
                if (!firstPreview) { Console.WriteLine("Loading demo: " + demoScreen.GetTitle()); }
#endif
                RenderTarget2D preview = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 2, pp.BackBufferHeight / 2, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

                demoScreen.Framework = this;
                demoScreen.IsExiting = false;

                demoScreen.SpriteBatch = _spriteBatch;
                demoScreen.LineBatch = _lineBatch;
                demoScreen.BatchEffect = _batchEffect;
                demoScreen.Quads = _quadRenderer;

                demoScreen.LoadContent();

                // "Abuse" transition rendertarget to render screen preview
                GraphicsDevice.SetRenderTarget(_transitions[0]);
                GraphicsDevice.Clear(Color.Transparent);

                _quadRenderer.Begin();
                _quadRenderer.Render(Vector2.Zero, new Vector2(_transitions[0].Width, _transitions[0].Height), null, true, ContentWrapper.Grey, Color.White * 0.3f);
                _quadRenderer.End();

                // Update ensures that the screen is fully visible, we "cover" it so that no physics are run
                demoScreen.Update(new GameTime(demoScreen.TransitionOnTime, demoScreen.TransitionOnTime), true, false);
                demoScreen.Draw(new GameTime());
                demoScreen.Draw(new GameTime());

                GraphicsDevice.SetRenderTarget(preview);
                GraphicsDevice.Clear(Color.Transparent);

                _spriteBatch.Begin();
                _spriteBatch.Draw(_transitions[0], preview.Bounds, Color.White);
                _spriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);

                demoScreen.ExitScreen();
                demoScreen.Update(new GameTime(demoScreen.TransitionOffTime, demoScreen.TransitionOffTime), true, false);
                if (!firstPreview)
                {
                    _menuScreen.AddMenuItem(demoScreen, preview);
                }
                else
                {
                    firstPreview = false;
                }
            }

            AddScreen(new BackgroundScreen());
            AddScreen(_menuScreen);
            AddScreen(new LogoScreen(new TimeSpan(0, 0, 0, 3)));

            ResetElapsedTime();
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
            base.UnloadContent();
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            _input.Update(gameTime);
            // Update the framerate counter

            _counter.Update(gameTime);

            if ((_input.IsNewButtonPress(Buttons.Y) || _input.IsNewKeyPress(Keys.F5)) && !(_screens[_screens.Count - 1] is OptionsScreen))
                AddScreen(new OptionsScreen());

            if (_input.IsNewKeyPress(Keys.F11))
                _showFPS = !_showFPS;

            if (_input.IsNewKeyPress(Keys.F12))
                _graphics.ToggleFullScreen();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();
            _screensToUpdate.AddRange(_screens);

            bool otherScreenHasFocus = !IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus && !_isExiting)
                    {
                        _input.ShowCursor = screen.HasCursor;
                        screen.HandleInput(_input, gameTime);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            if (_isExiting && _screens.Count == 0)
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.TransitionOff)
                {
                    transitionCount++;
                    if (_transitions.Count < transitionCount)
                    {
                        PresentationParameters pp = GraphicsDevice.PresentationParameters;
                        _transitions.Add(new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents));
                    }
                    GraphicsDevice.SetRenderTarget(_transitions[transitionCount - 1]);
                    GraphicsDevice.Clear(Color.Transparent);
                    screen.Draw(gameTime);
                    GraphicsDevice.SetRenderTarget(null);
                }
            }

            GraphicsDevice.Clear(Color.Black);

            transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                {
                    continue;
                }

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.TransitionOff)
                {
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    if (screen is PhysicsDemoScreen)
                    {
                        Vector2 position = Vector2.Lerp(_menuScreen.PreviewPosition, new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2f, 1f - screen.TransitionPosition);
                        _spriteBatch.Draw(_transitions[transitionCount], position, null, Color.White * Math.Min(screen.TransitionAlpha / 0.2f, 1f), 0f,
                                          new Vector2(_transitions[transitionCount].Width, _transitions[transitionCount].Height) / 2f, 0.5f + 0.5f * (1f - screen.TransitionPosition), SpriteEffects.None, 0f);
                    }
                    else
                    {
                        _spriteBatch.Draw(_transitions[transitionCount], Vector2.Zero, Color.White * screen.TransitionAlpha);
                    }
                    _spriteBatch.End();

                    transitionCount++;
                }
                else
                {
                    screen.Draw(gameTime);
                }
            }

            _input.Draw(_spriteBatch);

            if (_showFPS)
            {
                _counter.Draw(_spriteBatch);
            }

            base.Draw(gameTime);
        }

        public void ExitGame()
        {
            foreach (GameScreen screen in _screens)
            {
                screen.ExitScreen();
            }
            _isExiting = true;
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.Framework = this;
            screen.IsExiting = false;

            screen.SpriteBatch = _spriteBatch;
            screen.LineBatch = _lineBatch;
            screen.BatchEffect = _batchEffect;
            screen.Quads = _quadRenderer;

            // Tell the screen to load content.
            screen.LoadContent();
            // Loading my take a while so elapsed time is reset to prevent hick-ups
            ResetElapsedTime();
            _screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // Tell the screen to unload content.
            screen.UnloadContent();
            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
        }
    }
}