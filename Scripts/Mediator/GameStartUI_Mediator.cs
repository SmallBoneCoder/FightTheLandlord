using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class GameStartUI_Mediator : EventMediator
{
    [Inject]
    public GameStartUI_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        View.dispatcher.AddListener(ViewConst.Click_StartGame, CallBack_StartGame);

    }
    private void CallBack_StartGame(IEvent evt)
    {
        dispatcher.Dispatch(NotificationConst.Noti_ShowGameLobbyUI,(string)evt.data);//显示游戏大厅
    }
    
    public override void OnRemove()
    {
        View.dispatcher.RemoveListener(ViewConst.Click_StartGame, CallBack_StartGame);
    }
}