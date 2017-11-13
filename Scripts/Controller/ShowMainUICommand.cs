using strange.extensions.command.impl;
using strange.extensions.context.api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMainUICommand : EventCommand {
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }
    public override void Execute()
    {
        Transform canvas = ContextView.transform.Find("Canvas");
        GameObject mainUI = null;
        if (canvas.Find("MainGameUI(Clone)") != null)//已经加载过了
        {
            Debug.Log("ShowMainUICommand");
            mainUI = canvas.Find("MainGameUI(Clone)").gameObject;
            mainUI.transform.SetSiblingIndex(canvas.childCount - 1);//显示在最前面
            //初始化数据
            mainUI.transform.Find("Player1").GetComponent<Player1_View>().ResetGame();
            mainUI.transform.Find("Player2").GetComponent<Player2_View>().ResetGame();
            mainUI.transform.Find("Player3").GetComponent<Player3_View>().ResetGame();
            mainUI.transform.Find("CurrentCard").GetComponent<CurrentCard_View>().ResetGame();
            mainUI.transform.Find("Status").GetComponent<Status_View>().ResetGame();
            dispatcher.Dispatch(NotificationConst.Noti_UpdatePlayerIdentity);
            return;
        }
        GameObject go = Resources.Load<GameObject>("Prefabs/MainGameUI");
        mainUI = Object.Instantiate(go) as GameObject;
        mainUI.transform.SetParent(canvas, false);
        //添加视图
        Transform OptionMenu = mainUI.transform.Find("OptionMenu");
        OptionMenu.gameObject.AddComponent<OptionMenu_View>();
        Transform Player3 = mainUI.transform.Find("Player3");
        Player3.gameObject.AddComponent<Player3_View>();
        Transform Player2 = mainUI.transform.Find("Player2");
        Player2.gameObject.AddComponent<Player2_View>();
        Transform Player1 = mainUI.transform.Find("Player1");
        Player1.gameObject.AddComponent<Player1_View>();
        Transform CurrentCard = mainUI.transform.Find("CurrentCard");
        CurrentCard.gameObject.AddComponent<CurrentCard_View>();
        Transform Status = mainUI.transform.Find("Status");
        Status.gameObject.AddComponent<Status_View>();

    }
}
