using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カーソルの描画,アクティブエリアの描画,フォーカスユニットの制御
/// </summary>
public class CursorManager : MonoBehaviour
{
    // カーソル描画関連
    public GameObject cursorObj; // カーソルObj
    private Vector2 mouseScreenPos;
    private Vector3 _cursorPos, cursorPos;

    // アクティブUI
    public GameObject activeMenu;
    // バトルスタンバイUI
    public GameObject battleStandby;

    // フォーカスUnit関連
    [HideInInspector]
    public UnitInfo focusUnit;
    private Vector3 oldFocusUnitPos;
    [HideInInspector]
    public List<Vector3> moveRoot; // 移動ルートの座標引き渡し用
    [HideInInspector]
    public Struct.NodeMove[,] activeAreaList; // 行動可能エリアを管理する配列
    public Struct.NodeMove[,] attackAreaList;

    // エリア描画用関連
    private GameObject activeArea;
    private GameObject attackArea;
    private GameObject rootArea;
    public GameObject areaBlue;
    public GameObject areaRed;
    public GameObject markerObj;
    public Sprite[] makerSprites;

    // インスタンス
    private RouteManager routeManager;
    public UIUnitInfo uIUnitInfo;
    public UICellInfo uIcellInfo;
    private CursorManager cursorManager;

    // 行動ターン
    Enum.PHASE phase = Enum.PHASE.START;

    void Start()
    {
        // UIの非表示
        activeMenu.SetActive(false);
        battleStandby.SetActive(false);

        // カーソルの生成
        cursorObj = Instantiate(cursorObj, Vector3.zero, Quaternion.identity);

        // エリア描画用関連の読み込み
        attackArea = new GameObject("AttackArea");
        activeArea = new GameObject("ActiveArea");
        rootArea = new GameObject("rootArea");
        attackArea.transform.parent = transform;
        activeArea.transform.parent = transform;
        rootArea.transform.parent = transform;

        // インスタンスの初期化
        routeManager = new RouteManager();
        cursorManager = GetComponent<CursorManager>();
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
                BattlePhase(true);
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
    /// ターン開始時
    /// </summary>
    private void StartPhase()
    {
        phase = Enum.PHASE.SELECT;
        Debug.Log("phase.SELECT");
    }

    /// <summary>
    /// ユニット選択前
    /// </summary>
    private void StandbyPhase()
    {
        // カーソルの更新
        CursorUpdate(false);

        // クリック処理
        if (Input.GetMouseButtonDown(0))
            if (GameManager.GetMapUnit(cursorPos) != null && activeAreaList == null)
                if (!GameManager.GetMapUnit(cursorPos).isMoving)
                    AddActiveArea(); // 未行動のユニットであればフォーカスする
    }

    /// <summary>
    /// ユニット選択後
    /// </summary>
    private void FoucusPhase()
    {
        // カーソルの更新
        CursorUpdate(true);

        if (Input.GetMouseButtonDown(0))
            // アクティブエリア（移動可能マス）を選択されたら移動する
            if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enum.AREA.MOVE)
            {
                // 他ユニットがいなければ
                if (!GameManager.GetMapUnit(cursorPos))
                {
                    // ユニットの移動前の座標を保存
                    oldFocusUnitPos = focusUnit.moveController.getPos();

                    // 移動可能エリアがクリックされたら移動する
                    focusUnit.moveController.setMoveRoots(moveRoot);

                    // ターンとUI切り替え
                    Debug.Log("phase.MOVE");
                    phase = Enum.PHASE.MOVE;
                    rootArea.SetActive(false);
                    cursorObj.SetActive(false);
                    activeArea.SetActive(false);
                }
            }
            else // アクティブエリア外をクリックされたらフォーカスを外す
            {
                // アニメーションを元に戻す
                focusUnit.moveController.NotFocuse();
                focusUnit = null;

                // ターンとUI切り替え
                Debug.Log("phase.SELECT");
                phase = Enum.PHASE.SELECT;
                RemoveMarker();
                RemoveActiveArea();
            }
    }

