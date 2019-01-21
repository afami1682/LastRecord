using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

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
    public ActiveAreaManager activeAreaManager;
    PhaseManager phaseManager;

    [HideInInspector]
    public Vector3 cursorPos, oldCursorPos;

    // フォーカス（選択中）Unit関連
    [HideInInspector]
    public GameObject focusUnitObj;
    Vector3 oldFocusUnitPos;
    [HideInInspector]
    public List<Vector3> moveRoot; // 移動ルートの座標引き渡し用

    // バトル関連
    private bool isBattle;
    [HideInInspector]
    public bool playerAttack;
    [HideInInspector]
    public GameObject playerUnitObj, enemyUnitObj; // Unitのオブジェクト
    [HideInInspector]
    public int playerAttackPower, enemyAttackPower; // 攻撃力
    [HideInInspector]
    public int playerDeathblow, enemyDeathblow; // 必殺の発生率
    [HideInInspector]
    public int playerAttackCount, enemyAttackCount;// 攻撃回数
    [HideInInspector]
    public int playerHitRate, enemyHitRate;// 命中率
    [HideInInspector]
    public Enums.BATTLE playerAttackState, enemyAttackState; // 攻撃成功判定
    [HideInInspector]
    public Text playerHPText, enemyHPText; // 表示用

    // 行動ターン
    [HideInInspector]
    public Enums.PHASE phase = Enums.PHASE.START;

    // 行動するターンプレイヤー
    [HideInInspector]
    public Enums.ARMY turnPlayer;

    // 各行動イベント
    Action StartPhase, // プレイヤーのターン開始時
    StandbyPhase, // Unit選択中
    FoucusPhase, // Unit選択時
    MovePhase, // Unit行動時
    BattleStandbyPhase, // Unit攻撃選択時
    BattlePhase, // Unit攻撃時
    ResultPhase, // Unit攻撃終了時（まだ見操作のUnitがいれば、STANDBYに戻る）
    EndPhase; // プレイヤーのターン終了時

    void Start()
    {
        // インスタンスの初期化
        phaseManager = this;
        isBattle = false;

        // プレイヤーターンから始める
        TurnChange(Enums.ARMY.ALLY);

        // プレイヤーユニットを全て未行動にする
        GameManager.GetUnit().UnBehaviorUnitAll(Enums.ARMY.ALLY);

        // UIの非表示
        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        selectUnitInfoUI.SetActive(false);
        cellInfoUI.SetActive(false);
        playerTurnImage.gameObject.SetActive(false);
        enemyTurnImage.gameObject.SetActive(false);
        //cursorObj.SetActive(false); // 一番最初だけ表示

        // カーソル更新時に呼び出す処理の登録
        CursorController.AddCallBack((Vector3 newPos) => { cursorPos = newPos; });
    }

    public void Update()
    {
        switch (phase)
        {
            case Enums.PHASE.START: StartPhase(); break;
            case Enums.PHASE.STANDBY: StandbyPhase(); break;
            case Enums.PHASE.FOCUS: FoucusPhase(); break;
            case Enums.PHASE.MOVE: MovePhase(); break;
            case Enums.PHASE.BATTLE_STANDBY: BattleStandbyPhase(); break;
            case Enums.PHASE.BATTLE: BattlePhase(); break;
            case Enums.PHASE.RESULT: ResultPhase(); break;
            case Enums.PHASE.END: EndPhase(); break;
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
    /// ターンの切り替え処理
    /// </summary>
    /// <param name="player">Turn player.</param>
    void TurnChange(Enums.ARMY player)
    {
        switch (player)
        {
            case Enums.ARMY.ALLY:
                this.turnPlayer = player;
                StartPhase = PlayerStartPhase;
                StandbyPhase = PlayerStandbyPhase;
                FoucusPhase = PlayerFoucusPhase;
                MovePhase = PlayerMovePhase;
                BattleStandbyPhase = PlayerBattleStandbyPhase;
                BattlePhase = PlayerBattlePhase;
                ResultPhase = PlayerResultPhase;
                EndPhase = PlayerEndPhase;
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
    public void AttackBtn()
    {
        // ターンとUI切り替え
        phase = Enums.PHASE.BATTLE_STANDBY;
        activeAreaManager.activeAreaObj.SetActive(false);
        activeMenuUI.SetActive(false);
        moveMarkerManager.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// 行動キャンセル処理
    /// </summary>
    public void MoveCancelBtn()
    {
        // アニメーションを元に戻す
        if (focusUnitObj)
            focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);

        // ユニットの位置を元に戻す
        focusUnitObj.GetComponent<MoveController>().DirectMove(oldFocusUnitPos);

        activeAreaManager.RemoveActiveArea();
        activeAreaManager.RemoveAttackArea();
        moveMarkerManager.RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        phase = Enums.PHASE.STANDBY;
        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        cursorObj.SetActive(true);
        cellInfoUI.SetActive(true);
        activeAreaManager.activeAreaObj.SetActive(true);
    }

    /// <summary>
    /// 攻撃選択画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void BattleCancelBtn()
    {
        activeAreaManager.RemoveAttackArea();

        // ターンとUIの切り替え
        phase = Enums.PHASE.MOVE;
        battleStandbyUI.SetActive(false);
        cursorObj.SetActive(false);
        cellInfoUI.SetActive(true);
        moveMarkerManager.SetActive(true);
        activeAreaManager.activeAreaObj.SetActive(true);
    }

    /// <summary>
    /// 行動終了ボタン処理
    /// </summary>
    public void MoveEndBtn()
    {
        PlayerResultPhase();
    }

    /// <summary>
    /// カーソルの更新時に呼ばれる
    /// </summary>
    /// <param name="newPos">New position.</param>
    public void CursorUpdate(Vector3 newPos) { cursorPos = newPos; }

    /// <summary>
    /// ターン開始時
    /// </summary>
    void PlayerStartPhase()
    {
        if (turnImageAnim == null)
        {
            // ターン開始アニメーションの再生
            turnImageAnim = playerTurnImage.gameObject.GetComponent<Animator>();
            playerTurnImage.gameObject.SetActive(true);

            // ランダムな未行動ユニット1体の座標にカーソルを合わせる
            cursorObj.transform.position = GameManager.GetUnit().GetUnBehaviorRandomUnit(Enums.ARMY.ALLY).transform.position;
        }
        else
        {
            // アニメーションが終了したらターンを開始する
            if (!(turnImageAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                // ターンとUIの切り替え
                List<GameObject> list = GameManager.GetUnit().GetUnBehaviorUnits(Enums.ARMY.ALLY);
                if (list.Count == 0)
                    phase = Enums.PHASE.END;
                else
                    phase = Enums.PHASE.STANDBY;

                selectUnitInfoUI.SetActive(true);
                cellInfoUI.SetActive(true);
                playerTurnImage.gameObject.SetActive(false);
                activeAreaManager.activeAreaObj.SetActive(true);
                cursorObj.SetActive(true);
                turnImageAnim = null;
            }
        }
    }

    /// <summary>
    /// ユニット選択前
    /// </summary>
    void PlayerStandbyPhase()
    {
        // カーソルが更新されたら
        if (cursorPos != oldCursorPos)
        {
            // カーソルの更新
            oldCursorPos = cursorPos;

            // セル情報の更新
            cellInfoUI.GetComponent<CellInfo>().SetData(GameManager.GetMap().field.cells[-(int)cursorPos.y, (int)cursorPos.x]);

            // ユニット情報の更新
            if (GameManager.GetUnit().GetMapUnitInfo(cursorPos))
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().ShowUnitInfo(GameManager.GetUnit().GetMapUnitInfo(cursorPos));
            else
                selectUnitInfoUI.GetComponent<SelectUnitInfo>().CloseUnitInfo();
        }

        // クリック処理
        if (Input.GetMouseButtonDown(0))
        {
            // アクティブエリアの初期化
            activeAreaManager.RemoveActiveArea();

            // 未行動の自軍ユニットであればフォーカスし、アクティブエリアを表示する
            if (GameManager.GetUnit().GetMapUnitInfo(cursorPos) != null && activeAreaManager.activeAreaList == null)
                switch (GameManager.GetUnit().GetMapUnitInfo(cursorPos).aRMY)
                {
                    case Enums.ARMY.ALLY:
                        if (!GameManager.GetUnit().GetMapUnitInfo(cursorPos).IsMoving())
                        {
                            // フォーカスユニットの取得
                            focusUnitObj = GameManager.GetUnit().GetMapUnitObj(cursorPos);
                            activeAreaManager.CreateActiveArea(focusUnitObj, true);

                            // ターンとUIの切り替え
                            phase = Enums.PHASE.FOCUS;
                            selectUnitInfoUI.SetActive(false);
                            moveMarkerManager.SetActive(true);
                        }
                        break;
                    case Enums.ARMY.ENEMY:
                    case Enums.ARMY.NEUTRAL:
                        // フォーカスユニットの取得
                        focusUnitObj = GameManager.GetUnit().GetMapUnitObj(cursorPos);
                        activeAreaManager.CreateActiveArea(focusUnitObj, true);
                        break;
                }
        }
    }

    /// <summary>
    /// ユニット選択後
    /// </summary>
    void PlayerFoucusPhase()
    {
        // 移動マーカの更新
        if (cursorPos != oldCursorPos)
        {
            // カーソルの更新
            oldCursorPos = cursorPos;
            // セル情報の更新
            cellInfoUI.GetComponent<CellInfo>().SetData(GameManager.GetMap().field.cells[-(int)cursorPos.y, (int)cursorPos.x]);
            // 移動マーカの更新
            moveMarkerManager.AddMarker(ref phaseManager);
        }

        if (Input.GetMouseButtonDown(0))
            // アクティブエリア（移動可能マス）を選択されたら移動する
            switch (activeAreaManager.activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA)
            {
                case Enums.AREA.MOVE:
                    if (GameManager.GetUnit().GetMapUnitInfo(cursorPos)) return; // 移動先に他ユニットがいるならキャンセル
                    goto case Enums.AREA.UNIT;

                case Enums.AREA.UNIT:
                    // ユニットの移動前の座標を保存
                    oldFocusUnitPos = focusUnitObj.transform.position;

                    // 移動可能エリアがクリックされたら移動する
                    focusUnitObj.GetComponent<MoveController>().SetMoveRoots(moveRoot);

                    // ターンとUI切り替え
                    phase = Enums.PHASE.MOVE;
                    moveMarkerManager.SetActive(false);
                    cursorObj.SetActive(false);
                    cellInfoUI.SetActive(false);
                    activeAreaManager.activeAreaObj.SetActive(false);
                    break;

                default:
                    // アクティブエリア外をクリックされたらフォーカスを外す
                    // アニメーションを元に戻す
                    focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);
                    focusUnitObj = null;

                    // ターンとUI切り替え
                    phase = Enums.PHASE.STANDBY;
                    moveMarkerManager.RemoveMarker();
                    activeAreaManager.RemoveActiveArea();
                    break;
            }
    }

    /// <summary>
    /// ユニットの移動
    /// </summary>
    void PlayerMovePhase()
    {
        // 移動が終わったらUIを切り替える
        if (focusUnitObj.GetComponent<MoveController>().IsMoved())
        {
            activeAreaManager.attackAreaObj.SetActive(true);
            activeMenuUI.SetActive(true);
        }
    }

    /// <summary>
    /// 戦闘するユニットの選択
    /// </summary>
    void PlayerBattleStandbyPhase()
    {
        // 攻撃範囲の描画
        if (activeAreaManager.attackAreaList == null)
            activeAreaManager.CreateAttackArea(focusUnitObj.transform.position, focusUnitObj.GetComponent<UnitInfo>().attackRange);
        else
        {
            // カーソルを敵ユニットに合わせた時の処理
            if (GameManager.GetUnit().GetMapUnitInfo(cursorPos) &&
                GameManager.GetUnit().GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY &&
                activeAreaManager.attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK)
            {

                // バトルパラメータのセット
                playerUnitObj = focusUnitObj;
                enemyUnitObj = GameManager.GetUnit().GetMapUnitObj(cursorPos);
                playerHPText = battleStandbyUI.GetComponent<BattleStandby>().textMyHP;
                enemyHPText = battleStandbyUI.GetComponent<BattleStandby>().textEnemyHP;

                playerAttackPower = GameManager.GetCommonCalc().GetAttackDamage(focusUnitObj.GetComponent<UnitInfo>(), enemyUnitObj.GetComponent<UnitInfo>());
                playerHitRate = GameManager.GetCommonCalc().GetHitRate(focusUnitObj.GetComponent<UnitInfo>(), enemyUnitObj.GetComponent<UnitInfo>());
                playerDeathblow = GameManager.GetCommonCalc().GetDeathBlowRete(focusUnitObj.GetComponent<UnitInfo>(), enemyUnitObj.GetComponent<UnitInfo>());
                playerAttackCount = GameManager.GetCommonCalc().GetAttackCount(focusUnitObj.GetComponent<UnitInfo>(), enemyUnitObj.GetComponent<UnitInfo>()); ;

                if (GameManager.GetCommonCalc().GetCellDistance(
                    enemyUnitObj.transform.position,
                    focusUnitObj.transform.position) <= enemyUnitObj.GetComponent<UnitInfo>().attackRange
                    )
                {
                    // 反撃可能
                    enemyAttackPower = GameManager.GetCommonCalc().GetAttackDamage(enemyUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
                    enemyHitRate = GameManager.GetCommonCalc().GetHitRate(enemyUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
                    enemyDeathblow = GameManager.GetCommonCalc().GetDeathBlowRete(enemyUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
                    enemyAttackCount = GameManager.GetCommonCalc().GetAttackCount(enemyUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
                }
                else
                {
                    // 反撃不可
                    enemyAttackPower = -1;
                    enemyHitRate = -1;
                    enemyDeathblow = -1;
                    enemyAttackCount = 0;
                }

                // 敵の向きに合わせてUnitのアニメーション変更
                Vector3 distance = enemyUnitObj.transform.position - focusUnitObj.transform.position;
                if (Mathf.Abs(distance.y) <= Mathf.Abs(distance.x))
                {
                    if (enemyUnitObj.transform.position.x > focusUnitObj.transform.position.x)
                        focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.RIGHT);
                    else
                        focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.LEFT);
                }
                else
                {
                    if (enemyUnitObj.transform.position.y > focusUnitObj.transform.position.y)
                        focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.UP);
                    else
                        focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);
                }

                // UIの切り替え
                if (!battleStandbyUI.activeSelf)
                    battleStandbyUI.SetActive(true);
                battleStandbyUI.GetComponent<BattleStandby>().SetMyUnitData(
                    focusUnitObj.GetComponent<UnitInfo>(), playerAttackPower, playerHitRate, playerDeathblow);
                battleStandbyUI.GetComponent<BattleStandby>().SetEnemyUnitData(
                    enemyUnitObj.GetComponent<UnitInfo>(), enemyAttackPower, enemyHitRate, enemyDeathblow);
            }
            else
                if (battleStandbyUI.activeSelf)
                battleStandbyUI.SetActive(false); // UIの切り替え

            // クリック処理
            if (Input.GetMouseButtonDown(0))
            {
                // アクティブエリア（攻撃可能マス）で攻撃対象を選択する
                if (activeAreaManager.attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.ATTACK &&
                    GameManager.GetUnit().GetMapUnitInfo(cursorPos) &&
                        GameManager.GetUnit().GetMapUnitInfo(cursorPos).aRMY == Enums.ARMY.ENEMY)
                {
                    // 移動完了
                    GameManager.GetUnit().MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);
                    focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み

                    // 戦闘開始
                    // ターンとUIの切り替え
                    cursorObj.SetActive(false);
                    activeAreaManager.attackAreaObj.SetActive(false);
                    phase = Enums.PHASE.BATTLE;
                }
                else BattleCancelBtn(); // UIの切り替え
            }
        }
    }

    /// <summary>
    /// ユニットとの戦闘
    /// </summary>
    void PlayerBattlePhase()
    {
        if (!isBattle)
        {
            while (true)
            {
                // こちらの攻撃
                if (0 < playerAttackCount)
                {
                    // 必殺の検証
                    playerAttackState = GameManager.GetCommonCalc().ProbabilityDecision(playerDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (playerAttackState != Enums.BATTLE.DEATH_BLOW)
                        playerAttackState = GameManager.GetCommonCalc().GetHitDecision(playerHitRate) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;
                    attackEvent.myUnitObj = playerUnitObj;
                    attackEvent.myAttackPower = playerAttackPower;
                    attackEvent.myAttackState = playerAttackState;
                    attackEvent.targetUnitObj = enemyUnitObj;
                    attackEvent.targetHPText = enemyHPText;
                    battleManager.AddEvent(attackEvent);

                    playerAttackCount--;
                }

                // 敵の反撃
                if (0 < enemyAttackCount)
                {
                    // 必殺の検証
                    enemyAttackState = GameManager.GetCommonCalc().ProbabilityDecision(enemyDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (enemyAttackState != Enums.BATTLE.DEATH_BLOW)
                        enemyAttackState = GameManager.GetCommonCalc().GetHitDecision(enemyHitRate) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;
                    attackEvent.myUnitObj = enemyUnitObj;
                    attackEvent.myAttackPower = enemyAttackPower;
                    attackEvent.myAttackState = enemyAttackState;
                    attackEvent.targetUnitObj = playerUnitObj;
                    attackEvent.targetHPText = playerHPText;
                    battleManager.AddEvent(attackEvent);

                    enemyAttackCount--;
                }

                // 戦闘イベント登録終了
                if (playerAttackCount <= 0 && enemyAttackCount <= 0) break;
            }

            // 敵をこちらに向かせる
            Vector3 distance = enemyUnitObj.transform.position - focusUnitObj.transform.position;
            if (Mathf.Abs(distance.y) <= Mathf.Abs(distance.x))
            {
                if (enemyUnitObj.transform.position.x < focusUnitObj.transform.position.x)
                    enemyUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.RIGHT);
                else
                    enemyUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.LEFT);
            }
            else
            {
                if (enemyUnitObj.transform.position.y < focusUnitObj.transform.position.y)
                    enemyUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.UP);
                else
                    enemyUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);
            }

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
                    enemyUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);

                isBattle = false;
                // ターンとUIの切り替え
                phase = Enums.PHASE.RESULT; // 攻撃終了
            }
        }
    }

    /// <summary>
    /// ユニット同士の戦闘終了後
    /// </summary>
    public void PlayerResultPhase()
    {
        if (focusUnitObj)
        {
            // アニメーションを元に戻す
            focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);

            // グレースケールにする
            focusUnitObj.GetComponent<UnitEffectController>().GrayScale(true);

            // 未移動であれば移動済みとする
            if (!focusUnitObj.GetComponent<UnitInfo>().IsMoving())
            {
                GameManager.GetUnit().MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);
                focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み
            }
        }

        activeAreaManager.RemoveActiveArea();
        activeAreaManager.RemoveAttackArea();
        moveMarkerManager.RemoveMarker();
        focusUnitObj = null;

        // ターンとUIの切り替え
        List<GameObject> list = GameManager.GetUnit().GetUnBehaviorUnits(Enums.ARMY.ALLY);
        if (list.Count == 0)
            phase = Enums.PHASE.END;
        else
            phase = Enums.PHASE.STANDBY;

        activeMenuUI.SetActive(false);
        battleStandbyUI.SetActive(false);
        cursorObj.SetActive(true);
        cellInfoUI.SetActive(true);
        activeAreaManager.activeAreaObj.SetActive(true);
    }

    /// <summary>
    /// ターン終了時
    /// </summary>
    void PlayerEndPhase()
    {
        activeAreaManager.activeAreaObj.SetActive(false);

        // 自軍ユニットを全て未行動に戻す
        GameManager.GetUnit().UnBehaviorUnitAll(Enums.ARMY.ALLY);
        // 敵ターンに切り替える
        TurnChange(Enums.ARMY.ENEMY);
        phase = Enums.PHASE.START;
    }

    void EnemyStartPhase()
    {
        if (turnImageAnim == null)
        {
            // ターン開始アニメーションの再生
            turnImageAnim = enemyTurnImage.gameObject.GetComponent<Animator>();
            enemyTurnImage.gameObject.SetActive(true);
        }
        else
        {
            // アニメーションが終了したらターンを開始する
            if (!(turnImageAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                // ターンとUI切り替え
                phase = Enums.PHASE.STANDBY;
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
        // ランダムな未行動ユニット1体の取得
        GameObject checkUnitObj = GameManager.GetUnit().GetUnBehaviorRandomUnit(Enums.ARMY.ENEMY);

        if (checkUnitObj != null)
        {
            // アクティブエリアの取得
            activeAreaManager.CreateActiveArea(checkUnitObj, true);

            // 行動範囲内にて攻撃できるプレイヤーUnitを探索する
            List<GameObject> targetList = GameManager.GetEnemyAI().GetAttackTargetList(phaseManager.activeAreaManager.activeAreaList);

            playerUnitObj = GameManager.GetEnemyAI().GetAttackTargetSelection(checkUnitObj.GetComponent<UnitInfo>(), targetList);
            if (playerUnitObj != null)
            {
                // 行動範囲内に攻撃できる対象がいる場合は、移動して攻撃する
                focusUnitObj = checkUnitObj;

                phase = Enums.PHASE.FOCUS;
            }
            else if (false)
            {
                // TODO 攻撃が届かないので、一番近いプレイヤーユニットに近づく



            }
            else
            {
                // このターンは移動しない
                checkUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み
                focusUnitObj = null;
                phase = Enums.PHASE.RESULT;
            }
        }
        else phase = Enums.PHASE.END;
    }

    void EnemyFoucusPhase()
    {
        // ユニットの移動前の座標を保存
        oldFocusUnitPos = focusUnitObj.transform.position;

        // 移動できる範囲で、ターゲットに攻撃できる場所のリストを取得する
        List<Vector3> attackLocationList = GameManager.GetEnemyAI().GetAttackLocationList(phaseManager.activeAreaManager.activeAreaList, focusUnitObj, playerUnitObj);

        // 攻撃対象に対して、攻撃できる場所がなかった場合、行動終了とする
        if (attackLocationList.Count < 1)
        {
            // このターンは移動しない
            focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み
            focusUnitObj = null;
            phase = Enums.PHASE.RESULT;
            return;
        }

        // TODO とりあえず一つ目を目的地とする
        Vector3 movePos = attackLocationList[0];

        // 目標までのルートを取得し設定
        GameManager.GetRoute().CheckShortestRoute(ref phaseManager, movePos);
        focusUnitObj.GetComponent<MoveController>().SetMoveRoots(moveRoot);

        // ターンとUI切り替え
        phase = Enums.PHASE.MOVE;
    }

    void EnemyMovePhase()
    {
        // 移動が終わったらフェイズを切り替える
        if (focusUnitObj.GetComponent<MoveController>().IsMoved())
        {
            // Unitリストの座標を更新
            focusUnitObj.GetComponent<UnitInfo>().Moving(true);
            GameManager.GetUnit().MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);

            // フェイズの切り替え
            phase = Enums.PHASE.BATTLE_STANDBY;
        }
    }

    void EnemyBattleStandbyPhase()
    {
        // バトルパラメータのセット
        enemyUnitObj = focusUnitObj;
        enemyHPText = battleStandbyUI.GetComponent<BattleStandby>().textEnemyHP;
        playerHPText = battleStandbyUI.GetComponent<BattleStandby>().textMyHP;

        // 敵ユニット(攻撃する側)
        enemyAttackPower = GameManager.GetCommonCalc().GetAttackDamage(focusUnitObj.GetComponent<UnitInfo>(), playerUnitObj.GetComponent<UnitInfo>());
        enemyHitRate = GameManager.GetCommonCalc().GetHitRate(focusUnitObj.GetComponent<UnitInfo>(), playerUnitObj.GetComponent<UnitInfo>());
        enemyDeathblow = GameManager.GetCommonCalc().GetDeathBlowRete(focusUnitObj.GetComponent<UnitInfo>(), playerUnitObj.GetComponent<UnitInfo>());
        enemyAttackCount = GameManager.GetCommonCalc().GetAttackCount(focusUnitObj.GetComponent<UnitInfo>(), playerUnitObj.GetComponent<UnitInfo>());

        // プレイヤーユニット
        if (GameManager.GetCommonCalc().GetCellDistance(
            playerUnitObj.transform.position,
            focusUnitObj.transform.position) <= playerUnitObj.GetComponent<UnitInfo>().attackRange)
        {
            // 反撃可能
            playerAttackPower = GameManager.GetCommonCalc().GetAttackDamage(playerUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
            playerHitRate = GameManager.GetCommonCalc().GetHitRate(playerUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
            playerDeathblow = GameManager.GetCommonCalc().GetDeathBlowRete(playerUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
            playerAttackCount = GameManager.GetCommonCalc().GetAttackCount(playerUnitObj.GetComponent<UnitInfo>(), focusUnitObj.GetComponent<UnitInfo>());
        }
        else
        {
            // 反撃不可
            playerAttackPower = -1;
            playerHitRate = -1;
            playerDeathblow = -1;
            playerAttackCount = 0;
        }

        // 敵の向きに合わせてUnitのアニメーション変更
        Vector3 distance = playerUnitObj.transform.position - focusUnitObj.transform.position;
        if (Mathf.Abs(distance.y) <= Mathf.Abs(distance.x))
        {
            if (playerUnitObj.transform.position.x > focusUnitObj.transform.position.x)
                focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.RIGHT);
            else
                focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.LEFT);
        }
        else
        {
            if (playerUnitObj.transform.position.y > focusUnitObj.transform.position.y)
                focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.UP);
            else
                focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);
        }
        phase = Enums.PHASE.BATTLE;
    }

    void EnemyBattlePhase()
    {
        if (!isBattle)
        {
            while (true)
            {
                // こちらの攻撃
                if (0 < enemyAttackCount)
                {
                    // 必殺の検証
                    enemyAttackState = GameManager.GetCommonCalc().ProbabilityDecision(enemyDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (enemyAttackState != Enums.BATTLE.DEATH_BLOW)
                        enemyAttackState = GameManager.GetCommonCalc().GetHitDecision(enemyHitRate) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;

                    attackEvent.myUnitObj = enemyUnitObj;
                    attackEvent.myAttackPower = enemyAttackPower;
                    attackEvent.myAttackState = enemyAttackState;

                    attackEvent.targetUnitObj = playerUnitObj;
                    attackEvent.targetHPText = playerHPText;

                    battleManager.AddEvent(attackEvent);

                    enemyAttackCount--;
                }

                // プレイヤーUnitの反撃
                if (0 < playerAttackCount)
                {
                    // 必殺の検証
                    playerAttackState = GameManager.GetCommonCalc().ProbabilityDecision(playerDeathblow) ? Enums.BATTLE.DEATH_BLOW : Enums.BATTLE.NORMAL;

                    // 通常攻撃命中判定
                    if (playerAttackState != Enums.BATTLE.DEATH_BLOW)
                        playerAttackState = GameManager.GetCommonCalc().GetHitDecision(playerHitRate) ? Enums.BATTLE.NORMAL : Enums.BATTLE.MISS;

                    // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                    AttackEvent attackEvent = gameObject.AddComponent<AttackEvent>();
                    attackEvent.phaseManager = this;

                    attackEvent.myUnitObj = playerUnitObj;
                    attackEvent.myAttackPower = playerAttackPower;
                    attackEvent.myAttackState = playerAttackState;

                    attackEvent.targetUnitObj = enemyUnitObj;
                    attackEvent.targetHPText = enemyHPText;

                    battleManager.AddEvent(attackEvent);

                    playerAttackCount--;
                }

                // 戦闘イベント登録終了
                if (playerAttackCount <= 0 && enemyAttackCount <= 0) break;
            }

            // 敵をこちらに向かせる
            Vector3 distance = playerUnitObj.transform.position - focusUnitObj.transform.position;
            if (Mathf.Abs(distance.y) <= Mathf.Abs(distance.x))
            {
                if (playerUnitObj.transform.position.x < focusUnitObj.transform.position.x)
                    playerUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.RIGHT);
                else
                    playerUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.LEFT);
            }
            else
            {
                if (playerUnitObj.transform.position.y < focusUnitObj.transform.position.y)
                    playerUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.UP);
                else
                    playerUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);
            }

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
                if (playerUnitObj) playerUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);

                isBattle = false;
                // ターンとUIの切り替え
                phase = Enums.PHASE.RESULT; // 攻撃終了
            }
        }
    }

    void EnemyResultPhase()
    {
        if (focusUnitObj)
        {
            // アニメーションを元に戻す
            focusUnitObj.GetComponent<MoveController>().PlayAnim(Enums.MOVE.DOWN);

            // グレースケールにする
            focusUnitObj.GetComponent<UnitEffectController>().GrayScale(true);

            // 未移動であれば移動済みとする
            if (!focusUnitObj.GetComponent<UnitInfo>().IsMoving())
            {
                GameManager.GetUnit().MoveMapUnitObj(oldFocusUnitPos, focusUnitObj.transform.position);
                focusUnitObj.GetComponent<UnitInfo>().Moving(true); // 行動済み
            }
        }

        activeAreaManager.RemoveActiveArea();
        activeAreaManager.RemoveAttackArea();
        focusUnitObj = null;

        // ターンとUIの切り替え
        List<GameObject> list = GameManager.GetUnit().GetUnBehaviorUnits(Enums.ARMY.ENEMY);
        if (list.Count == 0) phase = Enums.PHASE.END;
        else
            phase = Enums.PHASE.STANDBY;
    }

    void EnemyEndPhase()
    {
        // 自軍ユニットを全て未行動に戻す
        GameManager.GetUnit().UnBehaviorUnitAll(Enums.ARMY.ENEMY);
        // プレイヤーターンに切り替える
        TurnChange(Enums.ARMY.ALLY);
        phase = Enums.PHASE.START;
    }
}
