using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CutInAnimController : MonoBehaviour
{
    const float ANIM_END_TIME = 1.8f;

    // ユニット画像の移動
    AnimationCurve unitImageMove;
    public GameObject unitImageObj;
    RectTransform unitImageRect;

    // 上下の狭まり
    AnimationCurve panelHeight;
    RectTransform panelRect;

    // 全体の透明度
    AnimationCurve panelAlpha;
    CanvasGroup panelGroup;

    float time;
    Action callBackEvent;

    void Update()
    {
        // アニメーション
        unitImageRect.localPosition = new Vector3(unitImageMove.Evaluate(time), 0, 0);
        panelRect.sizeDelta = new Vector2(0, -panelHeight.Evaluate(time));
        panelGroup.alpha = panelAlpha.Evaluate(time);

        // アニメーションが終了したら非アクティブにして、コールバックイベントを実行する
        if (ANIM_END_TIME < (time += Time.deltaTime))
        {
            gameObject.SetActive(false);
            callBackEvent();
        }
    }

    /// <summary>
    /// Starts the animation.
    /// </summary>
    /// <param name="unitId">Unit identifier.</param>
    public void StartAnim(int unitId, Action callBackEvent)
    {
        this.callBackEvent = callBackEvent;
        time = 0;
        unitImageObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitId);
        unitImageRect = unitImageObj.GetComponent<RectTransform>();
        panelRect = gameObject.GetComponent<RectTransform>();
        panelGroup = GetComponent<CanvasGroup>();

        unitImageMove = new AnimationCurve(
                        new Keyframe(0.0f, -800),
                        new Keyframe(1.0f, 0),
                        new Keyframe(ANIM_END_TIME, 100)
                );

        panelHeight = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(1.0f, 400),
                      new Keyframe(ANIM_END_TIME, 400)
                );

        panelAlpha = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(1.0f, 1),
                      new Keyframe(1.6f, 1),
                      new Keyframe(ANIM_END_TIME, 0)
                );

        // アニメーションの初期値設定
        unitImageRect.localPosition = new Vector3(unitImageMove.Evaluate(0), 0, 0);
        panelRect.sizeDelta = new Vector2(0, -panelHeight.Evaluate(0));
        panelGroup.alpha = panelAlpha.Evaluate(0);

        gameObject.SetActive(true);
    }
}
