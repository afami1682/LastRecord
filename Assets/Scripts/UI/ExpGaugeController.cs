using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpGaugeController : MonoBehaviour
{

    public GameObject expText;
    public GameObject ExpGauge;
    public GameObject ExpGaugeBg;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = ExpGauge.GetComponent<LineRenderer>();
    }

    void Update()
    {

    }

    /// <summary>
    /// 経験値取時のゲージ更新処理
    /// </summary>
    /// <param name="getExp">Exp.</param>
    /// <param name="unitInfo">Level.</param>
    public void GaugeUpdate(int getExp, UnitInfo unitInfo)
    {

    }

    /// <summary>
    /// Expゲージの表示/非表示の切り替え
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    void ChangeActive(bool value)
    {
        expText.SetActive(value);
        ExpGauge.SetActive(value);
        ExpGaugeBg.SetActive(value);
    }
}
