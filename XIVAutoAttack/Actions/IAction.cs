﻿namespace XIVAutoAttack.Actions
{
    public interface IAction
    {
       bool Use();
       uint ID { get; }
       uint AdjustedID { get; }
    }
}
