using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : BattleFunc
{
    const float ATTACK_SPEED = 0.3f; // 攻撃アニメーションの速度
    const float ATTACK_MOVE = 0.4f; // 攻撃する時の移動距離

    GameObject myUnitObj, enemyUnitObj;
    Text textEnemyHP;
    int damage; // 与えるダメージ
    bool deathblowFlg; // 必殺発生フラグ
    bool accuracyFlg; // 攻撃命中フラグ
    int enemyHP;
    int enemyResidualHP; // ダーメージを受けた後の残りHP

    Animation animation;
    AnimationClip clip;

    // 各小イベントの実行中かどうか
    bool[] runninge = new bool[] {
        true, // ライフの減算
        true // 攻撃アニメーション
    };

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="myUnitObj">My unit object.</param>
    /// <param name="enemyUnitObj">Enemy unit object.</param>
    /// <param name="textEnemyHP">Text enemy hp.</param>
    /// <param name="damage">Damage.</param>
    /// <param name="deathblowFlg">If set to <c>true</c> deathblow flg.</param>
    /// <param name="accuracyFlg">If set to <c>true</c> accuracy flg.</param>
    public AttackEvent(ref GameObject myUnitObj, ref GameObject enemyUnitObj, Text textEnemyHP, int damage, bool deathblowFlg, bool accuracyFlg)
    {
        this.myUnitObj = myUnitObj;
        this.enemyUnitObj = enemyUnitObj;
        this.damage = damage;
        this.deathblowFlg = deathblowFlg;
        this.accuracyFlg = accuracyFlg;
        this.textEnemyHP = textEnemyHP;

        // 攻撃アニメーションの設定
        Vector3 pos = myUnitObj.transform.position;

        // アニメーションカーブ（曲線）の宣言
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();

        // アニメーションのキーフレームの設定(時間,値)
        Keyframe[] keysX = new Keyframe[3];
        Keyframe[] keysY = new Keyframe[3];
        keysX[0] = new Keyframe(ATTACK_SPEED * 0, pos.x);
        keysY[0] = new Keyframe(ATTACK_SPEED * 0, pos.y);

        if (Mathf.Abs(pos.x - enemyUnitObj.transform.position.x) <= 0f) keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x);
        else if (pos.x < enemyUnitObj.transform.position.x) keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x + ATTACK_MOVE);
        else keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x - ATTACK_MOVE);

        if (Mathf.Abs(pos.y - enemyUnitObj.transform.position.y) <= 0f) keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y);
        else if (pos.y < enemyUnitObj.transform.position.y) keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y + ATTACK_MOVE);
        else keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y - ATTACK_MOVE);

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
        animation = myUnitObj.GetComponent<Animation>();
        animation.AddClip(clip, clip.name); // アタッチ
    }

    protected override void Start()
    {
        // ダメージ減算処理
        enemyHP = enemyUnitObj.GetComponent<UnitInfo>().hp; // 現在のHP
        if (deathblowFlg)
            enemyResidualHP = enemyHP - damage * 3; // 必殺発動
        else if (accuracyFlg)
            enemyResidualHP = enemyHP - damage;  // 通常攻撃命中
        else
            enemyResidualHP = enemyHP; // 攻撃失敗

        // HPは0未満にしない
        enemyResidualHP = enemyResidualHP < 0 ? 0 : enemyResidualHP;

        // アニンメーションの再生
        animation.Play(clip.name);
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    protected override bool Update()
    {
        // ライフの減算
        if (enemyHP > enemyResidualHP)
        {
            // 実行中
            enemyHP--;
            textEnemyHP.text = enemyHP.ToString();
        }
        else
            runninge[0] = false;

        // アニメーションの終了検知
        if (!animation.IsPlaying(clip.name))
        {
            // 実行終了
            Main.GameManager.GetMapUnitInfo(enemyUnitObj.transform.position).hp = enemyResidualHP;
            runninge[1] = false;
        }

        // 全ての小イベントが終了（false)になったらfalseを返す
        return !runninge.All(value => value == false);
    }
}
