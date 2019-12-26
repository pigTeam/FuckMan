using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class InjuryUI : MonoBehaviour
{
    public Text injury1p;
    public Text injury2p;
    public Text shotDown1p;
    public Text shotDown2p;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        injury1p.gameObject.SetActive(false);
        injury2p.gameObject.SetActive(false);
        int[,] status = GameManager.Inst.GetPlayerStatus();

        injury1p.gameObject.SetActive(true);
        injury1p.text = status[0, 0] + "%";
        shotDown1p.text = status[0, 1].ToString();

        injury2p.gameObject.SetActive(true);
        injury2p.text = status[1,0] + "%";
        shotDown2p.text = status[1,1] + "";
    }
}
