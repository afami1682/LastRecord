using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// カットインアニメーション
/// </summary>
public class CutInAnimController : MonoBehaviour
{
    const float FPS = 60f; // フレームの描画間隔
    const float ANIM_TOTAL_TIME = 1.4f; // アニメーション全体の時間
    const float ANIM_END_WAIT_TIME = 0.5f; // アニメーション後の待機時間

    float time;
    float oneFrameTime;
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
    /// <param name="actionName">Action name.</param>
    /// <param name="callBackEvent">Call back event.</param>
    public void StartAnim(int unitId, string actionName, Action callBackEvent)
    {
        this.callBackEvent = callBackEvent;
        oneFrameTime = 1f / FPS;
        time = 0;
        unitImageObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UnitFace/Unit" + unitId);
        unitImageRect = unitImageObj.GetComponent<RectTransform>();
        panelRect = gameObject.GetComponent<RectTransform>();
        actionNameObj.GetComponent<Text>().text = actionName;
        actionNameRect = actionNameObj.GetComponent<RectTransform>();
        panelGroup = GetComponent<CanvasGroup>();

        unitImageMove = new AnimationCurve(
                        new Keyframe(0.0f, -500),
                        new Keyframe(0.6f, 0),
                        new Keyframe(ANIM_TOTAL_TIME, 150)
                );

        actionNameMove = new AnimationCurve(
                        new Keyframe(0.0f, 500),
                        new Keyframe(0.6f, -100),
                        new Keyframe(ANIM_TOTAL_TIME, -250)
                );

        panelHeight = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(0.6f, 400)
                );

        panelAlpha = new AnimationCurve(
                      new Keyframe(0.0f, 0),
                      new Keyframe(0.6f, 1),
                      new Keyframe(1.2f, 1),
                      new Keyframe(ANIM_TOTAL_TIME, 0)
                );

        // アニメーションの初期値設定
        unitImageRect.localPosition = new Vector3(unitImageMove.Evaluate(0), 0, 0);
        actionNameRect.localPosition = new Vector3(actionNameMove.Evaluate(0), 0, 0);
        panelRect.sizeDelta = new Vector2(0, -panelHeight.Evaluate(0));
        panelGroup.alpha = panelAlpha.Evaluate(0);

        // 繰り返し処理の開始とUIの表示
        InvokeRepeating("NextFrame", 0f, oneFrameTime);
        gameObject.SetActive(true);
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
        if (ANIM_TOTAL_TIME < (time += oneFrameTime))
        {
            CancelInvoke(); // InvokeRepeatingの終了
            // 1秒後にコールバック処理を実行する
            StartCoroutine(DelayMethod(ANIM_END_WAIT_TIME, () =>
            {
                gameObject.SetActive(false);
                callBackEvent();
            }));
        }
    }

    /// <summary>
    /// Delaies the method.
    /// </summary>
    /// <returns>The method.</returns>
    /// <param name="waitTime">Wait time.</param>
    /// <param name="action">Action.</param>
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
