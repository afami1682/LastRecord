using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickBaseController : MonoBehaviour
{
    protected GameObject ActiveAreaManager;

    protected virtual void Start()
    {
        // GameManagerにユニット情報を登録する
        GameManager.GetUnit().AddMapUnitObj(transform.position, gameObject);

        ActiveAreaManager = GameObject.Find("ActiveAreaManager");
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// ターゲットに選択された時のイベント
    /// </summary>
    public virtual void SelectStart()
    {
        // UIの非表示
        ActiveAreaManager.SetActive(false);
    }

    /// <summary>
    /// ターゲットから外された時のイベント
    /// </summary>
    public virtual void SelectEnd()
    {
        // UIの表示
        ActiveAreaManager.SetActive(true);
    }

    /// <summary>
    /// ギミック処理
    /// </summary>
    public virtual void GimmickEvent()
    {

    }
}
