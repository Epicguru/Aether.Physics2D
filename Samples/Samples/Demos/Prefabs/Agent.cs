﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Samples.DrawingSystem;
using nkast.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nkast.Aether.Physics2D.Samples.Demos.Prefabs
{
    public class Agent
    {
        private Body _agentBody;
        private Sprite _box;
        private Category _collidesWith;
        private Category _collisionCategories;
        private Sprite _knob;
        private float _offset;
        private SpriteBatch _batch;

        public Agent(World world, ScreenManager screenManager, Vector2 position)
        {
            _batch = screenManager.SpriteBatch;

            _collidesWith = Category.All;
            _collisionCategories = Category.All;

            _agentBody = world.CreateBody(position);
            _agentBody.BodyType = BodyType.Dynamic;

            //Center
            _agentBody.CreateCircle(0.5f, 0.5f);

            //Left arm
            _agentBody.CreateRectangle(1.5f, 0.4f, 1f, new Vector2(-1f, 0f));
            _agentBody.CreateCircle(0.5f, 0.5f, new Vector2(-2f, 0f));

            //Right arm
            _agentBody.CreateRectangle(1.5f, 0.4f, 1f, new Vector2(1f, 0f));
            _agentBody.CreateCircle(0.5f, 0.5f, new Vector2(2f, 0f));

            //Top arm
            _agentBody.CreateRectangle(0.4f, 1.5f, 1f, new Vector2(0f, 1f));
            _agentBody.CreateCircle(0.5f, 0.5f, new Vector2(0f, 2f));

            //Bottom arm
            _agentBody.CreateRectangle(0.4f, 1.5f, 1f, new Vector2(0f, -1f));
            _agentBody.CreateCircle(0.5f, 0.5f, new Vector2(0f, -2f));

            //GFX
            _box = new Sprite(screenManager.Assets.TextureFromVertices(PolygonTools.CreateRectangle(2.5f / 2f, 0.4f / 2f), MaterialType.Blank, Color.White, 1f, 24f));
            _knob = new Sprite(screenManager.Assets.CircleTexture(0.5f, MaterialType.Blank, Color.Orange, 1f, 24f));
            _offset = (2f);
        }

        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                _collisionCategories = value;
                foreach (Fixture fixture in Body.FixtureList)
                    fixture.CollisionCategories = value;
            }
        }

        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                _collidesWith = value;
                foreach (Fixture fixture in Body.FixtureList)
                    fixture.CollidesWith = value;
            }
        }

        public Body Body
        {
            get { return _agentBody; }
        }

        public void Draw()
        {
            //cross
            _batch.Draw(_box.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _box.Origin, new Vector2(4f, 0.4f) * _box.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_box.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation + MathHelper.Pi / 2f, _box.Origin, new Vector2(4f, 0.4f) * _box.TexelSize, SpriteEffects.FlipVertically, 0f);

            //knobs
            _batch.Draw(_knob.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _knob.Origin, new Vector2(2f * 0.5f) * _knob.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_knob.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _knob.Origin + new Vector2(0f, _offset) * _knob.Size / new Vector2(2f * 0.5f), new Vector2(2f * 0.5f) * _knob.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_knob.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _knob.Origin - new Vector2(0f, _offset) * _knob.Size / new Vector2(2f * 0.5f), new Vector2(2f * 0.5f) * _knob.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_knob.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _knob.Origin + new Vector2(_offset, 0f) * _knob.Size / new Vector2(2f * 0.5f), new Vector2(2f * 0.5f) * _knob.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_knob.Texture, _agentBody.Position, null, Color.White, _agentBody.Rotation, _knob.Origin - new Vector2(_offset, 0f) * _knob.Size / new Vector2(2f * 0.5f), new Vector2(2f * 0.5f) * _knob.TexelSize, SpriteEffects.FlipVertically, 0f);
        }
    }
}