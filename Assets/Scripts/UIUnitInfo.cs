using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInfo : MonoBehaviour
{

    public GameObject flame;
    public Image faceImage;
    public Text nameText;
    public Text levelText;
    public Text lifeText;
    public GameObject gageImage;
    public RectTransform gageImageBk;

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
        flame.SetActive(false);
    }

    public void ShowUnitInfo(UnitInfo unitInfo)
    {
        // フォーカスユニットがいるならユニット情報を表示する
        flame.SetActive(true);
        faceImage.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitInfo.ID);
        nameText.text = unitInfo.Name;
        levelText.text = string.Format("Lv: {0}", unitInfo.Level);
        lifeText.text = string.Format("HP: {0}/{1}", unitInfo.HP, unitInfo.maxHP);
        lifeRate = (float)unitInfo.HP / (float)unitInfo.maxHP;
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
        flame.SetActive(false);
    }
}
