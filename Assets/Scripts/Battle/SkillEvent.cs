﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキル発動イベント
/// </summary>
public class SkillEvent : BattleFunc{
    
    string test;

    public SkillEvent(string _test)
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