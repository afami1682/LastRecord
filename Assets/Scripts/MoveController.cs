using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveController : MonoBehaviour
{
    // ゲーム設定値
    const float MOVE_SPEED = 8F; // 移動速度
    const float NEXT_MOVE_ERROR = 0.1F; // 移動の誤差補完値
    enum ANIM { NONE, UP, DOWN, LEFT, RIGHT } // アニメーションID

    // 移動ルート管理用の2次元配列
    private List<Vector3> moveRoot = new List<Vector3>(); // 自動行動用、移動ルート
    private Vector3 pos, movePos, nextPos; // 各移動状態管理用変数
    private bool moveFlg = false; // 単体の移動中かどうか
    public bool movingFlg = false; // 移動中かどうか
    private Animator animator;

    void Start()
    {
        // 初期化
        pos = transform.position;
        animator = GetComponent<Animator>();

        // GameManagerにユニット情報を登録する
        UnitInfo unitInfo = GetComponent<UnitInfo>();
        unitInfo.moveController = GetComponent<MoveController>();
        GameManager.AddMapUnitData(pos, unitInfo);
    }

    private void Update()
    {
        // ルートデータがあるなら移動する
        if (!moveFlg & 0 < moveRoot.Count)
        {
            // リストの先頭を取得&削除
            nextPos = moveRoot[0];
            moveRoot.RemoveAt(0);

            // アニメーションの切り替え
            if (nextPos == Vector3.up) animator.SetInteger("Walk", (int)ANIM.UP);
            else if (nextPos == Vector3.down) animator.SetInteger("Walk", (int)ANIM.DOWN);
            else if (nextPos == Vector3.left) animator.SetInteger("Walk", (int)ANIM.LEFT);
            else if (nextPos == Vector3.right) animator.SetInteger("Walk", (int)ANIM.RIGHT);

            // 移動開始
            moveFlg = true;
            movingFlg = true;
        }
    }

    void FixedUpdate()
    {
        // 移動処理
        if (moveFlg)
        {
            // 移動
            transform.position += nextPos * Time.fixedDeltaTime * MOVE_SPEED;

            // 目標地点までの誤差がNEXT_MOVE_ERROR未満なら移動完了とする（移動ラグ対策）
            if ((Mathf.Abs(pos.x + nextPos.x - transform.position.x) < NEXT_MOVE_ERROR) &
                (Mathf.Abs(pos.y + nextPos.y - transform.position.y) < NEXT_MOVE_ERROR))
            {
                // 移動ラグの補完
                transform.position = pos + nextPos;

                // 移動開始位置の更新
                pos = transform.position;

                // 移動終了
                moveFlg = false;

                if (moveRoot.Count == 0)
                    movingFlg = false;
            }
        }
    }

    /// <summary>
    ///  移動ルートの追加
    /// </summary>
    /// <param name="pos">Position.</param>
    public void AddMoveRoots(Vector3 pos)
    {
        moveRoot.Insert(moveRoot.Count, pos);
    }

    /// <summary>
    /// Directs the move.
    /// </summary>
    /// <param name="newPos">Position.</param>
    public void DirectMove(Vector3 newPos)
    {
        pos = newPos;
        transform.position = newPos;
    }

    /// <summary>
    /// 移動ルートの指定
    /// </summary>
    /// <param name="moveRoots">Move roots.</param>
    public void setMoveRoots(List<Vector3> moveRoots)
    {
        moveRoot = moveRoots;
    }

    /// <summary>
    /// 座標取得用
    /// </summary>
    /// <returns>The position.</returns>
    public Vector3 getPos()
    {
        return transform.position;
    }
}