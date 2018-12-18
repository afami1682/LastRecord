using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 会話イベント
/// </summary>
public class ConversationEvent : BattleFunc
{
    string test;

    public ConversationEvent(string _test)
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
