using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleManager : MonoBehaviour
{
    // バトル関連
    private bool isStart = true; // バトル処理の開始フラグ
    private bool eventExecuting = false; // イベントが実行中かどうか

    private List<MonoBehaviour> eventFuncs = new List<MonoBehaviour>();
    private Action callBackEvent;

    public CutInAnimController cutInAnimController;

    [HideInInspector]
    public bool myAttack;
    [HideInInspector]
    public GameObject myUnitObj, targetUnitObj; // Unitのオブジェクト
    [HideInInspector]
    public int myAttackPower, targetAttackPower; // 攻撃力
    [HideInInspector]
    public int myCriticalRate, targetCriticalRate; // 必殺の発生率
    [HideInInspector]
    public int myAttackCount, targetAttackCount;// 攻撃回数
    [HideInInspector]
    public int myHitRate, targetHitRate;// 命中率
    [HideInInspector]
    public Enums.BATTLE myAttackState, targetAttackState; // 攻撃成功判定
    [HideInInspector]
    public Text myHPText, targetHPText; // 表示用

    void Update()
    {
        // イベントを一つずつ実行していく
        if (isStart &&
        !eventExecuting &&
        0 < eventFuncs.Count)
        {
            // 次のイベントを実行する
            eventFuncs[0].enabled = true;
            eventExecuting = true;
        }
    }

    /// <summary>
    /// 攻撃パラメータの設定
    /// </summary>
    /// <param name="myUnitObj">My unit object.</param>
    /// <param name="targetUnitObj">Target unit object.</param>
    public void SetParam(GameObject myUnitObj, GameObject targetUnitObj, Text myHPText, Text targetHPText)
    {
        // Unit情報の取得
        this.myUnitObj = myUnitObj;
        UnitInfo myUnitInfo = myUnitObj.GetComponent<UnitInfo>();
        this.myHPText = myHPText;

        this.targetUnitObj = targetUnitObj;
        UnitInfo targetUnitInfo = targetUnitObj.GetComponent<UnitInfo>();
        this.targetHPText = targetHPText;

        // 自分
        myAttackPower = GameManager.GetCommonCalc().GetAttackDamage(myUnitInfo, targetUnitInfo);
        myHitRate = GameManager.GetCommonCalc().GetHitRate(myUnitInfo, targetUnitInfo);
        myCriticalRate = GameManager.GetCommonCalc().GetCriticalRete(myUnitInfo, targetUnitInfo);
        myAttackCount = GameManager.GetCommonCalc().GetAttackCount(myUnitInfo, targetUnitInfo);

        // 敵の反撃値
        if (GameManager.GetCommonCalc().GetCellDistance(
                            targetUnitObj.transform.position,
                            myUnitObj.transform.position) <= targetUnitInfo.attackRange)
        {
            // 反撃可能
            targetAttackPower = GameManager.GetCommonCalc().GetAttackDamage(targetUnitInfo, myUnitInfo);
            targetHitRate = GameManager.GetCommonCalc().GetHitRate(targetUnitInfo, myUnitInfo);
            targetCriticalRate = GameManager.GetCommonCalc().GetCriticalRete(targetUnitInfo, myUnitInfo);
            targetAttackCount = GameManager.GetCommonCalc().GetAttackCount(targetUnitInfo, myUnitInfo);
        }
        else
        {
            // 反撃不可
            targetAttackPower = -1;
            targetHitRate = -1;
            targetCriticalRate = -1;
            targetAttackCount = 0;
        }
    }

    /// <summary>
    /// Battles the start.
    /// </summary>
    /// <param name="callBackEvent">イベント終了時に呼び出される処理</param>
    public void BattleStart(Action callBackEvent)
    {
        this.callBackEvent = callBackEvent;

        while (true)
        {
            myAttackCount--;
            if (-1 < myAttackCount--)
            {
                // 攻撃判定（通常攻撃/クリティカル/失敗）の判定
                myAttackState = GameManager.GetCommonCalc().ProbabilityDecision(myCriticalRate) ?
                    Enums.BATTLE.CRITICAL :
                        GameManager.GetCommonCalc().GetHitDecision(myHitRate) ?
                            Enums.BATTLE.NORMAL :
                            Enums.BATTLE.MISS;

                // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                attackEvent.battleManager = this;
                attackEvent.myUnitObj = myUnitObj;
                attackEvent.myAttackPower = myAttackPower;
                attackEvent.myAttackState = myAttackState;
                attackEvent.targetUnitObj = targetUnitObj;
                attackEvent.targetHPText = targetHPText;
                attackEvent.cutInAnimController = cutInAnimController;
                AddEvent(attackEvent);
            }

            if (-1 < targetAttackCount--)
            {
                // 攻撃判定（通常攻撃/クリティカル/失敗）の判定
                targetAttackState = GameManager.GetCommonCalc().ProbabilityDecision(targetCriticalRate) ?
                    Enums.BATTLE.CRITICAL :
                        GameManager.GetCommonCalc().GetHitDecision(targetHitRate) ?
                            Enums.BATTLE.NORMAL :
                            Enums.BATTLE.MISS;

                // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                attackEvent.battleManager = this;
                attackEvent.myUnitObj = targetUnitObj;
                attackEvent.myAttackPower = targetAttackPower;
                attackEvent.myAttackState = targetAttackState;
                attackEvent.targetUnitObj = myUnitObj;
                attackEvent.targetHPText = myHPText;
                attackEvent.cutInAnimController = cutInAnimController;
                AddEvent(attackEvent);
            }

            // 戦闘イベント登録終了
            if (myAttackCount <= 0 && targetAttackCount <= 0) break;
        }

        // イベントの開始
        isStart = true;
    }

    /// <summary>
    /// Adds the event.
    /// </summary>
    /// <param name="eventFunc">Event func.</param>
    public void AddEvent(MonoBehaviour eventFunc)
    {
        eventFunc.enabled = false; // スクリプトのアタッチ直後に無効化する
        eventFuncs.Add(eventFunc);
    }

    /// <summary>
    /// Ends the event.
    /// </summary>
    public void EndEvent()
    {
        // 現在のイベントを削除
        eventFuncs.RemoveAt(0);
        eventExecuting = false;

        // イベント全てが終わったら
        if (eventFuncs.Count == 0)
        {
            isStart = false;
            callBackEvent();
        }
    }
}
