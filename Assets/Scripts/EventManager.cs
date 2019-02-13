using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    List<MonoBehaviour> eventFuncs = new List<MonoBehaviour>();
    MonoBehaviour eventFunc;
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

            // イベントの開始(Start()から始まる)
            eventFunc.enabled = oneEventFlg = true;
        }
    }

    /// <summary>
    /// Starts the event.
    /// </summary>
    public void StartEvent()
    {
        // 1件以上のイベントが登録されてるなら開始する
        allEventFlg = (0 < eventFuncs.Count) ? true : false;
    }

    /// <summary>
    /// Nexts the event.
    /// </summary>
    public void NextEvent()
    {
        oneEventFlg = false;

        // イベント全てが終わったら
        if (eventFuncs.Count == 0)
            allEventFlg = false;
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
    public void AddEvent(MonoBehaviour eventFunc)
    {
        eventFunc.enabled = false; // スクリプトのアタッチ直後に無効化する
        eventFuncs.Add(eventFunc);
    }
}