    /// <summary>
    /// ユニットの移動
    /// </summary>
    private void MovePhase()
    {
        // 移動が終わったらUIを切り替える
        if (!focusUnit.moveController.movingFlg)
        {
            activeMenu.SetActive(true);
        }
    }

    /// <summary>
    /// 戦闘するユニットの選択
    /// </summary>
    private void BattleStandbyPhase()
    {
        // カーソルの更新
        CursorUpdate(false);

        // 攻撃範囲の描画
        if (attackAreaList == null)
        {
            AddAttackArea();
        }
        else
        {
            // カーソルを敵ユニットに合わせた時の処理
            if (GameManager.GetMapUnit(cursorPos) &&
                GameManager.GetMapUnit(cursorPos).aRMY == Enum.ARMY.ENEMY)
            {
                if (attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enum.AREA.ATTACK)
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
                if (attackAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enum.AREA.ATTACK)
                {
                    // 敵プレイヤーをタップしたら
                    if (GameManager.GetMapUnit(cursorPos) &&
                        GameManager.GetMapUnit(cursorPos).aRMY == Enum.ARMY.ENEMY)
                    {
                        // 戦闘開始
                        // ターンとUIの切り替え
                        cursorObj.SetActive(false);
                        battleStandby.SetActive(false);
                        attackArea.SetActive(false);
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
    private void BattlePhase(bool playerPhase)
    {
        // プレイヤーターンのバトル処理
        if (playerPhase)
        {






        }
    }

    /// <summary>
    /// ユニット同士の戦闘終了後
    /// </summary>
    private void ResultPhase()
    {

    }

    /// <summary>
    /// ターン終了時
    /// </summary>
    private void EndPhase()
    {

    }

    /// <summary>
    /// Ons the attack button.
    /// </summary>
    public void OnAttackBtn()
    {
        // ターンとUI切り替え
        Debug.Log("phase.BATTLE_STANDBY");
        phase = Enum.PHASE.BATTLE_STANDBY;
        activeArea.SetActive(false);
        activeMenu.SetActive(false);
        rootArea.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// 行動画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelActiveMenu()
    {
        // アニメーションを元に戻す
        if (focusUnit) focusUnit.moveController.NotFocuse();

        // ユニットの座標を元に戻す
        focusUnit.moveController.DirectMove(oldFocusUnitPos);

        RemoveActiveArea();
        RemoveMarker();
        focusUnit = null;

        // ターンとUIの切り替え
        Debug.Log("phase.SELECT");
        phase = Enum.PHASE.SELECT;
        activeMenu.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// 攻撃選択画面からのキャンセルボタン処理（画面外のクリック）
    /// </summary>
    public void OnCancelBattleStandby()
    {
        RemoveAttackArea();

        // ターンとUIの切り替え
        Debug.Log("phase.MOVE");
        phase = Enum.PHASE.MOVE;
        battleStandby.SetActive(false);
        cursorObj.SetActive(false);
        rootArea.SetActive(true);
        activeArea.SetActive(true);
    }

    /// <summary>
    /// 行動終了ボタン処理
    /// </summary>
    public void OnEndBtn()
    {
        // アニメーションを元に戻す
        if (focusUnit) focusUnit.moveController.NotFocuse();

        // ユニット管理リストの更新
        GameManager.MoveMapUnitData(oldFocusUnitPos, focusUnit.moveController.getPos());

        RemoveActiveArea();
        RemoveMarker();
        focusUnit = null;

        // ターンとUIの切り替え
        Debug.Log("phase.SELECT");
        phase = Enum.PHASE.SELECT;
        activeMenu.SetActive(false);
        cursorObj.SetActive(true);
    }

    /// <summary>
    /// カーソルの描画処理
    /// </summary>
    private void CursorUpdate(bool showMarker)
    {
        // マウスの座標を変換して取得
        mouseScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _cursorPos = new Vector3(MultipleRound(mouseScreenPos.x, 1),
                                MultipleRound(mouseScreenPos.y, 1), 0);

        // マップ内なら新しいカーソル座標を取得する
        if (0 <= _cursorPos.x && _cursorPos.x < MapManager.GetFieldData().width &&
            0 <= -_cursorPos.y && -_cursorPos.y < MapManager.GetFieldData().height)
            cursorPos = _cursorPos;

        // カーソル座標が更新されてないなら更新する
        if (cursorObj.transform.position != cursorPos && !activeMenu.activeSelf)
        {
            // カーソルの座標を更新
            cursorObj.transform.position = cursorPos;

            // セル情報の更新
            uIcellInfo.SetData(MapManager.GetFieldData().cells[-(int)cursorPos.y, (int)cursorPos.x]);

            // 移動マーカの更新
            if (showMarker) AddMarker();

            // ユニット情報の更新

            if (GameManager.GetMapUnit(cursorPos))
            {
                if (phase == Enum.PHASE.SELECT)
                    uIUnitInfo.ShowUnitInfo(GameManager.GetMapUnit(cursorPos));
            }
            else
            {
                uIUnitInfo.CloseUnitInfo();
            }
        }
    }

    /// <summary>
    /// アクティブエリアの表示
    /// </summary>
    private void AddActiveArea()
    {
        // フォーカスユニットの取得
        focusUnit = GameManager.GetMapUnit(cursorPos);

        // アクティブリストの生成と検証
        activeAreaList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];

        // 移動エリアの検証
        routeManager.CheckMoveArea(ref cursorManager);

        // エリアパネルの表示
        for (int y = 0; y < MapManager.GetFieldData().height; y++)
            for (int x = 0; x < MapManager.GetFieldData().width; x++)
                if (activeAreaList[y, x].aREA == Enum.AREA.MOVE || activeAreaList[y, x].aREA == Enum.AREA.UNIT)
                {
                    // 移動エリアの表示
                    Instantiate(areaBlue, new Vector3(x, -y, 0), Quaternion.identity).transform.parent = activeArea.transform;

                    // 攻撃エリアの検証
                    routeManager.CheckAttackArea(ref activeAreaList, new Vector3(x, -y, 0), ref focusUnit);
                }

        // 攻撃エリアの表示
        for (int ay = 0; ay < MapManager.GetFieldData().height; ay++)
            for (int ax = 0; ax < MapManager.GetFieldData().width; ax++)
                if (activeAreaList[ay, ax].aREA == Enum.AREA.ATTACK)
                    Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = activeArea.transform;

        // ターンとUIの切り替え
        Debug.Log("phase.FOCUS");
        phase = Enum.PHASE.FOCUS;
        activeArea.SetActive(true);
        rootArea.SetActive(true);
    }


    /// <summary>
    /// 攻撃エリアの表示
    /// </summary>
    private void AddAttackArea()
    {
        // アクティブリストの生成と検証
        attackAreaList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];

        // 攻撃エリアの検証と表示
        routeManager.CheckAttackArea(ref attackAreaList, focusUnit.moveController.getPos(), ref focusUnit);
        for (int ay = 0; ay < MapManager.GetFieldData().height; ay++)
            for (int ax = 0; ax < MapManager.GetFieldData().width; ax++)
                if (attackAreaList[ay, ax].aREA == Enum.AREA.ATTACK)
                    Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = attackArea.transform;
    }

    /// <summary>
    /// markerの表示
    /// </summary>
    private void AddMarker()
    {
        // アクティブエリアがあるなら、マーカを表示する
        if (activeAreaList != null)
            // 移動エリア内ならマーカを表示する
            if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enum.AREA.MOVE)
            {
                // マーカの削除
                RemoveMarker();

                // 目標までのルートを取得
                routeManager.CheckShortestRoute(ref cursorManager, cursorPos);

                // マーカの生成とスプライト変更
                Vector3 nextPos = focusUnit.moveController.getPos();
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
                            spriteId = 3;
                            angle = Quaternion.identity;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.up)
                            {
                                if (moveRoot[i + 1] == Vector3.left)
                                    angle.eulerAngles = new Vector3(0, 180, 0);
                                else
                                    angle = Quaternion.identity;
                                spriteId = 2;
                            }
                            else
                            {
                                spriteId = 1;
                                angle = Quaternion.identity;
                            }
                        }
                    }
                    else if (moveRoot[i] == Vector3.down)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            spriteId = 3;
                            angle.eulerAngles = new Vector3(0, 0, 180);
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.down)
                            {
                                if (moveRoot[i + 1] == Vector3.left)
                                    angle.eulerAngles = new Vector3(0, 0, 180);
                                else
                                    angle.eulerAngles = new Vector3(180, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                spriteId = 1;
                                angle.eulerAngles = new Vector3(0, 0, 180);
                            }
                        }

                    }
                    else if (moveRoot[i] == Vector3.right)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            spriteId = 3;
                            angle.eulerAngles = new Vector3(0, 0, -90);
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.right)
                            {
                                if (moveRoot[i + 1] == Vector3.up)
                                    angle.eulerAngles = new Vector3(0, 180, 90);
                                else
                                    angle.eulerAngles = new Vector3(0, 0, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                spriteId = 1;
                                angle.eulerAngles = new Vector3(0, 0, -90);
                            }
                        }
                    }
                    else
                    {
                        if (i + 1 == moveRootCount)
                        {
                            spriteId = 3;
                            angle.eulerAngles = new Vector3(0, 0, 90);
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.left)
                            {
                                if (moveRoot[i + 1] == Vector3.up)
                                    angle.eulerAngles = new Vector3(0, 0, 90);
                                else
                                    angle.eulerAngles = new Vector3(0, 180, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                spriteId = 1;
                                angle.eulerAngles = new Vector3(0, 0, 90);
                            }
                        }
                    }
                    markerObj.GetComponent<SpriteRenderer>().sprite = makerSprites[spriteId];
                    Instantiate(markerObj, nextPos += moveRoot[i], angle).transform.parent = rootArea.transform;
                }
            }
            else if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enum.AREA.UNIT)
                RemoveMarker(); // カーソルがユニット上なら表示しない
    }

    /// <summary>
    /// 行動エリアの初期化と削除
    /// </summary>
    private void RemoveActiveArea()
    {
        activeAreaList = null;
        foreach (Transform a in activeArea.transform) Destroy(a.gameObject);
    }

    /// <summary>
    /// 攻撃エリアの初期化と削除
    /// </summary>
    private void RemoveAttackArea()
    {
        attackAreaList = null;
        foreach (Transform a in attackArea.transform) Destroy(a.gameObject);
    }

    /// <summary>
    /// マーカの削除
    /// </summary>
    private void RemoveMarker()
    {
        foreach (Transform r in rootArea.transform) Destroy(r.gameObject);
    }

    /// <summary>
    /// 倍数での四捨五入のような値を求める（ｎおきの数の中間の値で切り捨て・切り上げをする）
    ///（例）倍数 = 10 のとき、12 → 10, 17 → 20
    /// </summary>
    /// <param name="value">入力値</param>
    /// <param name="multiple">倍数</param>
    /// <return>倍数の中間の値で、切り捨て・切り上げした値</return>
    private static float MultipleRound(float value, float multiple)
    {
        return MultipleFloor(value + multiple * 0.5f, multiple);
    }

    /// <summary>
    /// より小さい倍数を求める（倍数で切り捨てられるような値）
    ///（例）倍数 = 10 のとき、12 → 10, 17 → 10
    /// </summary>
    /// <param name="value">入力値</param>
    /// <param name="multiple">倍数</param>
    /// <return>倍数で切り捨てた値</return>
    private static float MultipleFloor(float value, float multiple)
    {
        return Mathf.Floor(value / multiple) * multiple;
    }
}