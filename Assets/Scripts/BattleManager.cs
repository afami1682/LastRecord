using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    List<BattleFunc> eventFuncs = new List<BattleFunc>(); // バトル処理の全体イベント
    BattleFunc eventFunc; // バトル処理の単体イベント
    bool oneEventFlg = false; // 単体のイベント実行中かどうかのフラグ
    bool allEventFlg = false; // 全体のイベント実行中かどうかのフラグ

    void Update()
    {
        // 一つずつイベントを実行していく
        if (allEventFlg & !oneEventFlg & 0 < eventFuncs.Count)
        {
            // リストの先頭を取得&削除
            eventFunc = eventFuncs[0];
            eventFuncs.RemoveAt(0);

            // イベント開始
            oneEventFlg = true;
        }


        // 一つずつのイベントの実行
        if (oneEventFlg)
        {
           
            // イベントが終了したら次のイベントを実行する
            if (!eventFunc.Run())
            {
                // 移動終了
                oneEventFlg = false;

                // イベント全てが終わったら
                if (eventFuncs.Count == 0)
                    allEventFlg = false;
            }
        }
    }

    /// <summary>
    /// Starts the event.
    /// </summary>
    public void StartEvent()
    {
        allEventFlg = true;
    }

    /// <summary>
    /// Ises the battle.
    /// </summary>
    /// <returns><c>true</c>, if battle was ised, <c>false</c> otherwise.</returns>
    public bool isBattle()
    {
        return allEventFlg;
    }

    /// <summary>
    /// Adds the event.
    /// </summary>
    /// <param name="eventFunc">Event func.</param>
    public void AddEvent(BattleFunc eventFunc)
    {
        eventFuncs.Add(eventFunc);
    }
}
