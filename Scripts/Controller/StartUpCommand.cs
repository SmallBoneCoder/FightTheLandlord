
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class StartUpCommand:EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }
    public override void Execute()
    {
        Transform canvas = ContextView.transform.Find("Canvas");
        GameObject GameStartUI = null;
        GameObject go = Resources.Load<GameObject>("Prefabs/GameStartUI");
        GameStartUI = Object.Instantiate(go) as GameObject;
        GameStartUI.AddComponent<GameStartUI_View>();
        GameStartUI.transform.SetParent(canvas, false);
        GameObject client = new GameObject("NetClient");
        client.AddComponent<ClientService>();//添加客户端服务
                                             //
        go = Resources.Load<GameObject>("Prefabs/MainGameUI");
        GameObject mainUI = Object.Instantiate(go) as GameObject;
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
        //
        go = Resources.Load<GameObject>("Prefabs/GameLobbyUI");
        GameObject GameLobbyUI = Object.Instantiate(go) as GameObject;
        GameLobbyUI.AddComponent<GameLobbyUI_View>();
        GameLobbyUI.transform.SetParent(canvas, false);
        //
        go = Resources.Load<GameObject>("Prefabs/GameOverUI");
        GameObject GameOverUI = Object.Instantiate(go) as GameObject;
        GameOverUI.AddComponent<GameOverUI_View>();
        GameOverUI.transform.SetParent(canvas, false);
        //
        dispatcher.Dispatch(ViewConst.UpdatePlayerSelfName, (string)evt.data);//显示名字
        //
        dispatcher.Dispatch(NotificationConst.Noti_ShowStartGameUI);//显示开始界面
        //
        GameObject pveController = GameObject.Find("PVEController");
        if (pveController == null)
        {
            pveController = new GameObject("PVEController");
            pveController.AddComponent<LocalService_View>();//添加本地服务器视图
        }
        else
        {
            pveController.GetComponent<LocalService_View>().Init();//初始化
        }
    }
}