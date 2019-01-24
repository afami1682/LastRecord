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

    const int ADD_VALUE_RATE = 10;
    const float GAUGE_UPDATE_SPAWN = 0.04f;
    float spawn = 0;

    bool isUpdateStart; // 更新処理の開始フラグ
    bool isActive; // UIの表示/非表示フラグ
    int nowExp = 0; // 更新中のexp値
    int getExp = 0; // 取得したexp
    int nextExp = 0; // そのレベルでの最大exp
    int gaugeRange = 0; // expゲージの長さ
    float gaugePosX, gaugePosY; // ゲージ位置
    UnitInfo unitInfo;
    Action callBackEvent;

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
                if (0 < nowExp)
                {
                    if (0 < getExp / ADD_VALUE_RATE && getExp / ADD_VALUE_RATE <= nowExp)
                    {
                        unitInfo.exp += getExp / ADD_VALUE_RATE;
                        lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.exp / nextExp) * gaugeRange - gaugePosX, gaugePosY, 0));
                        nowExp -= getExp / ADD_VALUE_RATE;
                    }
                    else
                    {
                        unitInfo.exp++;
                        lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.exp / nextExp) * gaugeRange - gaugePosX, gaugePosY, 0));
                        nowExp--;
                    }
                    if (!isActive) ChangeActive(true); // UIを表示する
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
        this.getExp = getExp;
        this.nowExp = getExp;
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

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
