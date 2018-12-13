using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : BattleFunc
{

    GameObject playerUnitObj, enemyUnitObj;
    int damage;
    bool deathBlow;
    int enemyResidualHP;

    float ATTACK_SPEED = 0.3f;

    Animation animation;
    AnimationClip clip;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="playerUnitObj">Player unit object.</param>
    /// <param name="enemyUnitObj">Enemy unit object.</param>
    /// <param name="damage">Damage.</param>
    /// <param name="deathBlow">If set to <c>true</c> death blow.</param>
    public AttackEvent(ref GameObject playerUnitObj, ref GameObject enemyUnitObj, int damage, bool deathBlow)
    {
        this.playerUnitObj = playerUnitObj;
        this.enemyUnitObj = enemyUnitObj;
        this.damage = damage;
        this.deathBlow = deathBlow;

        // ダメージを与える
        enemyResidualHP = enemyUnitObj.GetComponent<UnitInfo>().hp - damage;

        Vector3 pos = playerUnitObj.transform.position;

        // アニメーションカーブ（曲線）の宣言
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();

        // アニメーションのキーフレームの設定(時間,値)
        Keyframe[] keysX = new Keyframe[3];
        Keyframe[] keysY = new Keyframe[3];
        keysX[0] = new Keyframe(ATTACK_SPEED * 0, pos.x);
        keysY[0] = new Keyframe(ATTACK_SPEED * 0, pos.y);
        keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x + 0.5f);
        keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y);
        keysX[2] = new Keyframe(ATTACK_SPEED * 2, pos.x);
        keysY[2] = new Keyframe(ATTACK_SPEED * 2, pos.y);
        curveX = new AnimationCurve(keysX);
        curveY = new AnimationCurve(keysY);

        // アニメーションクリップの設定
        clip = new AnimationClip();
        clip.name = "moveAnim"; // アニメーションの名前
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.legacy = true;

        // 作成したアニメーションのアタッチ
        animation = playerUnitObj.GetComponent<Animation>();
        animation.AddClip(clip, clip.name); // アタッチ
        animation.Play(clip.name); // 再生
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    protected override bool Event()
    {


        // アニメーションの終了検知
        if (!animation.IsPlaying(clip.name))
            return false;
        else
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
