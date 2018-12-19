﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveController : MonoBehaviour
{
    // ゲーム設定値
    const float MOVE_SPEED = 7F; // 移動速度
    const float NEXT_MOVE_ERROR = 0.1F; // 移動の誤差補完値

    // 移動ルート管理用の2次元配列
    List<Vector3> moveRoot = new List<Vector3>(); // 自動行動用、移動ルート
    Vector3 movePos, nextPos; // 各移動状態管理用変数
    Animator animator;

    bool moveFlg = false; // 単体での移動中かどうか
    public bool movingFlg = false; // 全体での移動中かどうか

    void Start()
    {
        // 初期化
        movePos = transform.position;
        animator = GetComponent<Animator>();

        // GameManagerにユニット情報を登録する
        Main.GameManager.AddMapUnitObj(movePos, gameObject);
    }

    private void Update()
    {
        if (!moveFlg & 0 < moveRoot.Count)
        {
            // ルートデータがあるなら移動する

            // リストの先頭を取得&削除
            nextPos = moveRoot[0];
            moveRoot.RemoveAt(0);

            // アニメーションの切り替え
            if (nextPos == Vector3.up) playAnim(Enums.MOVE.UP);
            else if (nextPos == Vector3.down) playAnim(Enums.MOVE.DOWN);
            else if (nextPos == Vector3.left) playAnim(Enums.MOVE.LEFT);
            else if (nextPos == Vector3.right) playAnim(Enums.MOVE.RIGHT);

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
            if ((Mathf.Abs(movePos.x + nextPos.x - transform.position.x) < NEXT_MOVE_ERROR) &
                (Mathf.Abs(movePos.y + nextPos.y - transform.position.y) < NEXT_MOVE_ERROR))
            {
                // 移動開始位置の更新と移動ラグの補完
                transform.position = movePos += nextPos;

                // 1マスの移動完了
                moveFlg = false;

                // 移動全体の完了
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
        transform.position = movePos = newPos;
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
    /// アニメーションの再生
    /// </summary>
    /// <param name="move">Move.</param>
    public void playAnim(Enums.MOVE move)
    {
        animator.SetInteger("Walk", (int)move);
    }
}