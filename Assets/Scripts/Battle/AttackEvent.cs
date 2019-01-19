using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : MonoBehaviour
{
    const float ATTACK_SPEED = 0.3f; // 攻撃アニメーションの速度
    const float ATTACK_MOVE = 0.4f; // 攻撃する時の移動距離

    int targetHP; // 敵HP
    int targetResidualHP; // ダーメージ処理後の敵HP

    Animation anim;
    AnimationClip clip;

    // 引き継ぎパラメータ
    public PhaseManager phaseManager;
    public GameObject myUnitObj, targetUnitObj; // Unitのオブジェクト
    public int myAttackPower, targetAttackPower; // 攻撃力
    public int myDeathblow, targetDeathblow; // 必殺の発生率
    public int myAttackCount, targetAttackCount;// 攻撃回数
    public int myAccuracy, targetAccuracy;// 命中率
    public Enums.BATTLE myAttackState, targetAttackState; // 攻撃成功判定
    public Text myHPText, targetHPText; // 表示用

    // 各イベントの実行フラグ
    bool[] runninge =  {
         true, // ダメージ処理
         true, // ダメージ処理
         true  // 攻撃アニメーション
    };

    /// <summary>
    /// Start this instance.
    /// </summary>
    /// <returns>イベントを開始できるかどうか</returns>
    void Start()
    {
        // ダメージ減算処理
        targetHP = targetUnitObj.GetComponent<UnitInfo>().hp; // 現在のHP

        // 自軍か敵Unitの体力が0以下なら終了
        if (myUnitObj.GetComponent<UnitInfo>().hp <= 0 || targetHP <= 0)
        {
            Destroy(this);
            return;
        }

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

        if (Mathf.Abs(pos.x - targetUnitObj.transform.position.x) <= 0f) keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x);
        else if (pos.x < targetUnitObj.transform.position.x) keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x + ATTACK_MOVE);
        else keysX[1] = new Keyframe(ATTACK_SPEED * 1, pos.x - ATTACK_MOVE);

        if (Mathf.Abs(pos.y - targetUnitObj.transform.position.y) <= 0f) keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y);
        else if (pos.y < targetUnitObj.transform.position.y) keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y + ATTACK_MOVE);
        else keysY[1] = new Keyframe(ATTACK_SPEED * 1, pos.y - ATTACK_MOVE);

        keysX[2] = new Keyframe(ATTACK_SPEED * 2, pos.x);
        keysY[2] = new Keyframe(ATTACK_SPEED * 2, pos.y);
        curveX = new AnimationCurve(keysX);
        curveY = new AnimationCurve(keysY);

        // アニメーションクリップの設定
        clip = new AnimationClip
        {
            name = "moveAnim" // アニメーションの名前
        };
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.legacy = true;

        // 作成したアニメーションのアタッチ
        anim = myUnitObj.GetComponent<Animation>();
        anim.AddClip(clip, clip.name);

        switch (myAttackState)
        {
            case Enums.BATTLE.NORMAL:
                targetResidualHP = targetHP - myAttackPower;  // 通常攻撃命中
                break;

            case Enums.BATTLE.DEATH_BLOW:
                targetResidualHP = targetHP - myAttackPower * 3; // 必殺発動
                break;

            case Enums.BATTLE.MISS:
                targetResidualHP = targetHP; // 攻撃失敗
                break;
        }

        // HPは0未満にしない
        targetResidualHP = targetResidualHP < 0 ? 0 : targetResidualHP;

        // アニンメーションの再生
        anim.Play(clip.name);

        // 攻撃アニメーションの途中でダメージ処理
        StartCoroutine(DelayMethod(ATTACK_SPEED / 2, () =>
        {
            switch (myAttackState)
            {
                case Enums.BATTLE.NORMAL:
                case Enums.BATTLE.DEATH_BLOW:
                    if (myAttackPower != 0)
                    {
                        // ダメージの反映
                        (targetUnitObj).GetComponent<UnitInfo>().hp = targetResidualHP;
                        GameManager.GetUnit().GetMapUnitInfo(targetUnitObj.transform.position).hp = targetResidualHP;
                    }
                    else
                    {
                        // NO DAMAGEのエフェクトを生成する
                        GameObject noDamageObj = Resources.Load<GameObject>("Prefabs/NoDamage");
                        Instantiate(noDamageObj, targetUnitObj.transform.position, Quaternion.identity);
                    }
                    break;

                case Enums.BATTLE.MISS:
                    GameObject missObj = Resources.Load<GameObject>("Prefabs/Miss");
                    Instantiate(missObj, targetUnitObj.transform.position, Quaternion.identity);
                    break;
            }
            // ダメージ判定の終了
            runninge[0] = false;
        }));
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    void Update()
    {
        // ライフの減算（徐々に減らしていく）
        if (runninge[1])
            if (targetHP > targetResidualHP)
            {
                targetHP--;
                targetHPText.text = targetHP.ToString();
            }
            else
            {
                // HPが0になった場合は、消滅イベントを追加
                if (targetHP <= 0)
                {
                    UnitLoseEvent unitLoseEvent = targetUnitObj.AddComponent<UnitLoseEvent>();
                    unitLoseEvent.phaseManager = phaseManager;
                    phaseManager.battleManager.AddEvent(unitLoseEvent);
                }
                runninge[1] = false; // HPの減算終了
            }

        // アニメーションの終了検知
        if (runninge[2])
            if (!anim.IsPlaying(clip.name))
                runninge[2] = false; // アニメーションの終了

        // 全ての小イベントが終了（false)になったら自身を削除する
        if (runninge.All(value => value == false))
            Destroy(this);
    }

    /// <summary>
    /// コンポーネント削除時に呼ばれる
    /// </summary>
    private void OnDestroy()
    {
        // 次のバトルイベントを実行する
        phaseManager.battleManager.NextEvent();
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
