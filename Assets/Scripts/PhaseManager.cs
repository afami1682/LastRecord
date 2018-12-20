using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    public GameObject cursorObj;
    public Image playerTurnImage, enemyTurnImage;
    Animator turnImageAnim;

    // マネージャースクリプト
    public BattleManager battleManager;
    public MoveMarkerManager moveMarkerManager;
    PhaseManager phaseManager;
    RouteManager routeManager;

    [HideInInspector]
    public Vector3 cursorPos, oldCursorPos;

    // フォーカス（選択中）Unit関連
    [HideInInspector]
    public GameObject focusUnitObj;
    [HideInInspector]
    Vector3 oldFocusUnitPos;
    [HideInInspector]
    public List<Vector3> moveRoot; // 移動ルートの座標引き渡し用
    [HideInInspector]
    public Struct.NodeMove[,] activeAreaList; // 行動可能エリアを管理する配列
    [HideInInspector]
    public Struct.NodeMove[,] attackAreaList; // 攻撃可能エリアを管理する配列

    // エリア描画用関連
    [HideInInspector]
    public GameObject attackArea, activeArea;
    public GameObject areaBlue;
    public GameObject areaRed;

    // バトル用
    bool isBattle = false;
    [HideInInspector]
    public bool playerAttack;
    [HideInInspector]
    public GameObject myUnitObj, enemyUnitObj; // Unitのオブジェクト
    [HideInInspector]
    public int myAttackPower, enemyAttackPower; // 攻撃力
    [HideInInspector]
    public int myDeathblow, enemyDeathblow; // 必殺の発生率
    [HideInInspector]
    public int myAttackCount, enemyAttackCount;// 攻撃回数
    [HideInInspector]
    public int myAccuracy, enemyAccuracy;// 命中率
    [HideInInspector]
    public Enums.BATTLE myAttackState, enemyAttackState; // 攻撃判定
    [HideInInspector]
    public Text textMyHP, textEnemyHP; // 表示用

    // 行動ターン
    Enums.PHASE phase = Enums.PHASE.START;

    // 行動するターンプレイヤー
    Enums.ARMY turnPlayer;

    // 各行動イベント
    Action StartPhase, // プレイヤーのターン開始時
    StandbyPhase, // Unit選択中
    FoucusPhase, // Unit選択時
    MovePhase, // Unit行動時
    BattleStandbyPhase, // Unit攻撃選択時
    BattlePhase, // Unit攻撃時
    ResultPhase, // Unit攻撃終了時（まだ見操作のUnitがいれば、SELECTに戻る）
    EndPhase; // プレイヤーのターン終了時

    void Start()
    {
        // インスタンスの初期化
        routeManager = new RouteManager();
        phaseManager = this;

        // エリア描画用関連の読み込み
        attackArea = new GameObject("AttackArea");
        activeArea = new GameObject("ActiveArea");

        attackArea.transform.parent = transform;
        activeArea.transform.parent = transform;

        // プレイヤーターンから始める
        TurnChange(Enums.ARMY.ALLY);

        // UIの非表示
        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        selectUnitInfoUI.SetActive(false);
        cellInfoUI.SetActive(false);
        playerTurnImage.gameObject.SetActive(false);
        enemyTurnImage.gameObject.SetActive(false);
        cursorObj.SetActive(false);
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
    public void ChangePhase(Enums.PHASE newPhase) { phase = newPhase; }

    /// <summary>
    /// ターンの切り替え処理
    /// </summary>
    /// <param name="player">Turn player.</param>
    void TurnChange(Enums.ARMY player)
    {
        switch (player)
        {
            case Enums.ARMY.ALLY:
                this.turnPlayer = player;
                StartPhase = MyStartPhase;
                StandbyPhase = MyStandbyPhase;
                FoucusPhase = MyFoucusPhase;
                MovePhase = MyMovePhase;
                BattleStandbyPhase = MyBattleStandbyPhase;
                BattlePhase = MyBattlePhase;
                ResultPhase = MyResultPhase;
                EndPhase = MyEndPhase;
                break;

            case Enums.ARMY.ENEMY:
                this.turnPlayer = player;
                StartPhase = EnemyStartPhase;
                StandbyPhase = EnemyStandbyPhase;
                FoucusPhase = EnemyFoucusPhase;
                MovePhase = EnemyMovePhase;
                BattleStandbyPhase = EnemyBattleStandbyPhase;
                BattlePhase = EnemyBattlePhase;
                ResultPhase = EnemyResultPhase;
                EndPhase = EnemyEndPhase;
                break;
        }
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
        moveMarkerManager.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// 行動終了ボタン処理
    /// </summary>
    public void MoveCancel()
    {
        // アニメーションを元に戻す
        if (focusUnitObj)
            focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);

        // ユニットの位置を元に戻す
        focusUnitObj.GetComponent<MoveController>().DirectMove(oldFocusUnitPos);

        RemoveActiveArea();
        RemoveAttackArea();
        moveMarkerManager.RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        phase = Enums.PHASE.SELECT;
        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        cursorObj.SetActive(true);
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
        cursorObj.SetActive(false);
        moveMarkerManager.SetActive(true);
        activeArea.SetActive(true);
    }

    /// <summary>
    /// 引数の確率の検証
    /// </summary>
    /// <returns><c>true</c>, if check was randomed, <c>false</c> otherwise.</returns>
    /// <param name="probability">Probability.</param>
    bool RandomCheck(int probability) { return UnityEngine.Random.Range(1, 101) <= probability ? true : false; }

    /// <summary>
    /// アクティブエリアの表示
    /// </summary>
    public void AddActiveArea()
    {
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
        moveMarkerManager.SetActive(true);
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
    /// カーソルの更新時に呼ばれる
    /// </summary>
    /// <param name="newPos">New position.</param>
    public void cursorUpdate(Vector3 newPos) { cursorPos = newPos; }

    /// <summary>
    /// ターン開始時
    /// </summary>
    void MyStartPhase()
    {
        if (turnImageAnim == null)
        {
            turnImageAnim = playerTurnImage.gameObject.GetComponent<Animator>();
            playerTurnImage.gameObject.SetActive(true);
        }
        else
        {
            // アニメーションが終了したらターンを開始する
            if (!(turnImageAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                // ターンとUIの切り替え
                List<GameObject> list = Main.GameManager.GetUnBehaviorUnit(Enums.ARMY.ALLY);
                if (list.Count == 0)
                    phase = Enums.PHASE.END;
                else
                    phase = Enums.PHASE.SELECT;

                selectUnitInfoUI.SetActive(true);
                cellInfoUI.SetActive(true);
                playerTurnImage.gameObject.SetActive(false);
                cursorObj.SetActive(true);
                turnImageAnim = null;
            }
        }
    }

    /// <summary>
    /// ユニット選択前
    /// </summary>
    void MyStandbyPhase()
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
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().ShowUnitInfo(Main.GameManager.GetMapUnitInfo(cursorPos));
            else
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().CloseUnitInfo();
        }

        // クリック処理
        if (Input.GetMouseButtonDown(0))
        {
            // 未行動の自軍ユニットであればフォーカスし、アクティブエリアを表示する
            if (Main.GameManager.GetMapUnitInfo(cursorPos) != null && activeAreaList == null)
                if (Main.GameManager.GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ALLY)
                    if (!Main.GameManager.GetMapUnitInfo(cursorPos).isMoving())
                    {
                        // フォーカスユニットの取得
                        focusUnitObj = Main.GameManager.GetMapUnitObj(cursorPos);
                        AddActiveArea();
                    }
        }
    }

    /// <summary>
    /// ユニット選択後
    /// </summary>
    void MyFoucusPhase()
    {
        // 移動マーカの更新
        moveMarkerManager.AddMarker(ref phaseManager, ref routeManager, ref focusUnitObj, ref moveRoot, ref activeAreaList, ref cursorPos);

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
                    moveMarkerManager.SetActive(false);
                    cursorObj.SetActive(false);
                    activeArea.SetActive(false);
                }
            }
            else // アクティブエリア外をクリックされたらフォーカスを外す
            {
                // アニメーションを元に戻す
                focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);
                focusUnitObj = null;

                // ターンとUI切り替え
                phase = Enums.PHASE.SELECT;
                moveMarkerManager.RemoveMarker();
                RemoveActiveArea();
            }
    }

    /// <summary>
    /// ユニットの移動
    /// </summary>
    void MyMovePhase()
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
    void MyBattleStandbyPhase()
    {
        // 攻撃範囲の描画
        if (attackAreaList == null)
            AddAttackArea();
        else
        {
            // カーソルを敵ユニットに合わせた時の処理
            if (Main.GameManager.GetMapUnitInfo(cursorPos) &&
                Main.GameManager.GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY &&
                attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK)
            {

                // バトルパラメータのセット
                myUnitObj = focusUnitObj;
                enemyUnitObj = Main.GameManager.GetMapUnitObj(cursorPos);
                textMyHP = battleStandbyUI.GetComponent<BattleStandby>().textMyHP;
                textEnemyHP = battleStandbyUI.GetComponent<BattleStandby>().textEnemyHP;

                myAttackPower = 12;
                myAccuracy = 100;
                myDeathblow = 10;
                myAttackCount = 2;

                enemyAttackPower = 9;
                enemyAccuracy = 60;
                enemyDeathblow = 3;
                enemyAttackCount = 1;

                // 敵の向きに合わせてUnitのアニメーション変更
                if (Mathf.Abs(focusUnitObj.transform.position.x - enemyUnitObj.transform.position.x) <= 0f)
                    if (focusUnitObj.transform.position.y < enemyUnitObj.transform.position.y)
                        focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.UP);
                    else
                        focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);
                else if (focusUnitObj.transform.position.x < enemyUnitObj.transform.position.x)
                    focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.RIGHT);
                else
                    focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.LEFT);

                // UIの切り替え
                if (!battleStandbyUI.activeSelf)
                    battleStandbyUI.SetActive(true);
                battleStandbyUI.GetComponent<BattleStandby>().SetMyUnitData(
                    focusUnitObj.GetComponent<UnitInfo>(), myAttackPower, myAccuracy, myDeathblow);
                battleStandbyUI.GetComponent<BattleStandby>().SetEnemyUnitData(
                    enemyUnitObj.GetComponent<UnitInfo>(), enemyAttackPower, enemyAccuracy, enemyDeathblow);
            }
            else
                if (battleStandbyUI.activeSelf)
                battleStandbyUI.SetActive(false); // UIの切り替え

            // クリック処理
            if (Input.GetMouseButtonDown(0))
            {
                // アクティブエリア（攻撃可能マス）で攻撃対象を選択する
                if (attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK &&
                    Main.GameManager.GetMapUnitInfo(cursorPos) &&
                        Main.GameManager.GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY)
                {
                    // 移動完了
                    Main.GameManager.MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);
                    focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み

                    // 戦闘開始
                    // ターンとUIの切り替え
                    cursorObj.SetActive(false);
                    attackArea.SetActive(false);
                    phase = Enums.PHASE.BATTLE;
                }
                else OnCancelBattleStandby(); // UIの切り替え
            }
        }
    }

    /// <summary>
    /// ユニットとの戦闘
    /// </summary>
    void MyBattlePhase()
    {
        if (!isBattle)
        {
            while (true)
            {
                // こちらの攻撃
                if (0 < myAttackCount)
                {
                    // 必殺の検証
                    myAttackState = RandomCheck(myDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (myAttackState != Enums.BATTLE.DEATH_BLOW)
                        myAttackState = RandomCheck(myAccuracy) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;
                    attackEvent.myUnitObj = myUnitObj;
                    attackEvent.enemyUnitObj = enemyUnitObj;
                    attackEvent.myAttackPower = myAttackPower;
                    attackEvent.myAttackState = myAttackState;
                    attackEvent.textEnemyHP = textEnemyHP;
                    battleManager.AddEvent(attackEvent);

                    myAttackCount--;
                }

                // 敵の反撃
                if (0 < enemyAttackCount)
                {
                    // 必殺の検証
                    enemyAttackState = RandomCheck(enemyDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (enemyAttackState != Enums.BATTLE.DEATH_BLOW)
                        enemyAttackState = RandomCheck(enemyAccuracy) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;
                    attackEvent.myUnitObj = enemyUnitObj;
                    attackEvent.enemyUnitObj = myUnitObj;
                    attackEvent.myAttackPower = enemyAttackPower;
                    attackEvent.myAttackState = enemyAttackState;
                    attackEvent.textEnemyHP = textMyHP;
                    battleManager.AddEvent(attackEvent);

                    enemyAttackCount--;
                }

                // 戦闘イベント登録終了
                if (myAttackCount <= 0 && enemyAttackCount <= 0) break;
            }

            // 敵をこちらに向かせる
            if (Mathf.Abs(enemyUnitObj.transform.position.x - focusUnitObj.transform.position.x) <= 0f)
                if (enemyUnitObj.transform.position.y < focusUnitObj.transform.position.y)
                    enemyUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.UP);
                else
                    enemyUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);
            else if (enemyUnitObj.transform.position.x < focusUnitObj.transform.position.x)
                enemyUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.RIGHT);
            else
                enemyUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.LEFT);

            // バトルの実行
            battleManager.StartEvent();
            isBattle = true;
        }
        else
        {
            // 全てのイベントが終了したらフェーズを変える
            if (!battleManager.isBattle())
            {
                // 敵の向きを元に戻す
                if (enemyUnitObj)
                    enemyUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);

                isBattle = false;
                // ターンとUIの切り替え
                phase = Enums.PHASE.RESULT; // 攻撃終了
            }
        }
    }

    /// <summary>
    /// ユニット同士の戦闘終了後
    /// </summary>
    public void MyResultPhase()
    {
        // アニメーションを元に戻す
        if (focusUnitObj)
            focusUnitObj.GetComponent<MoveController>().playAnim(Enums.MOVE.DOWN);

        // 未移動であれば移動済みとする
        if (!focusUnitObj.GetComponent<UnitInfo>().isMoving())
        {
            Main.GameManager.MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);
            focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み
        }
        // グレースケールにする
        focusUnitObj.GetComponent<EffectController>().GrayScale(true);

        RemoveActiveArea();
        RemoveAttackArea();
        moveMarkerManager.RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        List<GameObject> list = Main.GameManager.GetUnBehaviorUnit(Enums.ARMY.ALLY);
        if (list.Count == 0)
            phase = Enums.PHASE.END;
        else
            phase = Enums.PHASE.SELECT;

        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// ターン終了時
    /// </summary>
    void MyEndPhase()
    {
        // 自軍ユニットを全て未行動に戻す
        Main.GameManager.UnBehaviorUnitAll(Enums.ARMY.ALLY);
        // 敵ターンに切り替える
        TurnChange(Enums.ARMY.ENEMY);
        phase = Enums.PHASE.START;
    }

    void EnemyStartPhase()
    {
        if (turnImageAnim == null)
        {
            turnImageAnim = enemyTurnImage.gameObject.GetComponent<Animator>();
            enemyTurnImage.gameObject.SetActive(true);
        }
        else
        {
            // アニメーションが終了したらターンを開始する
            if (!(turnImageAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                // ターンとUI切り替え
                phase = Enums.PHASE.SELECT;
                selectUnitInfoUI.SetActive(false);
                cellInfoUI.SetActive(false);
                enemyTurnImage.gameObject.SetActive(false);
                cursorObj.SetActive(false);
                turnImageAnim = null;
            }
        }
    }

    void EnemyStandbyPhase()
    {
    }

    void EnemyFoucusPhase()
    {
    }

    void EnemyMovePhase()
    {
    }

    void EnemyBattleStandbyPhase()
    {
    }

    void EnemyBattlePhase()
    {
    }

    void EnemyResultPhase()
    {
    }

    void EnemyEndPhase()
    {
    }


}
