using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameOverUI_Mediator : EventMediator
{
    [Inject]
    public GameOverUI_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        View.dispatcher.AddListener(ViewConst.Click_ReturnLobby, CallBack_ReturnLobby);
        dispatcher.AddListener(ViewConst.UpdateGameOverResult, CallBack_UpdateGameOverResult);
    }
    private void CallBack_UpdateGameOverResult(IEvent evt)
    {
        View.UpdateResultMsg((string)evt.data);
    }
    private void CallBack_ReturnLobby()
    {
        dispatcher.Dispatch(NotificationConst.Noti_ShowGameLobbyUI);
    }
    public override void OnRemove()
    {
        View.dispatcher.RemoveListener(ViewConst.Click_ReturnLobby, CallBack_ReturnLobby);
        dispatcher.RemoveListener(ViewConst.UpdateGameOverResult, CallBack_UpdateGameOverResult);
    }
}