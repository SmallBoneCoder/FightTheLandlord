using UnityEngine;
using strange.extensions.command.impl;
using System.Collections.Generic;

public class TipsCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        gameData.HandCards.AddRange((Card[])gameData.SelectedCards.ToArray().Clone());
        List<Card> selectCards = gameData.GetEnabledDisCards(
            new List<Card>((Card[])gameData.HandCards.ToArray().Clone())
            ,gameData.PlayerSelf.Name);
        if (selectCards.Count > 0)
        {
            gameData.SelectedCards.Clear();
            gameData.SelectedCards.AddRange((Card[])selectCards.ToArray().Clone());
            //更新选择的牌的数据
            dispatcher.Dispatch(NotificationConst.Noti_UpdateSelectedCards, (Card[])selectCards.ToArray().Clone());
            //更显显示UI
            dispatcher.Dispatch(ViewConst.ShowTipsCards, (Card[])selectCards.ToArray().Clone());
        }
    }
}
