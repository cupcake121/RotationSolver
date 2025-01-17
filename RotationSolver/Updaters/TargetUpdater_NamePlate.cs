﻿using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Commands;
using RotationSolver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    public static uint[] TreasureCharas { get; private set; } = new uint[0];
    private static void UpdateNamePlate(IEnumerable<BattleChara> allTargets)
    {
        List<uint> charas = new List<uint>(5);
        //60687 - 60691 For treasure hunt.
        for (int i = 60687; i <= 60691; i++)
        {
            var b = allTargets.FirstOrDefault(obj => obj.GetNamePlateIcon() == i);
            if (b == null || b.CurrentHp == 0) continue;
            charas.Add(b.ObjectId);
        }
        TreasureCharas = charas.ToArray();
    }
}
