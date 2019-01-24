using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップ上でUnitにカーソルが当たった時に表示されるUI（Unit情報）の管理クラス
/// </summary>
public class SelectUnitInfo : MonoBehaviour
{
    public GameObject selectUnitInfo;
    public Image faceImage;
    public Text nameText;
    public Text levelText;
    public Text lifeText;
    public GameObject gageImage;
    public RectTransform gageImageBk;

    /// <summary>
    /// 各体力割合時の色指定
    /// </summary>
    public Color lifeGreen;
    public Color lifeYellow;
    public Color lifeRed;

    private float lifeRate;
    private RectTransform gageImageRect;
    private Image gageImageColor;

    void Start()
    {
        // ゲージイメージのコンポーネント取得
        gageImageRect = gageImage.GetComponent<RectTransform>();
        gageImageColor = gageImage.GetComponent<Image>();

        // ゲーム開始時は非表示
        selectUnitInfo.SetActive(false);
    }

    public void ShowUnitInfo(UnitInfo unitInfo)
    {
        // フォーカスユニットがいるならユニット情報を表示する
        selectUnitInfo.SetActive(true);
        faceImage.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitInfo.id);
        nameText.text = unitInfo.unitName;
        levelText.text = string.Format("Lv: {0}", unitInfo.level);
        lifeText.text = string.Format("HP: {0}/{1}", unitInfo.hp, unitInfo.vitality * 2);
        lifeRate = ((float)unitInfo.hp / 2) / (float)unitInfo.vitality;
        gageImageRect.sizeDelta = new Vector2(
            lifeRate * (float)gageImageBk.sizeDelta.x,
                                          gageImageBk.sizeDelta.y);
        if (lifeRate <= 0.2)
            gageImageColor.color = lifeRed;
        else if (lifeRate <= 0.6)
            gageImageColor.color = lifeYellow;
        else if (lifeRate <= 1)
            gageImageColor.color = lifeGreen;
    }

    public void CloseUnitInfo()
    {
        selectUnitInfo.SetActive(false);
    }
}
