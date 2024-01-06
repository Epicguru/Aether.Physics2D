/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using nkast.Aether.Physics2D.Common;

namespace nkast.Aether.Physics2D.Content;

public struct Polygon
{
    public Vertices Vertices;
    public bool Closed;

    public Polygon(Vertices v, bool closed)
    {
        Vertices = v;
        Closed = closed;
    }
}