using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : BattleFunc
{
    const float ATTACK_SPEED = 0.3f; // 攻撃アニメーションの速度
    const float ATTACK_MOVE = 0.4f; // 攻撃する時の移動距離

    PhaseManager parent;
    GameObject myUnitObj, enemyUnitObj; // Unitのオブジェクト
    int myAttackPower, enemyAttackPower; // 攻撃力
    Enums.BATTLE myAttackState, enemyAttackState; // 攻撃判定
    Text textMyHP, textEnemyHP; // 表示用

    int enemyHP;
    int enemyResidualHP; // ダーメージを受けた後の残りHP

    float span = ATTACK_SPEED / 2; // ダメージ処理タイミング
    float delta = 0; // 計測時間

    Animation animation;
    AnimationClip clip;

    // 各小イベントの実行中かどうか
    bool[] runninge = new bool[] {
        true, // ダメージ処理
        true, // ライフの減算
        true // 攻撃アニメーション
    };

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="parent">Parent.</param>
    /// <param name="myUnitObj">My unit object.</param>
    /// <param name="enemyUnitObj">Enemy unit object.</param>
    /// <param name="myAttackPower">My attack power.</param>
    /// <param name="myAttackState">My attack state.</param>
    /// <param name="textEnemyHP">Text enemy hp.</param>
    public AttackEvent(PhaseManager parent, ref GameObject myUnitObj, ref GameObject enemyUnitObj, Text textEnemyHP, int myAttackPower, Enums.BATTLE myAttackState)
    {
        this.parent = parent;
        this.myUnitObj = myUnitObj;
        this.enemyUnitObj = enemyUnitObj;
        this.myAttackPower = myAttackPower;
        this.myAttackState = myAttackState;
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

    /// <summary>
    /// Start this instance.
    /// </summary>
    /// <returns>イベントを開始できるかどうか</returns>
    protected override bool Start()
    {
        // 自軍体力が0以下なら終了
        if (myUnitObj.GetComponent<UnitInfo>().hp < 1) { return false; }

        // ダメージ減算処理
        enemyHP = enemyUnitObj.GetComponent<UnitInfo>().hp; // 現在のHP

        // 敵体力が0以下なら終了
        if (enemyHP < 1) { return false; }

        switch (myAttackState)
        {
            case Enums.BATTLE.NORMAL:
                enemyResidualHP = enemyHP - myAttackPower;  // 通常攻撃命中
                break;

            case Enums.BATTLE.DEATH_BLOW:
                enemyResidualHP = enemyHP - myAttackPower * 3; // 必殺発動
                break;

            case Enums.BATTLE.MISS:
                enemyResidualHP = enemyHP; // 攻撃失敗
                break;
        }

        // HPは0未満にしない
        enemyResidualHP = enemyResidualHP < 0 ? 0 : enemyResidualHP;

        // アニンメーションの再生
        animation.Play(clip.name);

        return true;
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    protected override bool Update()
    {
        // 攻撃アニメーションの半分の時間に、ダメージ処理を行う
        if (span < delta && runninge[0])
        {
            switch (myAttackState)
            {
                case Enums.BATTLE.NORMAL:
                case Enums.BATTLE.DEATH_BLOW:
                    // ダメージの反映
                    enemyUnitObj.GetComponent<UnitInfo>().hp = enemyResidualHP;
                    Main.GameManager.GetMapUnitInfo(enemyUnitObj.transform.position).hp = enemyResidualHP;
                    break;

                case Enums.BATTLE.MISS:
                    GameObject missObj = Resources.Load<GameObject>("Prefabs/Miss");
                    parent.CreateObject(missObj, enemyUnitObj.transform.position, Quaternion.identity);
                    break;
            }

            // ダメージ判定の終了
            runninge[0] = false;
        }

        // ライフの減算（徐々に減らしていく）
        if (enemyHP > enemyResidualHP)
        {
            enemyHP--;
            textEnemyHP.text = enemyHP.ToString();
        }
        else
            runninge[1] = false; // HPの減算終了

        // アニメーションの終了検知
        if (!animation.IsPlaying(clip.name))
            runninge[2] = false; // アニメーションの終了

        delta += Time.deltaTime;

        // 全ての小イベントが終了（false)になったらfalseを返す
        return !runninge.All(value => value == false);
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
