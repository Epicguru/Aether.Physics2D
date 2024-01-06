/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using nkast.Aether.Physics2D.Common.PhysicsLogic;
using nkast.Aether.Physics2D.Dynamics;

namespace nkast.Aether.Physics2D.Controllers;

public abstract class Controller : FilterData
{
    public World World { get; internal set; }
    public ControllerCategory ControllerCategory = ControllerCategory.Cat01;

    public bool Enabled = true;

    public override bool IsActiveOn(Body body)
    {
        if (body.ControllerFilter.IsControllerIgnored(ControllerCategory))
            return false;

        return base.IsActiveOn(body);
    }

    public abstract void Update(float dt);
}