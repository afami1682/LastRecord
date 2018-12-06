using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象関数
/// </summary>
public abstract class BattleFunc
{
    protected abstract bool Event();
    public bool Run() { return Event(); }
}
