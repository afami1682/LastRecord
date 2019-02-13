using UnityEngine;
using System.Linq;

/// <summary>
/// Unit負けイベント
/// </summary>
public class UnitLoseEvent : MonoBehaviour
{
    //public PhaseManager phaseManager;
    public BattleManager battleManager;
    SpriteRenderer spriteRenderer;

    const float FADE_TIME = 0.8f; // フェードタイム
    float currentRemainTime;

    // 各イベントの実行フラグ
    bool[] runninge = {
        true // フェードアウト中
    };

    void Start()
    {
        // 初期化
        currentRemainTime = FADE_TIME;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (runninge[0])
        {
            // 残り時間を更新
            currentRemainTime -= Time.deltaTime;

            if (currentRemainTime <= 0f)
            {
                // 残り時間が無くなったら自身を消滅
                GameManager.GetUnit().RemoveMapUnitObj(transform.position);
                Destroy(gameObject);
                runninge[0] = false;
            }

            // フェードアウト
            float alpha = currentRemainTime / FADE_TIME;
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        // 全ての小イベントが終了（false)したら自身を削除する
        if (runninge.All(value => value == false)) Destroy(this);
    }

    /// <summary>
    /// コンポーネント削除時に呼ばれる
    /// </summary>
    private void OnDestroy()
    {
        // 次のバトルイベントを実行する
        battleManager.EndEvent();
    }
}
