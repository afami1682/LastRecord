using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// カーソルの描画,アクティブエリアの描画,フォーカスユニットの制御
/// </summary>
public class CursorController : MonoBehaviour
{
    // カーソル描画関連
    Vector2 mouseScreenPos;
    Vector3 cursorPos;
    Vector3 _cursorPos;

    // カーソル実行時に行う処理のリスト
    static List<Action<Vector3>> callbackList = new List<Action<Vector3>>();

    private void Update()
    {
        // マウスの座標を変換して取得
        mouseScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _cursorPos = new Vector3(MultipleRound(mouseScreenPos.x, 1),
                                MultipleRound(mouseScreenPos.y, 1), 0);

        // マップ内なら新しいカーソル座標を取得する
        if (0 <= _cursorPos.x && _cursorPos.x < GameManager.GetMap().field.width &&
            0 <= -_cursorPos.y && -_cursorPos.y < GameManager.GetMap().field.height)
            cursorPos = _cursorPos;

        // カーソル座標が更新されてないなら更新する
        if (transform.position != cursorPos)
        {
            // カーソルの座標を更新
            transform.position = cursorPos;

            // カーソル更新時に呼び出す処理
            foreach (Action<Vector3> callback in callbackList) callback(cursorPos);
        }
    }

    /// <summary>
    /// 倍数での四捨五入のような値を求める（ｎおきの数の中間の値で切り捨て・切り上げをする）
    ///（例）倍数 = 10 のとき、12 → 10, 17 → 20
    /// </summary>
    /// <param name="value">入力値</param>
    /// <param name="multiple">倍数</param>
    /// <return>倍数の中間の値で、切り捨て・切り上げした値</return>
    private static float MultipleRound(float value, float multiple)
    {
        return MultipleFloor(value + multiple * 0.5f, multiple);
    }

    /// <summary>
    /// より小さい倍数を求める（倍数で切り捨てられるような値）
    ///（例）倍数 = 10 のとき、12 → 10, 17 → 10
    /// </summary>
    /// <param name="value">入力値</param>
    /// <param name="multiple">倍数</param>
    /// <return>倍数で切り捨てた値</return>
    private static float MultipleFloor(float value, float multiple)
    {
        return Mathf.Floor(value / multiple) * multiple;
    }

    /// <summary>
    /// カーソル更新時に実行するコールバック処理の登録
    /// </summary>
    /// <param name="callback">Callback.</param>
    public static void AddCallBack(Action<Vector3> callback) { callbackList.Add(callback); }
}