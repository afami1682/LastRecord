using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回避イベント
/// </summary>
public class AvoidanceEvent : BattleFunc
{
    string test;

    public AvoidanceEvent(string _test)
    {
        test = _test;
    }
    protected override void Start()
    {
    }

    protected override bool Update()
    {
        return true;
    }
}
