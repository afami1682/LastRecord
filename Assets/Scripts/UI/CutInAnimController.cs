using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CutInAnimController : MonoBehaviour
{
    const float FPS = 60f;
    const float ANIM_END_TIME = 1.4f;

    float oneFrameTime;
    float time;
    Action callBackEvent;

    // ユニット画像の移動
    AnimationCurve unitImageMove;
    public GameObject unitImageObj;
    RectTransform unitImageRect;

    // アクションネーム
    AnimationCurve actionNameMove;
    public GameObject actionNameObj;
    RectTransform actionNameRect;

    // 上下の狭まり
    AnimationCurve panelHeight;
    RectTransform panelRect;

    // 全体の透明度
    AnimationCurve panelAlpha;
    CanvasGroup panelGroup;

    /// <summary>
    /// Starts the animation.
    /// </summary>
    /// <param name="unitId">Unit identifier.</param>
    public void StartAnim(int unitId, string actionName, Action callBackEvent)
    {
        this.callBackEvent = callBackEvent;
        oneFrameTime = 1f / FPS;
        time = 0;
        unitImageObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitId);
        unitImageRect = unitImageObj.GetComponent<RectTransform>();
        panelRect = gameObject.GetComponent<RectTransform>();
        actionNameObj.GetComponent<Text>().text = actionName;
        actionNameRect = actionNameObj.GetComponent<RectTransform>();
        panelGroup = GetComponent<CanvasGroup>();

        unitImageMove = new AnimationCurve(
                        new Keyframe(0.0f, -500),
                        new Keyframe(0.6f, 0),
                        new Keyframe(ANIM_END_TIME, 150)
                );

        actionNameMove = new AnimationCurve(
                        new Keyframe(0.0f, 500),
                        new Keyframe(0.6f, -100),
                        new Keyframe(ANIM_END_TIME, -250)
                );

        panelHeight = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(0.6f, 400)
                );

        panelAlpha = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(0.6f, 1),
                      new Keyframe(1.2f, 1),
                      new Keyframe(ANIM_END_TIME, 0)
                );

        // アニメーションの初期値設定
        unitImageRect.localPosition = new Vector3(unitImageMove.Evaluate(0), 0, 0);
        actionNameRect.localPosition = new Vector3(actionNameMove.Evaluate(0), 0, 0);
        panelRect.sizeDelta = new Vector2(0, -panelHeight.Evaluate(0));
        panelGroup.alpha = panelAlpha.Evaluate(0);

        gameObject.SetActive(true);
        InvokeRepeating("NextFrame", 0f, oneFrameTime);
    }

    /// <summary>
    /// Nexts the frame.
    /// </summary>
    void NextFrame()
    {
        // アニメーション
        unitImageRect.localPosition = new Vector3(unitImageMove.Evaluate(time), 0, 0);
        actionNameRect.localPosition = new Vector3(actionNameMove.Evaluate(time), 0, 0);
        panelRect.sizeDelta = new Vector2(0, -panelHeight.Evaluate(time));
        panelGroup.alpha = panelAlpha.Evaluate(time);

        // アニメーションが終了したら非アクティブにして、コールバックイベントを実行する
        if (ANIM_END_TIME < (time += oneFrameTime))
        {
            CancelInvoke(); // InvokeRepeatingの終了
            gameObject.SetActive(false);
            callBackEvent();
        }
    }
}
