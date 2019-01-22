using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarrierController : MonoBehaviour {

    public Enums.MAGIC_CIRCLE magicCircleType;
    string bkName;
    Vector3 pos;

    void Start()
    {
        // 生成場所を取得
        pos = transform.position;

        // 魔法陣は平地にしか設置できない
        if (GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].cellId == 0)
        {
            bkName = GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name;
            switch (magicCircleType)
            {
                case Enums.MAGIC_CIRCLE.BLUE:
                    // 設置した場合はそのセルのavoidanceBonusを加算する
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name = "魔法障壁";
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].avoidanceBonus = 20;
                    break;

                case Enums.MAGIC_CIRCLE.GREEN:
                    // 設置した場合はそのセルのavoidanceBonusを加算する
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name = "魔法陣(緑)";
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].defenseBonus = 5;
                    break;

                case Enums.MAGIC_CIRCLE.PURPLE:
                    // 設置した場合はそのセルのavoidanceBonusを加算する
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name = "魔法陣(紫)";
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].magicalDefenseBonus = 5;
                    break;

                case Enums.MAGIC_CIRCLE.YELLOW:
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name = "魔法陣(黄)";
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].avoidanceBonus = 10;
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].defenseBonus = 3;
                    GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].magicalDefenseBonus = 3;
                    break;
            }
        }
        else
        {
            // 配置できない場所なら削除する
            Destroy(this);
        }

    }

    private void OnDestroy()
    {
        // 加算したボーナスを元に戻す
        GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].name = bkName;
        GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].avoidanceBonus = 0;
        GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].defenseBonus = 0;
        GameManager.GetMap().field.cells[-(int)pos.y, (int)pos.x].magicalDefenseBonus = 0;
    }
}
