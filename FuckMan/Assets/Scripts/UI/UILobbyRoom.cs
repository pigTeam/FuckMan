using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILobbyRoom : UIBase
{
    private Button btnSingle;
    private Button btnMatch;

    // Use this for initialization
    void Start()
    {
        btnSingle = UIUtil.Instance.GetComponentInChildren<Button>(transform, "BtnSingle");
        btnSingle.onClick.AddListener(OnSingle);
        btnMatch = UIUtil.Instance.GetComponentInChildren<Button>(transform, "BtnMatch");
        btnMatch.onClick.AddListener(OnMatch);
    }

    void OnSingle()
    {
        Hide();
    }

    void OnMatch()
    {
        Hide();
        GameNetWork.Inst.Match((suc) => { Debug.LogError("match res:"+suc); });
    }

    

}
