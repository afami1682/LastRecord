using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ExpGaugeController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject levelUpUI;

    const int GAUGE_RANGE_MAX = 200;
    const int ADD_VALUE_RATE = 2; // 一度の更新で加算する割合(%)
    const float GAUGE_UPDATE_SPAWN = 0.02f; // 更新速度
    float spawn;

    int addExp; // 一度に加算する経験値
    int expCalc; // 計算用
    int nextExp; // 次のレベルまでのEXP
    float gaugePosX, gaugePosY; // ゲージ位置
    int levelUpCount; // レベルアップした数
    UnitInfo unitInfo;
    Action callBackEvent; // 経験値取得処理後に行う処理

    void Update()
    {
        //if (GAUGE_UPDATE_SPAWN < (spawn += Time.deltaTime))
        //{
        // expCalc(getExp)が0になるまで経験値に加算し続ける
        if (0 < expCalc)
        {
            // 基本はADD_VALUE_RATEを加算し、else はあまりを加算
            if (0 < addExp)
            {
                // 1割ずつ加算する
                unitInfo.AddExp(addExp);
                expCalc -= addExp;
            }
            else
            {
                // 余を加算する
                unitInfo.AddExp(expCalc);
                expCalc = 0;
            }

            // レベルアップ
            if (nextExp <= unitInfo.Exp)
            {
                // 余分な経験値を戻す
                expCalc += unitInfo.Exp - nextExp;

                // Expのリセット
                unitInfo.ResetExp();

                // レベルアップした回数を記録
                levelUpCount++;

                // 次のレベルまでの経験値を更新
                nextExp = GameManager.GetCommonCalc().GetExpMax(unitInfo.Level + levelUpCount);
            }

            // ゲージの更新
            lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.Exp / nextExp) * GAUGE_RANGE_MAX - gaugePosX, gaugePosY, 0));
        }
        else
        {
            // ゲージ更新後1秒間待つ
            StartCoroutine(DelayMethod(0.5f, () =>
            {
                gameObject.SetActive(false);
                if (0 < levelUpCount)
                {
                        // レベルアップイベント
                        levelUpUI.GetComponent<LevelUpController>().LevelUpEvent(unitInfo, levelUpCount, callBackEvent);
                }
                else
                {
                        // コールバックイベントの実行
                        callBackEvent();
                }
            }));
        }
        spawn = 0;
        // }
    }

    /// <summary>
    /// 経験値取時のゲージ更新処理
    /// </summary>
    /// <param name="getExp">Exp.</param>
    /// <param name="unitInfo">Level.</param>
    public void GaugeUpdate(int getExp, UnitInfo unitInfo, Action callBackEvent)
    {
        this.unitInfo = unitInfo;
        this.callBackEvent = callBackEvent;
        nextExp = GameManager.GetCommonCalc().GetExpMax(unitInfo.Level);
        addExp = (int)(getExp / 100) * ADD_VALUE_RATE;
        expCalc = getExp;
        levelUpCount = 0;
        spawn = 0;

        // expゲージの表示位置を取得
        gaugePosX = GAUGE_RANGE_MAX / 2;
        gaugePosY = lineRenderer.GetPosition(0).y;

        // ゲージの初期値の設定
        lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.Exp / nextExp) * GAUGE_RANGE_MAX - gaugePosX, gaugePosY, 0));

        // ゲージの表示
        gameObject.SetActive(true);
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
