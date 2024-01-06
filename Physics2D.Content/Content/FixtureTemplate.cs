/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using nkast.Aether.Physics2D.Collision.Shapes;

namespace nkast.Aether.Physics2D.Content;

public class FixtureTemplate
{
    public Shape Shape;
    public float Restitution;
    public float Friction;
    public string Name;
}