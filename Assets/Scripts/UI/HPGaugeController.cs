using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPゲージの生成と更新
/// </summary>
public class HPGaugeController : MonoBehaviour
{
    public GameObject hpGauge;
    public GameObject hpGaugeBg;

    int hp = 0;
    LineRenderer lineRenderer;
    UnitInfo unitInfo;

    void Start()
    {
        // ゲージの生成
        hpGauge = Instantiate(hpGauge);
        hpGauge.transform.SetParent(transform, false);
        hpGaugeBg = Instantiate(hpGaugeBg);
        hpGaugeBg.transform.SetParent(transform, false);

        lineRenderer = hpGauge.GetComponent<LineRenderer>();
        unitInfo = gameObject.GetComponent<UnitInfo>();

        // 初期のHP反映
        hp = unitInfo.hp;
        lineRenderer.SetPosition(1, new Vector2(Mathf.Clamp01((float)hp / (float)unitInfo.hpMax) - 0.5f, -0.3f));
    }

    void Update()
    {
        // HPゲージの加算/減算と更新
        if (unitInfo.hp < hp)
            lineRenderer.SetPosition(1, new Vector2(Mathf.Clamp01((float)hp-- / (float)unitInfo.hpMax) - 0.5f, -0.3f));
        else if (unitInfo.hp > hp)
            lineRenderer.SetPosition(1, new Vector2(Mathf.Clamp01((float)hp++ / (float)unitInfo.hpMax) - 0.5f, -0.3f));
    }
}
