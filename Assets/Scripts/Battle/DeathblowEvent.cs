﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 必殺イベント
/// </summary>
public class DeathblowEvent : BattleFunc
{
    string test;

    public DeathblowEvent(string _test)
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