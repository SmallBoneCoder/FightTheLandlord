using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player1_Mediator : EventMediator {
    [Inject]
	public Player1_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        
        //dispatcher.AddListener(ViewConst.RequestDiscards, CallBack_RequestDiscards);
        dispatcher.AddListener(ViewConst.Click_Reset, CallBack_Reset);
        dispatcher.AddListener(ViewConst.RemoveAllDiscards, CallBack_RemoveAllDiscards);
        dispatcher.AddListener(ViewConst.ShowHandCards, CallBack_ShowHandCards);
        dispatcher.AddListener(ViewConst.UpdatePlayerSelfName, CallBack_UpdatePlayerName);
        dispatcher.AddListener(ViewConst.UpdatePlayerSelfIdentiy, CallBack_UpdatePlayerIdentity);
        dispatcher.AddListener(ViewConst.ShowTipsCards, CallBack_ShowTipsCards);
        //
        View.dispatcher.AddListener(ViewConst.Click_SelectCard, CallBack_SelectCard);
        View.dispatcher.AddListener(ViewConst.Click_DisSelectCard, CallBack_DisSelectCard);
    }

    private void CallBack_ShowTipsCards(IEvent evt)
    {
        View.SelectedAllCards(evt.data as Card[]);
    }

    private void Test()
    {
        StartCoroutine(View.ConstructCards(new Card[]
        {
            new Card(PokerConst.Spade,1),
            new Card(PokerConst.Spade,3),
            new Card(PokerConst.Spade,5),
            new Card(PokerConst.Spade,7),
            new Card("",14),
            new Card("",15),
            new Card(PokerConst.Spade,10),
            new Card(PokerConst.Spade,11),
            new Card(PokerConst.Spade,12),
            new Card(PokerConst.Spade,4),
            new Card(PokerConst.Club,3),
            new Card(PokerConst.Spade,2),
            new Card(PokerConst.Club,1),
            new Card(PokerConst.Heart,1),
            new Card(PokerConst.Diamond,1),
            new Card(PokerConst.Spade,8),
            new Card(PokerConst.Diamond,5),
        }, ViewConst.CardDelay));
    }
    private void CallBack_UpdatePlayerIdentity(IEvent evt)
    {
        View.UpdatePlayerIdentity((Sprite)evt.data);
    }
    private void CallBack_UpdatePlayerName(IEvent evt)
    {
        View.UpdatePlayerName((string)evt.data);
    }
    private void CallBack_ShowHandCards(IEvent evt)
    {
        StartCoroutine(View.ConstructCards((Card[])evt.data,ViewConst.CardDelay));
    }
    private void CallBack_RemoveAllDiscards()
    {
        View.RemoveAllDiscards();
    }
    private void CallBack_Reset()
    {
        View.DisSelectedAllCards();
        Debug.Log("Reset");
    }
    private void CallBack_SelectCard(IEvent evt)
    {
        Debug.Log("Select");
        dispatcher.Dispatch(NotificationConst.Noti_UpdateSelectedCards,
            (evt.data as Card[]));
    }
    private void CallBack_DisSelectCard(IEvent evt)
    {
        Debug.Log("DisSelect");
        dispatcher.Dispatch(NotificationConst.Noti_UpdateSelectedCards,
            (evt.data as Card[]));

    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.Click_Reset, CallBack_Reset);
        dispatcher.RemoveListener(ViewConst.RemoveAllDiscards, CallBack_RemoveAllDiscards);
        dispatcher.RemoveListener(ViewConst.ShowHandCards, CallBack_ShowHandCards);
        dispatcher.RemoveListener(ViewConst.UpdatePlayerSelfIdentiy, CallBack_UpdatePlayerIdentity);
        dispatcher.RemoveListener(ViewConst.UpdatePlayerSelfName, CallBack_UpdatePlayerName);
        dispatcher.RemoveListener(ViewConst.ShowTipsCards, CallBack_ShowTipsCards);
        //
        View.dispatcher.RemoveListener(ViewConst.Click_SelectCard, CallBack_SelectCard);
        View.dispatcher.RemoveListener(ViewConst.Click_DisSelectCard, CallBack_DisSelectCard);
    }
}
