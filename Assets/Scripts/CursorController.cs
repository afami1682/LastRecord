using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カーソルの描画,アクティブエリアの描画,フォーカスユニットの制御
/// </summary>
public class CursorController : MonoBehaviour
{
    // カーソル描画関連
    Vector2 mouseScreenPos;
    Vector3 cursorPos;
    Vector3 _cursorPos;

    public PhaseManager phaseManager;
    public CameraController cameraController;

    void Start()
    {
    }

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

            // カーソル更新イベントの呼び出し
            phaseManager.cursorUpdate(cursorPos);
            cameraController.cursorUpdate(cursorPos);
        }
    }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <param name="pos">Position.</param>
    public void SetPos(Vector3 pos)
    {
        cursorPos = pos;
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
}