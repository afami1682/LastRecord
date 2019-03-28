using UnityEngine;

public class CameraController : MonoBehaviour
{
    const int CAM_TRACKING_WIDTH = 7;
    const int CAM_TRACKING_HEIGHT = 4;

    Vector3 cursorPos, oldCursorPos;
    Camera mainCamera;
    PhaseManager phaseManager;

    // 移動範囲制御
    Vector2 displayMin;
    Vector2 displayMax;

    private void Start()
    {
        mainCamera = Camera.main;
        phaseManager = GameObject.Find("PhaseManager").GetComponent<PhaseManager>();

        // 画面左下のワールド座標をビューポートから取得
        displayMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)) - transform.position;
        // 画面右上のワールド座標をビューポートから取得
        displayMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) - transform.position;

        // カーソル更新時に呼び出す処理の登録
        CursorController.AddCallBack((Vector3 newPos) => { cursorPos = newPos; });
    }

    void Update()
    {
        // カメラの追従
        if (phaseManager.turnPlayer != Enums.UNIT_KIND.PLAYER && phaseManager.phase != Enums.PHASE.START)
        {
            // フォーカスユニットを追従する
            // カメラとの距離を計算
            if (phaseManager.focusUnitObj)
                Move(mainCamera.transform.position - phaseManager.focusUnitObj.transform.position, 15f);
        }
        else
        {
            // カーソルを追従する
            if (phaseManager.cursorObj.activeSelf || phaseManager.phase == Enums.PHASE.START)
                // カーソルの座標が更新されたら取得
                if (cursorPos != oldCursorPos)
                    oldCursorPos = cursorPos;

            // カメラとの距離を計算
            Move(mainCamera.transform.position - cursorPos, 8f);
        }
    }

    /// <summary>
    /// カメラの移動
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="speed"></param>
    private void Move(Vector3 distance, float speed)
    {
        // 左移動
        if (distance.x > CAM_TRACKING_WIDTH)
            if (0 <= transform.position.x + displayMin.x + 0.5f)
                mainCamera.transform.position += Vector3.left * Time.deltaTime * speed;

        // 右移動
        if (distance.x < -CAM_TRACKING_WIDTH)
            if (transform.position.x <= (GameManager.GetMap().field.width - displayMax.x - 0.5f))
                mainCamera.transform.position += Vector3.right * Time.deltaTime * speed;

        // 上移動
        if (distance.y < -CAM_TRACKING_HEIGHT)
            if (0 >= transform.position.y + displayMax.y - 0.5f)
                mainCamera.transform.position += Vector3.up * Time.deltaTime * speed;

        // 下移動
        if (distance.y > CAM_TRACKING_HEIGHT)
            if (transform.position.y >= (-GameManager.GetMap().field.height - displayMin.y + 0.5f))
                mainCamera.transform.position += Vector3.down * Time.deltaTime * speed;
    }
}