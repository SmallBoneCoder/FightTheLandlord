using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

public class ShowGameLobbyUICommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        
        if (evt.data != null)
        {
            gameData.PlayerSelf = new PlayerInfo() { Name = (string)evt.data };//获取名字
            
            dispatcher.Dispatch(ViewConst.UpdatePlayerSelfName, (string)evt.data);//显示名字
        }
        Transform canvas = ContextView.transform.Find("Canvas");
        GameObject GameLobbyUI = null;
        if (canvas.Find("GameLobbyUI(Clone)") != null)//已经加载过了
        {
            Debug.Log("ShowGameLobbyUICommand:" + gameData.PlayerSelf.Name);
            GameLobbyUI = canvas.Find("GameLobbyUI(Clone)").gameObject;
            GameLobbyUI.transform.SetSiblingIndex(canvas.childCount - 1);//显示在最前面
            GameLobbyUI.GetComponent<GameLobbyUI_View>().HideMatchPanel();//先隐藏匹配界面
            return;
        }
        GameObject go = Resources.Load<GameObject>("Prefabs/GameLobbyUI");
        GameLobbyUI = Object.Instantiate(go) as GameObject;
        GameLobbyUI.AddComponent<GameLobbyUI_View>();
        GameLobbyUI.transform.SetParent(canvas, false);
    }
}