﻿using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Updaters;

namespace RotationSolver.Rotations.RangedMagicial.BLU;

internal class BLU_Simplify : BLU_Base
{
    public override string GameVersion => "6.3";

    public override string RotationName => "Simplify";

    public override string Description => "This is a simplfied version for me (ArchiTed) using, \nwhich doesn't contain all actions.";

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (ChocoMeteor.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act)) return true;


        if (TripleTrident.OnSlot && TripleTrident.RightType && TripleTrident.WillHaveOneChargeGCD(OnSlotCount(Whistle, Tingle), 0))
        {
            if ((TripleTrident.CanUse(out _, mustUse: true) || !HasHostilesInRange) &&Whistle.CanUse(out act)) return true;

            if (!Player.HasStatus(true, StatusID.Tingling)
                && Tingle.CanUse(out act, mustUse: true)) return true;
            if (Offguard.CanUse(out act)) return true;

            if (TripleTrident.CanUse(out act, mustUse: true)) return true;
        }
        if (ChocoMeteor.CanUse(out act, mustUse: TargetUpdater.HaveCompanion)) return true;

        if (SonicBoom.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act, mustUse: true)) return true;

        return false;
    }
}
