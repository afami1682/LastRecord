using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KegController : GimmickBaseController
{
    const int explosionArea = 2;
    const int damage = 10;


    override protected void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// ターゲットに指定された時のイベント
    /// </summary>
    public override void SelectStart()
    {
        base.SelectStart();
        // ユニットの攻撃範囲を非表示にする



        // 爆発範囲を表示する



    }

    /// <summary>
    /// ターゲットが外された時のイベント
    /// </summary>
    public override void SelectEnd()
    {
        base.SelectEnd();
        // ユニットの攻撃範囲を表示する


        // 爆発範囲を削除する




    }

    /// <summary>
    /// 攻撃された時のイベント
    /// </summary>
    public override void GimmickEvent()
    {
        base.GimmickEvent();
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