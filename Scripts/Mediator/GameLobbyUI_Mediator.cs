using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;

public class GameLobbyUI_Mediator : EventMediator
{
    [Inject]
    public GameLobbyUI_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        View.dispatcher.AddListener(ViewConst.Click_Match, CallBack_Match);
        View.dispatcher.AddListener(ViewConst.Click_CancelMatch, CallBack_CancelMatch);
        View.dispatcher.AddListener(ViewConst.Click_PVE, CallBack_PVE);
        View.dispatcher.AddListener(ServiceConst.UpdateIPAndPort,Click_UpdateIPAndPort);
        dispatcher.AddListener(ViewConst.UpdatePlayerSelfName, CallBack_UpdatePlayerSelfName);
        dispatcher.AddListener(ViewConst.UpdatePlayerSelfGrade, CallBack_UpdatePlayerSelfGrade);
        dispatcher.AddListener(ViewConst.UpdatePlayerSelfIMG, CallBack_UpdatePlayerSelfIMG);
        dispatcher.AddListener(ViewConst.ShowMatchSuccessMsg, CallBack_ShowMatchSuccessMsg);
    }

    private void Click_UpdateIPAndPort(IEvent evt)
    {
        dispatcher.Dispatch(ServiceConst.UpdateIPAndPort, evt.data);
    }

    private void CallBack_ShowMatchSuccessMsg(IEvent payload)
    {
        View.ShowMatchSuccessMsg();
    }

    private void CallBack_UpdatePlayerSelfName(IEvent evt)
    {
        View.UpdatePlayerSelfName((string)evt.data);
    }
    private void CallBack_UpdatePlayerSelfIMG(IEvent evt)
    {
        View.UpdatePlayerSelfRoleIMG((Sprite)evt.data);
    }
    private void CallBack_UpdatePlayerSelfGrade(IEvent evt)
    {
        View.UpdatePlayerSelfGrade((int)evt.data);
    }
    private void CallBack_Match()
    {
        View.ShowMatchPanel();
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Match;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);
    }
    private void CallBack_CancelMatch()
    {
        View.HideMatchPanel();
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.CancelMatch;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);
    }
    private void CallBack_PVE()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Match;
        dispatcher.Dispatch(NotificationConst.Noti_StartPVE,recData);//开启单机
    }
    public override void OnRemove()
    {
        View.dispatcher.RemoveListener(ViewConst.Click_Match, CallBack_Match);
        View.dispatcher.RemoveListener(ViewConst.Click_CancelMatch, CallBack_CancelMatch);
        View.dispatcher.RemoveListener(ViewConst.Click_PVE, CallBack_PVE);
        View.dispatcher.RemoveListener(ServiceConst.UpdateIPAndPort, Click_UpdateIPAndPort);
        dispatcher.RemoveListener(ViewConst.UpdatePlayerSelfName, CallBack_UpdatePlayerSelfName);
        dispatcher.RemoveListener(ViewConst.UpdatePlayerSelfGrade, CallBack_UpdatePlayerSelfGrade);
        dispatcher.RemoveListener(ViewConst.UpdatePlayerSelfIMG, CallBack_UpdatePlayerSelfIMG);
    }
}