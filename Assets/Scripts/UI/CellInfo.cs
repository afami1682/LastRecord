using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour {

    // セル情報描画関連
    public Text nameText, abText, dbText, mabText;

    /// <summary>
    /// エリア情報の更新
    /// </summary>
    /// <param name="cellInfo"></param>
    public void SetData(Struct.CellInfo cellInfo)
    {
        nameText.text = cellInfo.name;
        abText.text = string.Format("AB:{0}", cellInfo.avoidanceBonus.ToString());
        dbText.text = string.Format("DB:{0}", cellInfo.defenseBonus.ToString());
        mabText.text = string.Format("MAB:{0}", cellInfo.magicalDefenseBonus.ToString());
    }
}
// こだて