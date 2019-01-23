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

    const int ADD_SPEED = 50;

    public bool isActive;
    int nowExp = 0; // 更新中のexp値
    int getExp = 0; // 取得したexp
    int nextExp = 0; // そのレベルでの最大exp
    int gaugeRange = 0; // expゲージの長さ
    float gaugePosX, gaugePosY;
    UnitInfo unitInfo;
    Action callBackEvent;
    int expRate = 0;

    void Start()
    {
        lineRenderer = ExpGauge.GetComponent<LineRenderer>();

        // expゲージの長さを取得
        gaugeRange = (int)(lineRenderer.GetPosition(1).x - lineRenderer.GetPosition(0).x);
        gaugePosX = gaugeRange / 2;
        gaugePosY = lineRenderer.GetPosition(0).y;
        isActive = false;

        // UI非表示
        ChangeActive(false);
    }

    void Update()
    {
        if (isActive)
        {
            if (0 < nowExp)
            {
                if (0 < getExp / ADD_SPEED && getExp / ADD_SPEED < nowExp)
                {
                    unitInfo.exp += getExp / ADD_SPEED;
                    lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.exp / nextExp) * gaugeRange - gaugePosX, gaugePosY, 0));
                    nowExp -= getExp / ADD_SPEED;
                }
                else
                {
                    unitInfo.exp++;
                    lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)unitInfo.exp / nextExp) * gaugeRange - gaugePosX, gaugePosY, 0));
                    nowExp--;
                }
            } else
            {
                ChangeActive(false);
                callBackEvent();
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
        ChangeActive(true);
    }

    /// <summary>
    /// Expゲージの表示/非表示の切り替え
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    void ChangeActive(bool value)
    {
        // フラグの切り替え
        isActive = value;

        expText.SetActive(value);
        ExpGauge.SetActive(value);
        ExpGaugeBg.SetActive(value);
    }
}
