using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStandby : MonoBehaviour
{
    public Image imgMy; // プレイヤーUnitの画像
    public Text textMyName; // プレイヤーUnitの名前
    public Text textMyHP; // プレイヤーUnitのHP
    public Text textMyAttackPower; // プレイヤーUnitの攻撃力
    public Text textMyAvoidance; // プレイヤーUnitの回避率
    public Text textMyDeathblow; // プレイヤーUnitの必殺率
    public Image imgEnemy; // 敵Unitの画像
    public Text textEnemyName; // 敵Unitの名前
    public Text textEnemyHP; // 敵UnitのHP
    public Text textEnemyAttackPower; // 敵Unitの攻撃力
    public Text textEnemyAvoidance;  // 敵Unitの回避率
    public Text textEnemyDeathblow;  // 敵Unitの必殺率

    /// <summary>
    /// Sets the player unit data.
    /// </summary>
    /// <param name="unitInfo">Unit info.</param>
    /// <param name="attackPower">Attack power.</param>
    /// <param name="avoidance">Avoidance.</param>
    /// <param name="deathblow">Deathblow.</param>
    public void SetMyUnitData(UnitInfo unitInfo, int attackPower, int avoidance, int deathblow)
    {
        imgMy.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitInfo.id);
        textMyName.text = unitInfo.unitName;
        textMyHP.text = unitInfo.hp.ToString();
        textMyAttackPower.text = attackPower.ToString();
        textMyAvoidance.text = string.Format("{0}%", avoidance.ToString());
        textMyDeathblow.text = string.Format("{0}%", deathblow.ToString());
    }

    /// <summary>
    /// Sets the enemy unit data.
    /// </summary>
    /// <param name="unitInfo">Unit info.</param>
    /// <param name="attackPower">Attack power.</param>
    /// <param name="avoidance">Avoidance.</param>
    /// <param name="deathblow">Deathblow.</param>
    public void SetEnemyUnitData(UnitInfo unitInfo, int attackPower, int avoidance, int deathblow)
    {
        imgEnemy.sprite = Resources.Load<Sprite>("Sprite/UnitFace/Chara" + unitInfo.id);
        textEnemyName.text = unitInfo.unitName;
        textEnemyHP.text = unitInfo.hp.ToString();
        textEnemyAttackPower.text = (attackPower != -1) ? attackPower.ToString() : "-";
        textEnemyAvoidance.text = (avoidance != -1) ? string.Format("{0}%", avoidance.ToString()) : "-";
        textEnemyDeathblow.text = (deathblow != -1) ? string.Format("{0}%", deathblow.ToString()) : "-";
    }
}
