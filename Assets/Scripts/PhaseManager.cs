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
    public GameObject activeMenu;
    public GameObject battleStandby;
    public GameObject selectUnitInfo;
    public GameObject cellInfo;

    // インスタンス
    public CursorManager cursorManager;
    public BattleManager battleManager;


    // 行動ターン
    Enum.PHASE phase = Enum.PHASE.START;

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
        activeMenu.SetActive(false);
        battleStandby.SetActive(false);
        selectUnitInfo.SetActive(false);
        cellInfo.SetActive(false);
    }

    public void Update()
    {
        switch (phase)
        {
            case Enum.PHASE.START:
                StartPhase();
                break;

            case Enum.PHASE.SELECT:
                StandbyPhase();
                break;

            case Enum.PHASE.FOCUS:
                FoucusPhase();
                break;

            case Enum.PHASE.MOVE:
                MovePhase();
                break;

            case Enum.PHASE.BATTLE_STANDBY:
                BattleStandbyPhase();
                break;

            case Enum.PHASE.BATTLE:
                BattlePhase();
                break;

            case Enum.PHASE.RESULT:
                ResultPhase();
                break;

            case Enum.PHASE.END:
                EndPhase();
                break;
        }
    }

    /// <summary>
    /// 外部変更用
    /// </summary>
    /// <param name="newPhase">New phase.</param>
    public void ChangePhase(Enum.PHASE newPhase)
    {
        phase = newPhase;
    }

    /// <summary>
    /// ターン開始時
    /// </summary>
    void StartPhase()
    {
        // ターンとUI切り替え
        phase = Enum.PHASE.SELECT;
        selectUnitInfo.SetActive(true);
        cellInfo.SetActive(true);
    }

    /// <summary>
    /// ユニット選択前
    /// </summary>
    void StandbyPhase()
    {
        // カーソルの更新
        cursorManager.CursorUpdate(false);

        // クリック処理
        if (Input.GetMouseButtonDown(0))
            if (GameManager.GetMapUnit(cursorManager.cursorPos) != null && cursorManager.activeAreaList == null)
                if (!GameManager.GetMapUnit(cursorManager.cursorPos).isMoving)
                    cursorManager.AddActiveArea(); // 未行動のユニットであればフォーカスする
    }

    /// <summary>
    /// ユニット選択後
    /// </summary>
    void FoucusPhase()
    {
        // カーソルの更新
        cursorManager.CursorUpdate(true);

        if (Input.GetMouseButtonDown(0))
            // アクティブエリア（移動可能マス）を選択されたら移動する
            if (cursorManager.activeAreaList[-(int)cursorManager.cursorPos.y, (int)cursorManager.cursorPos.x].aREA == Enum.AREA.MOVE)
            {
                // 他ユニットがいなければ
                if (!GameManager.GetMapUnit(cursorManager.cursorPos))
                {
                    // ユニットの移動前の座標を保存
                    cursorManager.oldFocusUnitPos = cursorManager.focusUnit.moveController.getPos();

                    // 移動可能エリアがクリックされたら移動する
                    cursorManager.focusUnit.moveController.setMoveRoots(cursorManager.moveRoot);

                    // ターンとUI切り替え
                    phase = Enum.PHASE.MOVE;
                    cursorManager.rootArea.SetActive(false);
                    cursorManager.cursorObj.SetActive(false);
                    cursorManager.activeArea.SetActive(false);
                }
            }
            else // アクティブエリア外をクリックされたらフォーカスを外す
            {
                // アニメーションを元に戻す
                cursorManager.focusUnit.moveController.NotFocuse();
                cursorManager.focusUnit = null;

                // ターンとUI切り替え
                phase = Enum.PHASE.SELECT;
                cursorManager.RemoveMarker();
                cursorManager.RemoveActiveArea();
            }
    }

    /// <summary>
    /// ユニットの移動
    /// </summary>
    void MovePhase()
    {
        // 移動が終わったらUIを切り替える
        if (!cursorManager.focusUnit.moveController.movingFlg)
        {
            activeMenu.SetActive(true);
        }
    }

    /// <summary>
    /// 戦闘するユニットの選択
    /// </summary>
    void BattleStandbyPhase()
    {
        // カーソルの更新
        cursorManager.CursorUpdate(false);

        // 攻撃範囲の描画
        if (cursorManager.attackAreaList == null)
        {
            cursorManager.AddAttackArea();
        }
        else
        {
            // カーソルを敵ユニットに合わせた時の処理
            if (GameManager.GetMapUnit(cursorManager.cursorPos) &&
                GameManager.GetMapUnit(cursorManager.cursorPos).aRMY == Enum.ARMY.ENEMY)
            {
                if (cursorManager.attackAreaList[-(int)cursorManager.cursorPos.y, (int)cursorManager.cursorPos.x].aREA == Enum.AREA.ATTACK)
                {
                    // UIの切り替え
                    battleStandby.SetActive(true);
                }
            }
            else
            {
                // UIの切り替え
                battleStandby.SetActive(false);
            }

            // クリック処理
            if (Input.GetMouseButtonDown(0))
            {
                // アクティブエリア（攻撃可能マス）で攻撃対象を選択する
                if (cursorManager.attackAreaList[-(int)cursorManager.cursorPos.y, (int)cursorManager.cursorPos.x].aREA == Enum.AREA.ATTACK)
                {
                    // 敵プレイヤーをタップしたら
                    if (GameManager.GetMapUnit(cursorManager.cursorPos) &&
                        GameManager.GetMapUnit(cursorManager.cursorPos).aRMY == Enum.ARMY.ENEMY)
                    {
                        // 戦闘開始
                        // ターンとUIの切り替え
                        cursorManager.cursorObj.SetActive(false);
                        battleStandby.SetActive(false);
                        cursorManager.attackArea.SetActive(false);
                        phase = Enum.PHASE.BATTLE;
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
            UnitInfo enemyUnit = GameManager.GetMapUnit(cursorManager.cursorPos);

            // イベントの発生チェックと登録

            // 必殺の検証
            bool deathblowFlg = RandomCheck(playerDeathblow);
            if (deathblowFlg || RandomCheck(playerAvoidance))
            {
                // 通常攻撃か必殺が発生したら攻撃イベントとして登録する
                battleManager.AddEvent(new AttackEvent(cursorManager.focusUnit, enemyUnit, playerAttackPower, deathblowFlg));
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
                phase = Enum.PHASE.RESULT; // 攻撃終了
            }
        }
    }

    /// <summary>
    /// ユニット同士の戦闘終了後
    /// </summary>
    void ResultPhase()
    {
        //if 
        phase = Enum.PHASE.SELECT;
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
        phase = Enum.PHASE.BATTLE_STANDBY;
        cursorManager.activeArea.SetActive(false);
        activeMenu.SetActive(false);
        cursorManager.rootArea.SetActive(false);
        cursorManager.cursorObj.SetActive(true);
    }

    /// <summary>
    /// 行動画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelActiveMenu()
    {
        // アニメーションを元に戻す
        if (cursorManager.focusUnit) cursorManager.focusUnit.moveController.NotFocuse();

        // ユニットの座標を元に戻す
        cursorManager.focusUnit.moveController.DirectMove(cursorManager.oldFocusUnitPos);

        cursorManager.RemoveActiveArea();
        cursorManager.RemoveMarker();
        cursorManager.focusUnit = null;

        // ターンとUIの切り替え
        phase = Enum.PHASE.SELECT;
        activeMenu.SetActive(false);
        cursorManager.cursorObj.SetActive(true);
    }

    /// <summary>
    /// 攻撃選択画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelBattleStandby()
    {
        cursorManager.RemoveAttackArea();

        // ターンとUIの切り替え
        phase = Enum.PHASE.MOVE;
        battleStandby.SetActive(false);
        cursorManager.cursorObj.SetActive(false);
        cursorManager.rootArea.SetActive(true);
        cursorManager.activeArea.SetActive(true);
    }

    /// <summary>
    /// 行動終了ボタン処理
    /// </summary>
    public void OnEndBtn()
    {
        // アニメーションを元に戻す
        if (cursorManager.focusUnit) cursorManager.focusUnit.moveController.NotFocuse();

        // ユニット管理リストの更新
        GameManager.MoveMapUnitData(cursorManager.oldFocusUnitPos, cursorManager.focusUnit.moveController.getPos());

        cursorManager.RemoveActiveArea();
        cursorManager.RemoveMarker();
        cursorManager.focusUnit = null;

        // ターンとUIの切り替え
        phase = Enum.PHASE.SELECT;
        activeMenu.SetActive(false);
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
}
