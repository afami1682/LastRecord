using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (プレイヤーの)Unit負けイベント
/// </summary>
public class PlayerLoseEvent : BattleFunc
{
    string test;

    public PlayerLoseEvent(string _test)
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
