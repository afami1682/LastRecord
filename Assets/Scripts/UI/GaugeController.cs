using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPゲージの更新
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class GaugeController : MonoBehaviour
{
    int hp = 0;

    LineRenderer lineRenderer;
    UnitInfo unitInfo;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        unitInfo = transform.parent.gameObject.GetComponent<UnitInfo>();

        // 初期のHP反映
        hp = unitInfo.hp;
        lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)hp / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
    }

    void Update()
    {
        // HPゲージの加算/減算と更新
        if (unitInfo.hp < hp)
            lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)hp-- / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
        else if (unitInfo.hp > hp)
            lineRenderer.SetPosition(1, new Vector3(Mathf.Clamp01((float)hp++ / (float)unitInfo.vitality) - 0.5f, -0.3f, 0));
    }
}
