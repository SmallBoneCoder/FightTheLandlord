using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status_View : EventView {
    
    private Text Mutiple_Txt;
    private Text Base_Txt;
    public void Init()
    {
        Mutiple_Txt = transform.Find("Mutiple_Txt").GetComponent<Text>();
        Base_Txt = transform.Find("Base_Txt").GetComponent<Text>();
    }
    public void ResetGame()
    {
        Base_Txt.text = "1000";
        Mutiple_Txt.text = "1";
    }
    public void UpdateBase(int msg)
    {
        Base_Txt.text = msg.ToString();
    }
    public void UpdateMutiple(int msg)
    {
        Mutiple_Txt.text = msg.ToString();
    }
}
