using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveController : MonoBehaviour
{
    // ゲーム設定値
    const float MOVE_SPEED = 8F; // 移動速度
    const float NEXT_MOVE_ERROR = 0.1F; // 移動の誤差補完値

    // 移動ルート管理用の2次元配列
    List<Vector3> moveRoot = new List<Vector3>(); // 自動行動用、移動ルート
    Vector3 pos, movePos, nextPos, nextAttackPos; // 各移動状態管理用変数
    Animator animator;
    bool isFocuse = false; // フォーカスされてるかどうか

    bool moveFlg = false; // 単体での移動中かどうか
    public bool movingFlg = false; // 全体での移動中かどうか
    bool attackAnimFlg = false; // 攻撃アニメーションフラグ

    void Start()
    {
        // 初期化
        pos = transform.position;
        animator = GetComponent<Animator>();

        // GameManagerにユニット情報を登録する
        GameManager.AddMapUnitData(pos, gameObject);
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
            if (nextPos == Vector3.up) playAnim(Enum.MOVE.UP);
            else if (nextPos == Vector3.down) playAnim(Enum.MOVE.DOWN);
            else if (nextPos == Vector3.left) playAnim(Enum.MOVE.LEFT);
            else if (nextPos == Vector3.right) playAnim(Enum.MOVE.RIGHT);

            // 移動開始
            moveFlg = true;
            movingFlg = true;
        }
    }

    void FixedUpdate()
    {
        // アニメーション管理
        if (attackAnimFlg)
        {
            // 攻撃移動処理
            // 移動
            transform.position += nextAttackPos * Time.fixedDeltaTime;

            if ((Mathf.Abs(pos.x + nextAttackPos.x - transform.position.x) < NEXT_MOVE_ERROR) &
                (Mathf.Abs(pos.y + nextAttackPos.y - transform.position.y) < NEXT_MOVE_ERROR))
            {
                nextAttackPos *= -1; // 移動向きの反転
            }
        }
        else if (moveFlg)
        {
            // 移動処理

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

                // 移動が全て終わったら
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

    /// <summary>
    /// アニメーションの再生
    /// </summary>
    /// <param name="move">Move.</param>
    private void playAnim(Enum.MOVE move)
    {
        animator.SetInteger("Walk", (int)move);
    }

    /// <summary>
    /// フォーカスされたら呼び出す処理
    /// </summary>
    public void Focused()
    {
        isFocuse = true;
    }

    /// <summary>
    /// フォーカスが外れたら呼び出す処理
    /// </summary>
    public void FocuseEnd()
    {
        isFocuse = false;

        // アニメーションを元に戻す
        playAnim(Enum.MOVE.DOWN);
    }

    /// <summary>
    /// 外部からの呼び出し用
    /// </summary>
    public void AttackAnim()
    {
        nextAttackPos = new Vector3(0, 0.5f, 0);
        attackAnimFlg = true;
    }
}