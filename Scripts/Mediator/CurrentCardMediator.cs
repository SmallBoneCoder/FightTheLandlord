using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCardMediator : EventMediator {
    [Inject]
	public CurrentCard_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        
        dispatcher.AddListener(ViewConst.ShowBaseCards_Back,CallBack_ShowBaseCards_Back);
        dispatcher.AddListener(ViewConst.ShowBaseCards_Value, CallBack_ShowBaseCards_Value);
        dispatcher.AddListener(ViewConst.ShowDiscards, CallBack_ShowDiscards);
        dispatcher.AddListener(ViewConst.ShowDiscardMsg, CallBack_ShowDiscardMsg);
    }
    private void Test()
    {
        View.UpdateCards(new Card[]
        {
            new Card(PokerConst.Diamond,6),
            new Card(PokerConst.Club,6),
            new Card(PokerConst.Heart,6)
        });
    }
    private void CallBack_ShowBaseCards_Back()
    {
        View.ShowBackCard();
    }
    private void CallBack_ShowBaseCards_Value(IEvent evt)
    {
        Card[] cards = evt.data as Card[];
        View.UpdateCards(cards);
    }
    private void CallBack_ShowDiscards(IEvent evt)
    {
        Card[] cards = evt.data as Card[];
        View.UpdateCards(cards);
    }
    private void CallBack_ShowDiscardMsg(IEvent evt)
    {
        string  msg = evt.data as string;
        View.UpdateCardMsg(msg);
    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.ShowBaseCards_Back, CallBack_ShowBaseCards_Back);
        dispatcher.RemoveListener(ViewConst.ShowBaseCards_Value, CallBack_ShowBaseCards_Value);
        dispatcher.RemoveListener(ViewConst.ShowDiscards, CallBack_ShowDiscards);
        dispatcher.RemoveListener(ViewConst.ShowDiscardMsg, CallBack_ShowDiscardMsg);
    }
}
