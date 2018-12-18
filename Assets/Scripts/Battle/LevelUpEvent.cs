using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// レベルアップイベント
/// </summary>
public class LevelUpEvent : BattleFunc
{
    string test;

    public LevelUpEvent(string _test)
    {
        test = _test;
    }
    protected override bool Start()
    {
        return true;
    }

    protected override bool Update()
    {
        return true;
    }
}
