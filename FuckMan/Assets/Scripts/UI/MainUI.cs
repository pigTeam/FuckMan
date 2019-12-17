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

    }
}
