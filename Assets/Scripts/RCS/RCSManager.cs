using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ユニット詳細画面のレーダーチャート管理用クラス
/// </summary>
public class RCSManager : MonoBehaviour
{
    // ステータスリスト
    List<Struct.NodeStatus> statusList = new List<Struct.NodeStatus>();

    const int VALUE_MAX = 50; // グラフの最大値
    const float RADIUS = 2.8f; // グラフの半径
    const float LINE_WIDTH = 0.01f; // 罫線の太さ

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

    private void Start()
    {
        // テストデータ
        statusList.Add(new Struct.NodeStatus("体力", 55, 50));
        statusList.Add(new Struct.NodeStatus("筋力", 19, 40));
        statusList.Add(new Struct.NodeStatus("魔力", 26, 26));
        statusList.Add(new Struct.NodeStatus("技量", 0, 36));
        statusList.Add(new Struct.NodeStatus("速さ", 10, 40));
        statusList.Add(new Struct.NodeStatus("防御", 11, 22));
        statusList.Add(new Struct.NodeStatus("魔防", 22, 26));
        statusList.Add(new Struct.NodeStatus("幸運", 12, 30));
        ViewUpdate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            statusList.Add(new Struct.NodeStatus("追加", 10, 30));
            ViewUpdate();
        }
    }

    /// <summary>
    /// レーダーチャートの更新
    /// </summary>
    private void ViewUpdate()
    {
        rCSValMax.color = max;
        rCSValMax.radius = RADIUS;
        rCSValMax.statusListCount = statusList.Count;
        rCSValMax.statusValMax = VALUE_MAX;
        rCSValMax.SetVerticesDirty();

        rCSValJob.color = job;
        rCSValJob.radius = RADIUS;
        rCSValJob.statusJobList = statusList;
        rCSValJob.statusListCount = statusList.Count;
        rCSValJob.statusValMax = VALUE_MAX;
        rCSValJob.SetVerticesDirty();

        rCSVal.color = status;
        rCSVal.radius = RADIUS;
        rCSVal.statusList = statusList;
        rCSVal.statusListCount = statusList.Count;
        rCSVal.statusValMax = VALUE_MAX;
        rCSVal.SetVerticesDirty();

        rCSLine.color = line;
        rCSLine.radius = RADIUS;
        rCSLine.statusListCount = statusList.Count;
        rCSLine.statusValMax = VALUE_MAX;
        rCSLine.LineWidth = LINE_WIDTH;
        rCSLine.SetVerticesDirty();

        rCSLabel.radius = RADIUS;
        rCSLabel.statusList = statusList;
        rCSLabel.statusListCount = statusList.Count;
        rCSLabel.statusValMax = VALUE_MAX;
        rCSLabel.CreateText();

        rCSLabelVal.radius = RADIUS;
        rCSLabelVal.statusList = statusList;
        rCSLabelVal.statusListCount = statusList.Count;
        rCSLabelVal.statusValMax = VALUE_MAX;
        rCSLabelVal.CreateText();
    }
}