using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ExpGaugeController : MonoBehaviour
{
    public GameObject expText;
    public GameObject ExpGauge;
    public GameObject ExpGaugeBg;

    private LineRenderer lineRenderer;

    const int ADD_VALUE_RATE = 10; // 一度の更新で加算する割合
    const float GAUGE_UPDATE_SPAWN = 0.04f; // 更新速度
    float spawn = 0;

    bool isUpdateStart; // 更新処理の開始フラグ
    bool isActive; // UIの表示/非表示フラグ
    int addExp = 0; // 一度に加算する経験値
    int expCalc = 0; // 計算用
    int nextExp = 0; // 次のレベルまでのEXP
    int gaugeRange = 0; // expゲージの長さ
    float gaugePosX, gaugePosY; // ゲージ位置
    UnitInfo unitInfo;
    Action callBackEvent; // 経験値取得処理後に行う処理

    void Start()
    {
        lineRenderer = ExpGauge.GetComponent<LineRenderer>();

        // expゲージの長さを取得
        gaugeRange = (int)(lineRenderer.GetPosition(1).x - lineRenderer.GetPosition(0).x);
        gaugePosX = gaugeRange / 2;
        gaugePosY = lineRenderer.GetPosition(0).y;
        isActive = false;
        isUpdateStart = false;

        // UI非表示
        ChangeActive(false);
    }

    void Update()
    {
        if (isUpdateStart)
        {
            if (GAUGE_UPDATE_SPAWN < (spawn += Time.deltaTime))
            {
                // expCalc(getExp)が0になるまで経験値に加算し続ける
                if (0 < expCalc)
                {
                    // 基本はADD_VALUE_RATEを加算し、else はあまりを加算
                    if (0 < addExp)
                    {
                        // 1割ずつ加算する
                        unitInfo.exp += addExp;
                        expCalc -= addExp;
                    }
                    else
                    {
                        // 余を加算する
                        unitInfo.exp += expCalc;
                        expCalc = 0;
                    }

                    // レベルアップ
                    if (nextExp <= unitInfo.exp)
                    {
                        // レベル加算と次のレベルまでの経験値を更新
                        unitInfo.level++;
                        nextExp = GameManager.GetCommonCalc().GetExpMax(unitInfo.level);

                        // 余分な経験値を戻す
                        expCalc += unitInfo.exp -= nextExp;

                        // 経験値をリセット
                        unitInfo.exp = 0;

                        // レベルアップイベント
                        LevelUpEvent();
                    }

                    // ゲージの更新
                    lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.exp / nextExp) * gaugeRange - gaugePosX, gaugePosY, 0));

                    if (!isActive) ChangeActive(true); // UIを表示していなければ、表示する
                }
                else
                {
                    // ゲージ更新終了
                    isUpdateStart = false;

                    // ゲージ更新後1秒間待つ
                    StartCoroutine(DelayMethod(0.4f, () =>
                    {
                        ChangeActive(false);
                        callBackEvent();
                    }));
                }
                spawn = 0;
            }
        }
    }

    /// <summary>
    /// 経験値取時のゲージ更新処理
    /// </summary>
    /// <param name="getExp">Exp.</param>
    /// <param name="unitInfo">Level.</param>
    public void GaugeUpdate(int getExp, UnitInfo unitInfo, Action callBackEvent)
    {
        this.unitInfo = unitInfo;
        this.nextExp = GameManager.GetCommonCalc().GetExpMax(unitInfo.level);
        this.addExp = getExp / ADD_VALUE_RATE;
        this.expCalc = getExp;
        this.callBackEvent = callBackEvent;
        isUpdateStart = true;
    }

    /// <summary>
    /// Expゲージの表示/非表示の切り替え
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    private void ChangeActive(bool value)
    {
        // フラグの切り替え
        isActive = value;

        expText.SetActive(value);
        ExpGauge.SetActive(value);
        ExpGaugeBg.SetActive(value);
    }

    /// <summary>
    /// レベルアップイベント
    /// </summary>
    public void LevelUpEvent()
    {
        Debug.Log("レベルアップ(てーててててー）");
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
