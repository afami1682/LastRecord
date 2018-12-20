﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラの追尾範囲
    const int CAM_TRACKING_WIDTH = 7;
    const int CAM_TRACKING_HEIGHT = 5;

    Vector3 cursorPos, oldCursorPos, distance;
    Camera mainCamera;

    float span = 0.1f; // 生成間隔
    float delta = 0; // 計測時間

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // カーソル座標が更新されてないなら更新する
        if (cursorPos != oldCursorPos)
        {
            // カーソルの座標を更新
            oldCursorPos = cursorPos;

            // カメラとの距離を計算
            distance = mainCamera.transform.position - cursorPos;
        }

        // カメラ座標の更新処理
        if (span < delta)
        {
            delta = 0;

            if (distance.x > CAM_TRACKING_WIDTH)
                mainCamera.transform.position += Vector3.left;
            else if (distance.x < -CAM_TRACKING_WIDTH)
                mainCamera.transform.position += Vector3.right;

            if (distance.y > CAM_TRACKING_HEIGHT)
                mainCamera.transform.position += Vector3.down;
            else if (distance.y < -CAM_TRACKING_HEIGHT)
                mainCamera.transform.position += Vector3.up;
        }
        delta += Time.deltaTime;
    }

    /// <summary>
    /// カーソルの更新時に呼ばれる
    /// </summary>
    /// <param name="newPos">New position.</param>
    public void cursorUpdate(Vector3 newPos) { cursorPos = newPos; }
}