using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

public class GameOverUI_View : EventView
{

    private Text Result_Txt;//游戏结果
    private Button Btn_Return;//返回大厅
    public void Init()
    {
        Result_Txt = transform.Find("Result_Txt").GetComponent<Text>();
        Btn_Return = transform.Find("Btn_Return").GetComponent<Button>();
        Btn_Return.onClick.AddListener(Click_BtnReturn);//点击事件
    }
    public void UpdateResultMsg(string msg)
    {
        Result_Txt.text = msg;
    }

    private void Click_BtnReturn()
    {
        dispatcher.Dispatch(ViewConst.Click_ReturnLobby);//返回大厅
    }
}
