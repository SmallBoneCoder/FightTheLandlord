using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player3_Mediator : EventMediator {
    [Inject]
	public Player3_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        //View.UpdateCards(17);
        dispatcher.AddListener(ViewConst.UpdatePlayer3Cards, CallBack_UpdatePlayer3Cards);
        dispatcher.AddListener(ViewConst.ShowPlayer3Msg, CallBack_ShowPlayer3Msg);
        dispatcher.AddListener(ViewConst.SwitchTimer_Player3, CallBack_SwitchTimer_Player3);
        dispatcher.AddListener(ViewConst.UpdatePlayer3Name, CallBack_UpdatePlayer3Name);
        dispatcher.AddListener(ViewConst.UpdatePlayer3Identiy, CallBack_UpdatePlayerIdentity);
    }
    private void CallBack_UpdatePlayerIdentity(IEvent evt)
    {
        View.UpdatePlayerIdentity((Sprite)evt.data);
    }
    private void CallBack_SwitchTimer_Player3(IEvent evt)
    {
        View.SwitchTimer((bool)evt.data);
    }
    private void CallBack_UpdatePlayer3Name(IEvent evt)
    {
        View.UpdatePlayerName((string)evt.data);
    }
    private void CallBack_UpdatePlayer3Cards(IEvent evt)
    {
        StartCoroutine(View.UpdateCards((int)evt.data,ViewConst.CardDelay));
    }
    private void CallBack_ShowPlayer3Msg(IEvent evt)
    {
        View.UpdateMsg((string)evt.data,ViewConst.MsgClearDelay);
    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.UpdatePlayer3Cards, CallBack_UpdatePlayer3Cards);
        dispatcher.RemoveListener(ViewConst.ShowPlayer3Msg, CallBack_ShowPlayer3Msg);
        dispatcher.RemoveListener(ViewConst.SwitchTimer_Player3, CallBack_SwitchTimer_Player3);
        dispatcher.RemoveListener(ViewConst.UpdatePlayer3Name, CallBack_UpdatePlayer3Name);
        dispatcher.RemoveListener(ViewConst.UpdatePlayer3Identiy, CallBack_UpdatePlayerIdentity);
    }
}
