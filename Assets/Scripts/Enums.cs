using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 列挙型定数一覧
/// </summary>
public class Enums
{
    /// <summary>
    /// アクティブエリアの識別
    /// </summary>
    public enum AREA { NONE, UNIT, MOVE, ATTACK }

    /// <summary>
    /// 移動方向
    /// </summary>
    public enum MOVE { NONE, UP, DOWN, LEFT, RIGHT }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    public enum PHASE
    {
        START, // プレイヤーのターン開始時
        SELECT, // Unit選択中
        FOCUS, // Unit選択時
        MOVE, // Unit行動時
        BATTLE_STANDBY, // Unit攻撃選択時
        BATTLE, // Unit攻撃時
        RESULT, // Unit攻撃終了時（まだ見操作のUnitがいれば、SELECTに戻る）
        END // プレイヤーのターン終了時
    };

    /// <summary>
    /// 勢力
    /// </summary>
    public enum ARMY
    {
        ALLY, // プレイヤー側（青Unit）
        ENEMY, // 敵（赤Unit）
        NEUTRAL // 第３勢力（緑Unit）
    }

    /// <summary>
    /// Unitの移動タイプ
    /// </summary>
    public enum MOVE_TYPE
    {
        WALKING, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        ATHLETE, // 「建物」以外は移動可能
        HORSE, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        FLYING, // 「深森」以外は移動可能
    }

    /// <summary>
    /// Unitの状態
    /// </summary>
    public enum STATUS
    {
        HEALTH, // 健康（通常）
        POISON, // 毒（数ターンの間、小ダメージを受ける）
        SLEEP, // 睡眠（数ターンの間、行動不可）
        CONFUSION, // 混乱（数ターンの間、操作不能かつ無差別攻撃）
        SILENCE, // 沈黙（数ターンの間、魔法使用不可）
        HANDCUFFS, //手枷（数ターンの間、攻撃不可）
        FETTERS // 足枷 （数ターンの間、移動不可）
    }

    /// <summary>
    /// クラスタイプ
    /// </summary>
    public enum CLASS_TYPE
    {
        SOLDIER, 





        //アサシン
        //アーチャー
        //アーマーナイト
        //ウォーリア
        //グレートローど
        //シスター
        //シャーマン
        //ジェネラル
        //スナイパー
        //ソシアルナイト
        //ソルジャー
        //ソードマスター
        //トルバードル
        //ドラゴンナイト　ますた
        //パラディン
        //ファルコンナイト
        //ペガサスナイト
        //ヴァルキュリア
        //修道士
        //傭兵
        //剣士
        //勇者
        //司祭
        //大賢者
        //山賊
        //市民
        //戦士
        //海賊
        //狂戦士
        //盗賊
        //賢者
        //踊り子
        //輸送隊
        //遊牧民
        //遊牧騎兵
        //魔道士
    }
}
