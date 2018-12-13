using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Phaseの管理
/// </summary>
public class PhaseManager : MonoBehaviour
{
    // UI
    public GameObject activeMenuUI;
    public GameObject battleStandbyUI;
    public GameObject selectUnitInfoUI;
    public GameObject cellInfoUI;

    // マネージャースクリプト
    public CursorManager cursorManager;
    public BattleManager battleManager;
    PhaseManager phaseManager;
    RouteManager routeManager;

    [HideInInspector]
    public Vector3 cursorPos, oldCursorPos;

    // フォーカス（選択中）Unit関
    [HideInInspector]
    public GameObject focusUnitObj;
    [HideInInspector]
    public Vector3 oldFocusUnitPos;
    [HideInInspector]
    public List<Vector3> moveRoot; // 移動ルートの座標引き渡し用
    [HideInInspector]
    public Struct.NodeMove[,] activeAreaList; // 行動可能エリアを管理する配列
    [HideInInspector]
    public Struct.NodeMove[,] attackAreaList;

    // エリア描画用関連
    [HideInInspector]
    public GameObject attackArea;
    [HideInInspector]
    public GameObject activeArea;
    [HideInInspector]
    public GameObject rootArea;
    public GameObject areaBlue;
    public GameObject areaRed;
    public GameObject markerObj;
    public Sprite[] makerSprites;

    // 行動ターン
    Enums.PHASE phase = Enums.PHASE.START;

    // 戦闘関係のメンバー変数
    bool isBattle = false;
    int playerHP; // プレイヤーUnitのHP
    int playerAttackPower; // プレイヤーUnitの攻撃力
    int playerAvoidance; // プレイヤーUnitの回避率
    int playerDeathblow; // プレイヤーUnitの必殺率
    int enemyHP; // 敵UnitのHP
    int enemyAttackPower; // 敵Unitの攻撃力
    int enemyAvoidance;  // 敵Unitの回避率
    int enemyDeathblow; // 敵Unitの必殺率

    void Start()
    {
        // UIの非表示
        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        selectUnitInfoUI.SetActive(false);
        cellInfoUI.SetActive(false);

        // インスタンスの初期化
        routeManager = new RouteManager();
        phaseManager = this;

        // エリア描画用関連の読み込み
        attackArea = new GameObject("AttackArea");
        activeArea = new GameObject("ActiveArea");
        rootArea = new GameObject("rootArea");
        attackArea.transform.parent = transform;
        activeArea.transform.parent = transform;
        rootArea.transform.parent = transform;
    }

    public void Update()
    {
        switch (phase)
        {
            case Enums.PHASE.START:
                StartPhase();
                break;

            case Enums.PHASE.SELECT:
                StandbyPhase();
                break;

            case Enums.PHASE.FOCUS:
                FoucusPhase();
                break;

            case Enums.PHASE.MOVE:
                MovePhase();
                break;

            case Enums.PHASE.BATTLE_STANDBY:
                BattleStandbyPhase();
                break;

            case Enums.PHASE.BATTLE:
                BattlePhase();
                break;

            case Enums.PHASE.RESULT:
                ResultPhase();
                break;

            case Enums.PHASE.END:
                EndPhase();
                break;
        }
    }

    /// <summary>
    /// 外部変更用
    /// </summary>
    /// <param name="newPhase">New phase.</param>
    public void ChangePhase(Enums.PHASE newPhase)
    {
        phase = newPhase;
    }

    /// <summary>
    /// ターン開始時
    /// </summary>
    void StartPhase()
    {
        // ターンとUI切り替え
        phase = Enums.PHASE.SELECT;
        selectUnitInfoUI.SetActive(true);
        cellInfoUI.SetActive(true);
    }

    /// <summary>
    /// ユニット選択前
    /// </summary>
    void StandbyPhase()
    {
        // カーソルが更新されたら
        if (cursorPos != oldCursorPos)
        {
            // カーソルの更新
            oldCursorPos = cursorPos;

            // セル情報の更新
            cellInfoUI.GetComponent<CellInfo>().SetData(MapManager.GetFieldData().cells[-(int)cursorPos.y, (int)cursorPos.x]);

            // ユニット情報の更新
            if (Main.GameManager.GetMapUnitInfo(cursorPos))
            {
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().ShowUnitInfo(Main.GameManager.GetMapUnitInfo(cursorPos));
            }
            else
            {
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().CloseUnitInfo();
            }
        }

        // クリック処理
        if (Input.GetMouseButtonDown(0))
        {
            if (Main.GameManager.GetMapUnitInfo(cursorPos) != null && activeAreaList == null)
                if (!Main.GameManager.GetMapUnitInfo(cursorPos).isMoving)
                    AddActiveArea(); // 未行動のユニットであればフォーカスする
        }
    }

