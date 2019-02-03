using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// 敵UnitのAI
/// </summary>
public class EnemyAIManager
{
    readonly int fieldWidth;
    readonly int fieldHeight;
    AI.CommonCalc commonCalc;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public EnemyAIManager(Struct.Field field)
    {
        this.fieldWidth = field.width;
        this.fieldHeight = field.height;
        this.commonCalc = new AI.CommonCalc(field);
    }
}