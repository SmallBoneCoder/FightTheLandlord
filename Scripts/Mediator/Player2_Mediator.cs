using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player2_Mediator : EventMediator {
    [Inject]
	public Player2_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        //View.UpdateCards(17);
        dispatcher.AddListener(ViewConst.UpdatePlayer2Cards, CallBack_UpdatePlayer2Cards);
        dispatcher.AddListener(ViewConst.ShowPlayer2Msg, CallBack_ShowPlayer2Msg);
        dispatcher.AddListener(ViewConst.SwitchTimer_Player2, CallBack_SwitchTimer_Player2);
        dispatcher.AddListener(ViewConst.UpdatePlayer2Name, CallBack_UpdatePlayer2Name);
        dispatcher.AddListener(ViewConst.UpdatePlayer2Identiy, CallBack_UpdatePlayerIdentity);
    }
    private void CallBack_UpdatePlayerIdentity(IEvent evt)
    {
        View.UpdatePlayerIdentity((Sprite)evt.data);
    }
    private void CallBack_SwitchTimer_Player2(IEvent evt)
    {
        View.SwitchTimer((bool)evt.data);
    }
    private void CallBack_UpdatePlayer2Name(IEvent evt)
    {
        View.UpdatePlayerName((string)evt.data);
    }
    private void CallBack_UpdatePlayer2Cards(IEvent evt)
    {
        StartCoroutine(View.UpdateCards((int)evt.data,ViewConst.CardDelay));
    }
    private void CallBack_ShowPlayer2Msg(IEvent evt)
    {
        View.UpdateMsg((string)evt.data,ViewConst.MsgClearDelay);
    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.UpdatePlayer2Cards, CallBack_UpdatePlayer2Cards);
        dispatcher.RemoveListener(ViewConst.ShowPlayer2Msg, CallBack_ShowPlayer2Msg);
        dispatcher.RemoveListener(ViewConst.SwitchTimer_Player2, CallBack_SwitchTimer_Player2);
        dispatcher.RemoveListener(ViewConst.UpdatePlayer2Name, CallBack_UpdatePlayer2Name);
        dispatcher.RemoveListener(ViewConst.UpdatePlayer2Identiy, CallBack_UpdatePlayerIdentity);
    }
}
