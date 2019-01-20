using UnityEngine;

public class CameraController : MonoBehaviour
{
    const int CAM_TRACKING_WIDTH = 7;
    const int CAM_TRACKING_HEIGHT = 4;

    const float x_aspect = 16.0f;
    const float y_aspect = 9.0f;

    Vector3 cursorPos, oldCursorPos;
    Camera mainCamera;
    PhaseManager phaseManager;

    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = calcAspect(x_aspect, y_aspect);
        camera.rect = rect;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        phaseManager = GameObject.Find("PhaseManager").GetComponent<PhaseManager>();

        // カーソル更新時に呼び出す処理の登録
        CursorController.AddCallBack((Vector3 newPos) => { cursorPos = newPos; });
    }

    void Update()
    {
        // カメラの追従
        if (phaseManager.turnPlayer != Enums.ARMY.ALLY && phaseManager.phase != Enums.PHASE.START)
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
            if (12 <= transform.position.x)
                mainCamera.transform.position += Vector3.left * Time.deltaTime * speed;

        // 右移動
        if (distance.x < -CAM_TRACKING_WIDTH)
            if (transform.position.x <= (GameManager.GetMap().field.width - 13))
                mainCamera.transform.position += Vector3.right * Time.deltaTime * speed;

        // 上移動
        if (distance.y < -CAM_TRACKING_HEIGHT)
            if (-6.5 >= transform.position.y)
                mainCamera.transform.position += Vector3.up * Time.deltaTime * speed;

        // 下移動
        if (distance.y > CAM_TRACKING_HEIGHT)
            if (transform.position.y >= (-GameManager.GetMap().field.height + 7.5))
                mainCamera.transform.position += Vector3.down * Time.deltaTime * speed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private Rect calcAspect(float width, float height)
    {
        float target_aspect = width / height;
        float window_aspect = (float)Screen.width / (float)Screen.height;
        float scale_height = window_aspect / target_aspect;
        Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (1.0f > scale_height)
        {
            rect.x = 0;
            rect.y = (1.0f - scale_height) / 2.0f;
            rect.width = 1.0f;
            rect.height = scale_height;
        }
        else
        {
            float scale_width = 1.0f / scale_height;
            rect.x = (1.0f - scale_width) / 2.0f;
            rect.y = 0.0f;
            rect.width = scale_width;
            rect.height = 1.0f;
        }
        return rect;
    }
}