    /// <summary>
    /// ユニット選択後
    /// </summary>
    void FoucusPhase()
    {
        // 移動マーカの更新
        AddMarker();

        if (Input.GetMouseButtonDown(0))
            // アクティブエリア（移動可能マス）を選択されたら移動する
            if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.MOVE)
            {
                // 他ユニットがいなければ
                if (!Main.GameManager.GetMapUnitInfo(cursorPos))
                {
                    // ユニットの移動前の座標を保存
                oldFocusUnitPos = focusUnitObj.transform.position;

                    // 移動可能エリアがクリックされたら移動する
                    focusUnitObj.GetComponent<MoveController>().setMoveRoots(moveRoot);

                    // ターンとUI切り替え
                    phase = Enums.PHASE.MOVE;
                    rootArea.SetActive(false);
                    cursorManager.cursorObj.SetActive(false);
                    activeArea.SetActive(false);
                }
            }
            else // アクティブエリア外をクリックされたらフォーカスを外す
            {
                // アニメーションを元に戻す
                focusUnitObj.GetComponent<MoveController>().FocuseEnd();
                focusUnitObj = null;

                // ターンとUI切り替え
                phase = Enums.PHASE.SELECT;
                RemoveMarker();
                RemoveActiveArea();
            }
    }

    /// <summary>
    /// ユニットの移動
    /// </summary>
    void MovePhase()
    {
        // 移動が終わったらUIを切り替える
        if (!focusUnitObj.GetComponent<MoveController>().movingFlg)
        {
            attackArea.SetActive(true);

            activeMenuUI.SetActive(true);
        }
    }

    /// <summary>
    /// 戦闘するユニットの選択
    /// </summary>
    void BattleStandbyPhase()
    {

        // 攻撃範囲の描画
        if (attackAreaList == null)
        {
            AddAttackArea();
        }
        else
        {
            // カーソルを敵ユニットに合わせた時の処理
            if (Main.GameManager.GetMapUnitInfo(cursorPos) &&
                Main.GameManager.GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY &&
                attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK)
            {
                battleStandbyUI.SetActive(true); // UIの切り替え

                // TODO
                playerAttackPower = 10;
            }
            else
            {
                battleStandbyUI.SetActive(false); // UIの切り替え
            }

            // クリック処理
            if (Input.GetMouseButtonDown(0))
            {
                // アクティブエリア（攻撃可能マス）で攻撃対象を選択する
                if (attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK)
                {
                    // 敵プレイヤーをタップしたら
                    if (Main.GameManager.GetMapUnitInfo(cursorPos) &&
                        Main.GameManager.GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY)
                    {
                        // 戦闘開始
                        // ターンとUIの切り替え
                        cursorManager.cursorObj.SetActive(false);
                        battleStandbyUI.SetActive(false);
                        attackArea.SetActive(false);
                        phase = Enums.PHASE.BATTLE;
                    }
                }
                else
                {
                    // UIの切り替え
                    OnCancelBattleStandby();
                }
            }
        }
    }

    /// <summary>
    /// ユニットとの戦闘
    /// </summary>
    void BattlePhase()
    {
        if (!isBattle)
        {
            // 戦闘前に戦闘結果を演算しイベントを登録する

            // プレイヤーターンのバトル処理
            GameObject enemyUnitObj = Main.GameManager.GetMapUnitObj(cursorPos);

            // イベントの発生チェックと登録

            // 必殺の検証
            bool deathblowFlg = RandomCheck(playerDeathblow);
            if (deathblowFlg || RandomCheck(playerAvoidance))
            {



                // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                battleManager.AddEvent(new AttackEvent(ref focusUnitObj, ref enemyUnitObj, playerAttackPower, deathblowFlg));
            }
            else
            {
                // 攻撃失敗

            }


            //eventFunc = new AvoidanceEvent("ccc");
            //events.Add(eventFunc);
            //eventFunc = new ConversationEvent("eee");
            //events.Add(eventFunc);
            //eventFunc = new DeathblowEvent("fff");
            //events.Add(eventFunc);
            //eventFunc = new LevelUpEvent("fff");
            //events.Add(eventFunc);
            //eventFunc = new ExpUpEvent("fff");
            //events.Add(eventFunc);
            //eventFunc = new PlayerWinEvent("fff");
            //events.Add(eventFunc);
            //eventFunc = new PlayerLoseEvent("fff");
            //events.Add(eventFunc);

            // バトルの実行
            battleManager.StartEvent();
            isBattle = true;
        }
        else
        {
            // 全てのイベントが終了したらフェーズを変える
            if (!battleManager.isBattle())
            {
                isBattle = false;
                // ターンとUIの切り替え
                phase = Enums.PHASE.RESULT; // 攻撃終了
            }
        }
    }

    /// <summary>
    /// ユニット同士の戦闘終了後
    /// </summary>
    void ResultPhase()
    {
        focusUnitObj.GetComponent<MoveController>().FocuseEnd();
        focusUnitObj = null;
        RemoveActiveArea();
        RemoveMarker();
        RemoveAttackArea();
        cursorManager.cursorObj.SetActive(true);
        phase = Enums.PHASE.SELECT;
    }

    /// <summary>
    /// ターン終了時
    /// </summary>
    void EndPhase()
    {

    }

    /// <summary>
    /// Ons the attack button.
    /// </summary>
    public void OnAttackBtn()
    {
        // ターンとUI切り替え
        phase = Enums.PHASE.BATTLE_STANDBY;
        activeArea.SetActive(false);
        activeMenuUI.SetActive(false);
        rootArea.SetActive(false);
        cursorManager.cursorObj.SetActive(true);
    }

    /// <summary>
    /// 行動画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelActiveMenu()
    {
        // アニメーションを元に戻す
        if (focusUnitObj) focusUnitObj.GetComponent<MoveController>().FocuseEnd();

        // ユニットの座標を元に戻す
        focusUnitObj.GetComponent<MoveController>().DirectMove(oldFocusUnitPos);

        RemoveActiveArea();
        RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        phase = Enums.PHASE.SELECT;
        activeMenuUI.SetActive(false);
        cursorManager.cursorObj.SetActive(true);
    }

    /// <summary>
    /// 攻撃選択画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelBattleStandby()
    {
        RemoveAttackArea();

        // ターンとUIの切り替え
        phase = Enums.PHASE.MOVE;
        battleStandbyUI.SetActive(false);
        cursorManager.cursorObj.SetActive(false);
        rootArea.SetActive(true);
        activeArea.SetActive(true);
    }

    /// <summary>
    /// 行動終了ボタン処理
    /// </summary>
    public void OnEndBtn()
    {
        // アニメーションを元に戻す
        if (focusUnitObj) focusUnitObj.GetComponent<MoveController>().FocuseEnd();

        // ユニット管理リストの更新
        Main.GameManager.MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);

        RemoveActiveArea();
        RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        phase = Enums.PHASE.SELECT;
        activeMenuUI.SetActive(false);
        cursorManager.cursorObj.SetActive(true);
    }

    /// <summary>
    /// 引数の確率の検証
    /// </summary>
    /// <returns><c>true</c>, if check was randomed, <c>false</c> otherwise.</returns>
    /// <param name="probability">Probability.</param>
    bool RandomCheck(int probability)
    {
        return probability <= UnityEngine.Random.Range(1, 101) ? true : false;
    }

    /// <summary>
    /// アクティブエリアの表示
    /// </summary>
    public void AddActiveArea()
    {
        // フォーカスユニットの取得
        focusUnitObj = Main.GameManager.GetMapUnitObj(cursorPos);

        // アクティブリストの生成と検証
        activeAreaList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];

        // 移動エリアの検証
        routeManager.CheckMoveArea(ref phaseManager);

        // エリアパネルの表示
        for (int y = 0; y < MapManager.GetFieldData().height; y++)
            for (int x = 0; x < MapManager.GetFieldData().width; x++)
                if (activeAreaList[y, x].aREA == Enums.AREA.MOVE || activeAreaList[y, x].aREA == Enums.AREA.UNIT)
                {
                    // 移動エリアの表示
                    Instantiate(areaBlue, new Vector3(x, -y, 0), Quaternion.identity).transform.parent = activeArea.transform;
                    UnitInfo a = focusUnitObj.GetComponent<UnitInfo>();
                    // 攻撃エリアの検証
                    routeManager.CheckAttackArea(ref activeAreaList, new Vector3(x, -y, 0), focusUnitObj.GetComponent<UnitInfo>().attackRange);
                }

        // 攻撃エリアの表示
        for (int ay = 0; ay < MapManager.GetFieldData().height; ay++)
            for (int ax = 0; ax < MapManager.GetFieldData().width; ax++)
                if (activeAreaList[ay, ax].aREA == Enums.AREA.ATTACK)
                    Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = activeArea.transform;

        // ターンとUIの切り替え
        phase = Enums.PHASE.FOCUS;
        selectUnitInfoUI.SetActive(false);
        cellInfoUI.SetActive(false);
        activeArea.SetActive(true);
        rootArea.SetActive(true);
    }


    /// <summary>
    /// 攻撃エリアの表示
    /// </summary>
    public void AddAttackArea()
    {
        // アクティブリストの生成と検証
        attackAreaList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];

        // 攻撃エリアの検証と表示
        routeManager.CheckAttackArea(ref attackAreaList, focusUnitObj.transform.position, focusUnitObj.GetComponent<UnitInfo>().attackRange);
        for (int ay = 0; ay < MapManager.GetFieldData().height; ay++)
            for (int ax = 0; ax < MapManager.GetFieldData().width; ax++)
                if (attackAreaList[ay, ax].aREA == Enums.AREA.ATTACK)
                   Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = attackArea.transform;
    }

    /// <summary>
    /// markerの表示
    /// </summary>
    public void AddMarker()
    {
        // アクティブエリアがあるなら、マーカを表示する
        if (activeAreaList != null)
            // 移動エリア内ならマーカを表示する
            if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.MOVE)
            {
                // マーカの削除
                RemoveMarker();

                // 目標までのルートを取得
                routeManager.CheckShortestRoute(ref phaseManager, cursorPos);

                // マーカの生成とスプライト変更
                Vector3 nextPos = focusUnitObj.transform.position;
                int spriteId = 0;
                Quaternion angle = Quaternion.identity;
                int moveRootCount = moveRoot.Count;
                if (moveRootCount != 0)
                {
                    if (moveRoot[0] == Vector3.down) angle.eulerAngles = new Vector3(180, 0, 0);
                    else if (moveRoot[0] == Vector3.left) angle.eulerAngles = new Vector3(0, 0, 90);
                    else if (moveRoot[0] == Vector3.right) angle.eulerAngles = new Vector3(0, 0, -90);
                    markerObj.GetComponent<SpriteRenderer>().sprite = makerSprites[spriteId];
                    Instantiate(markerObj, nextPos, angle).transform.parent = rootArea.transform;
                }
                for (int i = 0; i < moveRootCount; i++)
                {
                    if (moveRoot[i] == Vector3.up)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle = Quaternion.identity;
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.up)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.left ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle = Quaternion.identity;
                                spriteId = 1;
                            }
                        }
                    }
                    else if (moveRoot[i] == Vector3.down)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, 180);
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.down)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.left ? new Vector3(0, 0, 180) : new Vector3(180, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, 180);
                                spriteId = 1;
                            }
                        }

                    }
                    else if (moveRoot[i] == Vector3.right)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, -90);
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.right)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.up ? new Vector3(0, 180, 90) : new Vector3(0, 0, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, -90);
                                spriteId = 1;
                            }
                        }
                    }
                    else
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, 90);
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.left)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.up ? new Vector3(0, 0, 90) : new Vector3(0, 180, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, 90);
                                spriteId = 1;
                            }
                        }
                    }
                    markerObj.GetComponent<SpriteRenderer>().sprite = makerSprites[spriteId];
                    Instantiate(markerObj, nextPos += moveRoot[i], angle).transform.parent = rootArea.transform;
                }
            }
            else if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.UNIT)
                RemoveMarker(); // カーソルがユニット上なら表示しない
    }

    /// <summary>
    /// 行動エリアの初期化と削除
    /// </summary>
    public void RemoveActiveArea()
    {
        activeAreaList = null;
        foreach (Transform a in activeArea.transform) Destroy(a.gameObject);
    }

    /// <summary>
    /// 攻撃エリアの初期化と削除
    /// </summary>
    public void RemoveAttackArea()
    {
        attackAreaList = null;
        foreach (Transform a in attackArea.transform) Destroy(a.gameObject);
    }

    /// <summary>
    /// マーカの削除
    /// </summary>
    public void RemoveMarker() { foreach (Transform r in rootArea.transform) Destroy(r.gameObject); }

    /// <summary>
    /// カーソルの更新時に呼ばれる
    /// </summary>
    /// <param name="newPos">New position.</param>
    public void cursorUpdate(Vector3 newPos) { cursorPos = newPos; }
}
