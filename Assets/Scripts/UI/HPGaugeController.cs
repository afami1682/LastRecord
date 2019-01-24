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
        lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01(((float)hp / 2) / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
    }

    void Update()
    {
        // HPゲージの加算/減算と更新
        if (unitInfo.hp < hp)
            lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01(((float)hp-- / 2) / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
        else if (unitInfo.hp > hp)
            lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01(((float)hp++ / 2) / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
    }
}
