using UnityEngine;
using strange.extensions.command.impl;
using System.Collections.Generic;

public class UpdateSelectedCardsCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        Debug.Log("Update Selected Cards:"+LitJson.JsonMapper.ToJson(evt.data as Card[]));
        Card[] cards = evt.data as Card[];//这次选择的牌
        gameData.SelectedCards.Clear();
        gameData.SelectedCards.AddRange((Card[])cards.Clone());
        
    }
}