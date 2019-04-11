using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelUpController : MonoBehaviour
{
    const int VALUE_MAX = 40; // グラフの最大値
    const float RADIUS = 4f; // グラフの半径
    const float LINE_WIDTH = 0.01f; // 罫線の太さ

    const float UPDATE_SPAWN = 0.35f; // 更新速度(0.35)
    private float spawn;

    // ステータスリスト
    private Struct.NodeStatus[] statusList = new Struct.NodeStatus[7];

    public Image faceImage;
    public Text unitName;
    public Text unitClass;
    public Text levelText;
    private List<Action> eventList = new List<Action>();

    private UnitInfo unitInfo; // 表示するユニット情報
    private Struct.UnitClassData maxUnitInfo; // 表示するユニットの最大ステータス

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

    private void Update()
    {
        if (0 < eventList.Count)
        {
            // 一定間隔でイベントを実行
            if (UPDATE_SPAWN < (spawn += Time.deltaTime))
            {
                // イベントを一つずつ実行
                eventList[0]();

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
        this.maxUnitInfo = UnitInfo.GetUnitClassData(unitInfo.ClassType);
        spawn = 0;

        // UIにレベルアップ前のパラメータをセット
        faceImage.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Unit" + unitInfo.Id);
        unitName.text = unitInfo.UnitName;
        unitClass.text = unitInfo.UnitClassName;
        levelText.text = String.Format("Lv: {0}", unitInfo.Level);
        statusList[0] = new Struct.NodeStatus("VIT", unitInfo.Vitality, maxUnitInfo.Vitality);
        statusList[1] = new Struct.NodeStatus("STR", unitInfo.Strengtht, maxUnitInfo.Attack);
        statusList[2] = new Struct.NodeStatus("TEC", unitInfo.Technical, maxUnitInfo.Technical);
        statusList[3] = new Struct.NodeStatus("SPPD", unitInfo.Speed, maxUnitInfo.Speed);
        statusList[4] = new Struct.NodeStatus("DEF", unitInfo.Defense, maxUnitInfo.Defense);
        statusList[5] = new Struct.NodeStatus("RES", unitInfo.Resist, maxUnitInfo.Defense);
        statusList[6] = new Struct.NodeStatus("LUK", unitInfo.Luck, maxUnitInfo.Luck);

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

        // UIを表示
        gameObject.SetActive(true);

        // 1秒後にレベルアップ処理を開始
        StartCoroutine(DelayMethod(1f, () =>
        {
            int oldLevel;
            int oldVitality;
            int oldStrengtht;
            int oldTechnical;
            int oldSpeed;
            int oldDefense;
            int oldResist;
            int oldLuck;

            int newLevel;
            int newVitality;
            int newStrengtht;
            int newTechnical;
            int newSpeed;
            int newDefense;
            int newResist;
            int newLuck;

            for (int i = 0; i < addLevel; i++)
            {
                // 現在のステータスを取得
                oldLevel = unitInfo.Level;
                oldVitality = unitInfo.Vitality;
                oldStrengtht = unitInfo.Strengtht;
                oldTechnical = unitInfo.Technical;
                oldSpeed = unitInfo.Speed;
                oldDefense = unitInfo.Defense;
                oldResist = unitInfo.Resist;
                oldLuck = unitInfo.Luck;

                newLevel = unitInfo.Level;
                newVitality = unitInfo.Vitality;
                newStrengtht = unitInfo.Strengtht;
                newTechnical = unitInfo.Technical;
                newSpeed = unitInfo.Speed;
                newDefense = unitInfo.Defense;
                newResist = unitInfo.Resist;
                newLuck = unitInfo.Luck;

                // レベル加算
                newLevel++;
                eventList.Add(() =>
                {
                    ReDraw(newLevel,
                    unitInfo.Vitality,
                    oldStrengtht,
                    oldTechnical,
                    oldSpeed,
                    oldDefense,
                    oldResist,
                    oldLuck);
                });

                // 体力
                if (unitInfo.Vitality < maxUnitInfo.Vitality)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Vitality + 30))
                    {
                        newVitality++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(0, newVitality);
                            ReDraw(newLevel,
                            newVitality,
                            oldStrengtht,
                            oldTechnical,
                            oldSpeed,
                            oldDefense,
                            oldResist,
                            oldLuck);
                        });
                    }

                // 力
                if (unitInfo.Strengtht < maxUnitInfo.Attack)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Attack + 30))
                    {
                        newStrengtht++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(1, newStrengtht);
                            ReDraw(newLevel,
                            newVitality,
                            newStrengtht,
                            oldTechnical,
                            oldSpeed,
                            oldDefense,
                            oldResist,
                            oldLuck);
                        });
                    }

                // 技量
                if (unitInfo.Technical < maxUnitInfo.Technical)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Technical + 30))
                    {
                        newTechnical++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(2, newTechnical);
                            ReDraw(newLevel,
                            newVitality,
                            newStrengtht,
                            newTechnical,
                            oldSpeed,
                            oldDefense,
                            oldResist,
                            oldLuck);
                        });
                    }

                // 速さ
                if (unitInfo.Speed < maxUnitInfo.Speed)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Speed + 30))
                    {
                        newSpeed++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(3, newSpeed);
                            ReDraw(newLevel,
                            newVitality,
                            newStrengtht,
                            newTechnical,
                            newSpeed,
                            oldDefense,
                            oldResist,
                            oldLuck);
                        });
                    }

                // 防御
                if (unitInfo.Defense < maxUnitInfo.Defense)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Defense + 30))
                    {
                        newDefense++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(4, newDefense);
                            ReDraw(newLevel,
                             newVitality,
                             newStrengtht,
                             newTechnical,
                             newSpeed,
                             newDefense,
                             oldResist,
                             oldLuck);
                        });
                    }

                // 魔防
                if (unitInfo.Resist < maxUnitInfo.Resist)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Resist + 30))
                    {
                        newResist++;
                        eventList.Add(() =>
                        {
                            rCSLevelUpLabel.CreateText(5, newResist);
                            ReDraw(newLevel,
                            newVitality,
                            newStrengtht,
                            newTechnical,
                            newSpeed,
                            newDefense,
                            newResist,
                            oldLuck);
                        });
                    }

                // 運
                if (unitInfo.Luck < maxUnitInfo.Luck)
                    if (GameManager.GetCommonCalc().ProbabilityDecision(maxUnitInfo.Luck + 30))
                    {
                        eventList.Add(() =>
                        {
                            newLuck++;
                            rCSLevelUpLabel.CreateText(6, newLuck);
                            ReDraw(newLevel,
                            newVitality,
                            newStrengtht,
                            newTechnical,
                            newSpeed,
                            newDefense,
                            newResist,
                            newLuck);
                        });
                    }

                // ステータスの更新
                unitInfo.NewStatus(newLevel, newVitality, newStrengtht, newTechnical, newSpeed, newDefense, newResist, newLuck);

                // ラベルのリセット
                eventList.Add(rCSLevelUpLabel.Reset);
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
    private void ReDraw(int level, int vitality, int strengtht, int technical, int speed, int defense, int resist, int luck)
    {
        // UIに各パラメータをセット
        levelText.text = String.Format("Lv: {0}", level);
        statusList[0].val = vitality;
        statusList[1].val = strengtht;
        statusList[2].val = technical;
        statusList[3].val = speed;
        statusList[4].val = defense;
        statusList[5].val = resist;
        statusList[6].val = luck;

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