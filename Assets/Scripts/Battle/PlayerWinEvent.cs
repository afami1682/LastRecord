﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (プレイヤーの)Unit勝ちイベント
/// </summary>
public class PlayerWinEvent : BattleFunc
{
    string test;

    public PlayerWinEvent(string _test)
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