using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelUpController : MonoBehaviour
{

    // ステータスリスト
    Struct.NodeStatus[] statusList = new Struct.NodeStatus[8];

    const int VALUE_MAX = 40; // グラフの最大値
    const float RADIUS = 4f; // グラフの半径
    const float LINE_WIDTH = 0.01f; // 罫線の太さ

    const float UPDATE_SPAWN = 0.4f; // 更新速度
    float spawn = 0;

    public Image faceImage;
    public Text unitName;
    public Text unitClass;
    public Text level;
    List<Action> eventList = new List<Action>();

    UnitInfo unitInfo; // 表示するユニット情報
    Struct.UnitClassData maxUnitInfo; // 表示するユニットの最大ステータス

    /// <summary>
    ///  色
    /// </summary>
    public Color max;
    public Color job;
    public Color status;
    public Color line;

    /// <summary>
    /// インスタンス
    /// </summary>
    public RCSValMax rCSValMax; // ステータス最大値
    public RCSValJob rCSValJob; // 各Jobの最大ステータス値
    public RCSVal rCSVal; // 各ステータス値
    public RCSLine rCSLine; // 罫線
    public RCSLabel rCSLabel; // ラベル
    public RCSLabelVal rCSLabelVal; // 値
    public RCSLevelUpLabel rCSLevelUpLabel; // レベルアップ時の+1

    private void Start()
    {
        gameObject.SetActive(false);
        // 描画確認用
        //statusList.Add(new Struct.NodeStatus("体力", 30, 50));
        //statusList.Add(new Struct.NodeStatus("筋力", 30, 40));
        //statusList.Add(new Struct.NodeStatus("魔力", 30, 26));
        //statusList.Add(new Struct.NodeStatus("技量", 30, 36));
        //statusList.Add(new Struct.NodeStatus("速さ", 30, 40));
        //statusList.Add(new Struct.NodeStatus("防御", 30, 22));
        //statusList.Add(new Struct.NodeStatus("魔防", 30, 26));
        //statusList.Add(new Struct.NodeStatus("幸運", 30, 30));
        //ViewUpdate();
    }

    private void Update()
    {
        if (0 < eventList.Count)
        {
            // 一定間隔でイベントを実行
            if (UPDATE_SPAWN < (spawn += Time.deltaTime))
            {
                // イベントを一つずつ実行
                eventList[0]();
                ReDraw(unitInfo);

                eventList.RemoveAt(0);

                spawn = 0;
            }
        }
    }

    /// <summary>
    /// レベルアップイベント
    /// </summary>
    /// <param name="unitInfo"></param>
    /// <param name="addLevel"></param>
    /// <param name="callBackEvent"></param>
    public void LevelUpEvent(UnitInfo unitInfo, int addLevel, Action callBackEvent)
    {
        this.unitInfo = unitInfo;
        this.maxUnitInfo = UnitInfo.GetUnitClassData(unitInfo.classType);

        // UIに各パラメータをセット
        faceImage.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitInfo.id);
        unitName.text = unitInfo.unitName;
        unitClass.text = unitInfo.classTypeName;
        level.text = unitInfo.level.ToString();
        statusList[0] = new Struct.NodeStatus("体力", unitInfo.vitality, maxUnitInfo.vitality);
        statusList[1] = new Struct.NodeStatus("筋力", unitInfo.strength, maxUnitInfo.strength);
        statusList[2] = new Struct.NodeStatus("魔力", unitInfo.intelligence, maxUnitInfo.intelligence);
        statusList[3] = new Struct.NodeStatus("技量", unitInfo.dexterity, maxUnitInfo.dexterity);
        statusList[4] = new Struct.NodeStatus("速さ", unitInfo.speed, maxUnitInfo.speed);
        statusList[5] = new Struct.NodeStatus("防御", unitInfo.defense, maxUnitInfo.defense);
        statusList[6] = new Struct.NodeStatus("魔防", unitInfo.mDefense, maxUnitInfo.mDefense);
        statusList[7] = new Struct.NodeStatus("幸運", unitInfo.luck, maxUnitInfo.luck);

        rCSValMax.color = max;
        rCSValMax.radius = RADIUS;
        rCSValMax.statusListCount = statusList.Length;
        rCSValMax.statusValMax = VALUE_MAX;
        rCSValMax.SetVerticesDirty();

        rCSValJob.color = job;
        rCSValJob.radius = RADIUS;
        rCSValJob.statusJobList = statusList;
        rCSValJob.statusListCount = statusList.Length;
        rCSValJob.statusValMax = VALUE_MAX;
        rCSValJob.SetVerticesDirty();

        rCSVal.color = status;
        rCSVal.radius = RADIUS;
        rCSVal.statusList = statusList;
        rCSVal.statusListCount = statusList.Length;
        rCSVal.statusValMax = VALUE_MAX;
        rCSVal.SetVerticesDirty();

        rCSLine.color = line;
        rCSLine.radius = RADIUS;
        rCSLine.statusListCount = statusList.Length;
        rCSLine.statusValMax = VALUE_MAX;
        rCSLine.LineWidth = LINE_WIDTH;
        rCSLine.SetVerticesDirty();

        rCSLabel.radius = RADIUS;
        rCSLabel.statusList = statusList;
        rCSLabel.statusListCount = statusList.Length;
        rCSLabel.statusValMax = VALUE_MAX;
        rCSLabel.ReDraw();

        rCSLabelVal.radius = RADIUS;
        rCSLabelVal.statusList = statusList;
        rCSLabelVal.statusListCount = statusList.Length;
        rCSLabelVal.statusValMax = VALUE_MAX;
        rCSLabelVal.ReDraw();

        rCSLevelUpLabel.radius = RADIUS;
        rCSLevelUpLabel.statusList = statusList;
        rCSLevelUpLabel.statusListCount = statusList.Length;
        rCSLevelUpLabel.statusValMax = VALUE_MAX;

        gameObject.SetActive(true);

        // 1秒後にイベントを開始
        StartCoroutine(DelayMethod(1f, () =>
        {
            // イベントの登録
            int addVal = 1;
            for (int i = 0; i < addLevel; i++)
            {
                // レベル加算
                eventList.Add(() =>
                {
                    unitInfo.level += addVal;
                });

                if (unitInfo.vitality < maxUnitInfo.vitality)
                    if(GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.vitality + 30))
                    eventList.Add(() =>
                    {
                        unitInfo.vitality += addVal;
                        rCSLevelUpLabel.CreateText(0, addVal);
                    });

                if (unitInfo.strength < maxUnitInfo.strength)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.strength + 30))
                        eventList.Add(() =>
                {
                    unitInfo.strength += addVal;
                    rCSLevelUpLabel.CreateText(1, addVal);
                });

                if (unitInfo.intelligence < maxUnitInfo.intelligence)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.intelligence + 30))
                        eventList.Add(() =>
                {
                    unitInfo.intelligence += addVal;
                    rCSLevelUpLabel.CreateText(2, addVal);
                });

                if (unitInfo.dexterity < maxUnitInfo.dexterity)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.dexterity + 30))
                        eventList.Add(() =>
                {
                    unitInfo.dexterity += addVal;
                    rCSLevelUpLabel.CreateText(3, addVal);
                });

                if (unitInfo.speed < maxUnitInfo.speed)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.speed + 30))
                        eventList.Add(() =>
                {
                    unitInfo.speed += addVal;
                    rCSLevelUpLabel.CreateText(4, addVal);
                });

                if (unitInfo.defense < maxUnitInfo.defense)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.defense + 30))
                        eventList.Add(() =>
                {
                    unitInfo.defense += addVal;
                    rCSLevelUpLabel.CreateText(5, addVal);
                });

                if (unitInfo.mDefense < maxUnitInfo.mDefense)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.mDefense + 30))
                        eventList.Add(() =>
                {
                    unitInfo.mDefense += addVal;
                    rCSLevelUpLabel.CreateText(6, addVal);
                });

                if (unitInfo.luck < maxUnitInfo.luck)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.luck + 30))
                        eventList.Add(() =>
                {
                    unitInfo.luck += addVal;
                    rCSLevelUpLabel.CreateText(7, addVal);
                });

                eventList.Add(() => { rCSLevelUpLabel.Reset(); });
            }

            eventList.Add(() =>
            {
                gameObject.SetActive(false); // UI非表示
                callBackEvent(); // コールバックイベントの実行
            });
        }));
    }

    /// <summary>
    /// UIの再描画
    /// </summary>
    private void ReDraw(UnitInfo unitInfo)
    {
        // UIに各パラメータをセット
        level.text = unitInfo.level.ToString();
        statusList[0].val = unitInfo.vitality;
        statusList[1].val = unitInfo.strength;
        statusList[2].val = unitInfo.intelligence;
        statusList[3].val = unitInfo.dexterity;
        statusList[4].val = unitInfo.speed;
        statusList[5].val = unitInfo.defense;
        statusList[6].val = unitInfo.mDefense;
        statusList[7].val = unitInfo.luck;

        rCSValMax.ReDraw();

        rCSValJob.ReDraw();

        rCSVal.statusList = statusList;
        rCSVal.ReDraw();

        rCSLine.ReDraw();

        rCSLabel.ReDraw();

        rCSLabelVal.statusList = statusList;
        rCSLabelVal.ReDraw();

    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}