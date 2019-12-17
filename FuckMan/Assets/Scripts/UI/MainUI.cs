using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Button btnMatch;
    private Text textBtnMatch;

    private void Start()
    {
        btnMatch.onClick.AddListener(OnBtnMatch);
        textBtnMatch = btnMatch.GetComponentInChildren<Text>();
    }

    private void OnBtnMatch()
    {
        btnMatch.interactable = false;
        GameNetWork.Inst.Match((res) => { 
            if(res)
            {
                textBtnMatch.text = "匹配成功";
                GameManager.Inst.DestroyLocalPlayer();
            }
            else
            {
                btnMatch.interactable = true;
                textBtnMatch.text = "重新匹配";
            }
        });
    }
}
