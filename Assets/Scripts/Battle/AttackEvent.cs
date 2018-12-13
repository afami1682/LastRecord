using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : BattleFunc
{

    GameObject playerObj;
    GameObject enemyObj;
    int damage;
    bool deathBlow;
    int enemyResidualHP; // 敵の残りHP（終了条件）

    float ATTACK_SPEED = 0.3f;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="playerObj">Player object.</param>
    /// <param name="enemyObj">Enemy object.</param>
    /// <param name="damage">Damage.</param>
    /// <param name="deathBlow">If set to <c>true</c> death blow.</param>
    public AttackEvent(ref GameObject playerObj, ref GameObject enemyObj, int damage, bool deathBlow)
    {
        this.playerObj = playerObj;
        this.enemyObj = enemyObj;
        this.damage = damage;
        this.deathBlow = deathBlow;

        // 終了条件の指定
        enemyResidualHP = enemyObj.GetComponent<UnitInfo>().hp - damage;
        Debug.Log(enemyResidualHP);

        // アニメーションのキーフレームの設定(時間,値)
        Keyframe[] keysX = new Keyframe[3];
        keysX[0] = new Keyframe(ATTACK_SPEED * 0, playerObj.GetComponent<MoveController>().getPos().x);
        keysX[1] = new Keyframe(ATTACK_SPEED * 1, playerObj.GetComponent<MoveController>().getPos().x + 0.5f);
        keysX[2] = new Keyframe(ATTACK_SPEED * 2, playerObj.GetComponent<MoveController>().getPos().x);

        Keyframe[] keysY = new Keyframe[3];
        keysY[0] = new Keyframe(ATTACK_SPEED * 0, playerObj.GetComponent<MoveController>().getPos().y);
        keysY[1] = new Keyframe(ATTACK_SPEED * 1, playerObj.GetComponent<MoveController>().getPos().y);
        keysY[2] = new Keyframe(ATTACK_SPEED * 2, playerObj.GetComponent<MoveController>().getPos().y);

        // アニメーションカーブ（曲線）の宣言
        AnimationCurve curveX = new AnimationCurve();
        curveX = new AnimationCurve(keysX);

        AnimationCurve curveY = new AnimationCurve();
        curveY = new AnimationCurve(keysY);

        // アニメーションクリップの宣言
        AnimationClip clip = new AnimationClip();
        clip.name = "moveAnim"; // アニメーションの名前
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.legacy = true;

        // 作成したアニメーションのアタッチ
        Animation animation = playerObj.GetComponent<Animation>();
        animation.AddClip(clip, clip.name); // アタッチ
        animation.Play(clip.name); // 再生

        // アニメーションの終了検知
        if (!animation.IsPlaying(clip.name))
        {

        }
        else
        {

        }
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    protected override bool Event()
    {


        // 実行中
        return true;

        /// Debug.Log("再生おわり");
        //GameManager.GetMapUnitInfo(enemyObj.GetComponent<MoveController>().getPos()).hp = enemyResidualHP;
        //return false;

        //// 与えたダメージ分まで減算
        //if (enemyResidualHP < enemyUnit.hp)
        //{
        //    enemyUnit.hp--;
        //    Debug.Log(enemyUnit.hp);
        //}
        //else
        //{
        // Uniyt管理配列の更新
        // 終了
        // }
        // return true; // 続行
    }
}
