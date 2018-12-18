using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象関数
/// </summary>
public abstract class BattleFunc
{
    bool isSetup = false;

    protected abstract bool Start();
    protected abstract bool Update();

    public bool Setup() { return Start(); }
    public bool Run()
    {
        if (!isSetup)
            return isSetup = Setup(); // セットアップ完了

        return Update();
    }
}
