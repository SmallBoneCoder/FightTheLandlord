using UnityEngine;

using strange.extensions.command.impl;

public class DealCardsCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        Debug.Log("DealCards:Get HandCards");
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        gameData.HandCards.Clear();
        gameData.HandCards.AddRange((Card[])recData.cards.Clone());//保存手牌
        dispatcher.Dispatch(ViewConst.ShowHandCards,(Card[])recData.cards.Clone());//显示手牌
        dispatcher.Dispatch(ViewConst.UpdatePlayer2Cards, 17);//显示玩家2的牌
        dispatcher.Dispatch(ViewConst.UpdatePlayer3Cards, 17);//显示玩家3的牌
    }
}