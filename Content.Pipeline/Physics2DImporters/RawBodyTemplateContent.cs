﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace nkast.Aether.Content.Pipeline
{
    struct RawBodyTemplateContent
    {
        public List<RawFixtureTemplateContent> Fixtures;
        public string Name;
        public float Mass;
        public BodyType BodyType;
    }

}
