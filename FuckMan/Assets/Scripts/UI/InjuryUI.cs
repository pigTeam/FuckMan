using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class InjuryUI : MonoBehaviour
{
    public Text text1p;
    public Text text2p;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text1p.gameObject.SetActive(false);
        text2p.gameObject.SetActive(false);
        List<int> injuries = GameManager.Inst.GetPlayerInjuries();
        if(injuries.Count > 0)
        {
            text1p.gameObject.SetActive(true);
            text1p.text = injuries[0] + "%";
        }
        if(injuries.Count > 1)
        {
            text2p.gameObject.SetActive(true);
            text2p.text = injuries[1] + "%";
        }
    }
}
