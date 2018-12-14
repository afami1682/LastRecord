using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象関数
/// </summary>
public abstract class BattleFunc
{
    bool isSetup = false;

    protected abstract void Start();
    protected abstract bool Update();

    public void Setup() { Start(); }
    public bool Run()
    {
        if (!isSetup)
        {
            Start();
            isSetup = true;
        }
        return Update();
    }
}
