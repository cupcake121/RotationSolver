using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Data.Parsing.Layer;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using static XIVAutoAttack.Combos.Melee.SAMCombo;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class SAMCombo : JobGaugeCombo<SAMGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.fatinghenji };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint[] JobIDs => new uint[] { 34 };

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    private static bool HaveMoon => Player.HaveStatus(ObjectStatus.Moon);
    private static bool HaveFlower => Player.HaveStatus(ObjectStatus.Flower);


    public static readonly BaseAction
        //刃风
        Hakaze = new(7477),

        //阵风
        Jinpu = new(7478),

        //心眼
        ThirdEye = new(7498),

        //燕飞
        Enpi = new(7486),

        //士风
        Shifu = new(7479),

        //风雅
        Fuga = new(7483),

        //月光
        Gekko = new(7481),

        //彼岸花
        Higanbana = new(7489, isDot: true)
        {
            OtherCheck = b => !IsMoving && SenCount == 1 && HaveMoon && HaveFlower,
            TargetStatus = new[] { ObjectStatus.Higanbana },
        },

        //天下五剑
        TenkaGoken = new(7488)
        {
            OtherCheck = b => !IsMoving,
        },

        //纷乱雪月花
        MidareSetsugekka = new(7487)
        {
            OtherCheck = b => !IsMoving && SenCount == 3,
        },

        //满月
        Mangetsu = new(7484),

        //花车
        Kasha = new(7482),

        //樱花
        Oka = new(7485),

        //明镜止水
        MeikyoShisui = new(7499)
        {
            BuffsProvide = new[] { ObjectStatus.MeikyoShisui },
            OtherCheck = b => JobGauge.HasSetsu && !JobGauge.HasKa && !JobGauge.HasGetsu,
        },

        //雪风
        Yukikaze = new(7480),

        //必杀剑·晓天
        HissatsuGyoten = new(7492),

        //必杀剑·震天
        HissatsuShinten = new(7490),

        //必杀剑·九天
        HissatsuKyuten = new(7491),

        //意气冲天
        Ikishoten = new(16482),

        //必杀剑·红莲
        HissatsuGuren = new(7496),

        //必杀剑·闪影
        HissatsuSenei = new(16481),

        //回返五剑
        KaeshiGoken = new(16485),

        //回返雪月花
        KaeshiSetsugekka = new(16486),

        //照破
        Shoha = new(16487),

        //无明照破
        Shoha2 = new(25779),

        //奥义斩浪
        OgiNamikiri = new(25781)
        {
            OtherCheck = b => HaveFlower && HaveMoon,
            BuffsNeed = new[] { ObjectStatus.OgiNamikiriReady },
        },

        //回返斩浪
        KaeshiNamikiri = new(25782);

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体防御, $"{ThirdEye}"},
        {DescType.移动技能, $"{HissatsuGyoten}"},
    };
    private protected override bool GeneralGCD(out IAction act)
    {
        bool haveMeikyoShisui = Player.HaveStatus(ObjectStatus.MeikyoShisui);

        //赶紧回返！
        if (Service.IconReplacer.OriginalHook(OgiNamikiri.ID) == KaeshiNamikiri.ID)
        {
            if (KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiGoken.ID)
        {
            if (KaeshiGoken.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiSetsugekka.ID)
        {
            if (KaeshiSetsugekka.ShouldUse(out act, mustUse: true)) return true;
        }

        if (!haveMeikyoShisui && OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        if (TenkaGoken.ShouldUse(out act))
        {
            if (SenCount == 2) return true;
            if (MidareSetsugekka.ShouldUse(out act)) return true;
        }
        else
        {
            if (MidareSetsugekka.ShouldUse(out act)) return true;
            //更改了彼岸花逻辑，应当在拥有雪闪的时候打出
            if (Higanbana.ShouldUse(out act) && JobGauge.HasSetsu) return true;
        }

        //123
        //如果是单体，且明镜止水的冷却时间小于3秒。
        if (!JobGauge.HasSetsu && !Fuga.ShouldUse(out _))
        {
            if (Yukikaze.ShouldUse(out act)) return true;
        }
        if (!HaveMoon)//判断风月buff，该buff提供10%伤害加成
        {
            if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!HaveFlower)//判断风花buff，该buff提供10%技速加成
        {
            if (KaGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasGetsu) //月闪
        {
            if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasKa) //花闪
        {
            if (KaGCD(out act, haveMeikyoShisui)) return true;
        }

        //来个月？
        if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        if (Yukikaze.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;

        if (Fuga.ShouldUse(out act)) return true;
        if (Hakaze.ShouldUse(out act)) return true;



        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Enpi.ShouldUse(out act)) return true;

        return false;
    }

    //处理樱花连击
    private bool KaGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Oka.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Kasha.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Shifu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    //处理月光连击
    private bool GetsuGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Mangetsu.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Gekko.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Jinpu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && HissatsuGyoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.MeditationStacks == 3)
        {
            if (Shoha2.ShouldUse(out act)) return true;
            if (Shoha.ShouldUse(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (HissatsuGuren.ShouldUse(out act)) return true;
            if (HissatsuKyuten.ShouldUse(out act)) return true;

            if (HissatsuSenei.ShouldUse(out act)) return true;
            if (HissatsuShinten.ShouldUse(out act)) return true;
        }

        if (InCombat && Ikishoten.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveHostileInRange &&
            !nextGCD.IsAnySameAction(false, Higanbana, OgiNamikiri, KaeshiNamikiri) &&
            MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (ThirdEye.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}