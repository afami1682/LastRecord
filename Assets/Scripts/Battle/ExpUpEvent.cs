﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Expの取得イベント
/// </summary>
public class ExpUpEvent : BattleFunc
{
    string test;

    public ExpUpEvent(string _test)
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