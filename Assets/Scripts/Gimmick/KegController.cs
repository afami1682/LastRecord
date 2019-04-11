using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KegController : MonoBehaviour
{
    const int explosionArea = 2;
    const int damage = 10;

    void Start()
    {
        // GameManagerにユニット情報を登録する
        GameManager.GetUnit().AddMapUnitObj(transform.position, gameObject);
    }

    void Update()
    {

    }

    /// <summary>
    /// ターゲットに指定された時のイベント
    /// </summary>
    public void Targeted()
    {

    }

    /// <summary>
    /// 攻撃された時のイベント
    /// </summary>
    public void Attacked()
    {
        // ダメージ判定
        // エリアパネルの表示
        Vector2 checkPos;
        for (int y = 0; y < GameManager.GetMap().field.height; y++)
        {
            for (int x = 0; x < GameManager.GetMap().field.width; x++)
            {
                checkPos = new Vector2(x, y);
                if (Vector2.Distance(transform.position, checkPos) <= explosionArea)
                {
                    GameManager.GetUnit().GetMapUnitInfo(checkPos).Damaged(damage);
                }
            }
        }

        // GameManagerからユニット情報を削除する
        GameManager.GetUnit().AddMapUnitObj(transform.position, null);

        // 爆発アニメーションを再生し削除する
        GetComponent<AnimationController>().enabled = true;
    }
}