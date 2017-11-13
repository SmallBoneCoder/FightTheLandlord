using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class DiscardCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        Debug.Log("DiscardCommand is Executing...:" + recData.player.Name);
        gameData.LastPlayer = recData.player.Name;//出牌人
        //从手牌中删除
        for (int i = 0; i < recData.cards.Length; i++)
        {
            for (int j = 0; j < gameData.HandCards.Count; j++)
            {
                if (recData.cards[i].Value == gameData.HandCards[j].Value &&
                    recData.cards[i].Suit == gameData.HandCards[j].Suit)
                {
                    gameData.HandCards.RemoveAt(j);
                    break;//找到了就跳出
                }
            }
        }
        gameData.CurrentDiscards.Clear();//清除上次的
        gameData.CurrentDiscards.AddRange(recData.cards);//保存出牌
        dispatcher.Dispatch(ViewConst.ShowDiscards, recData.cards);//显示出牌
        string cardType = gameData.GetCardsType(recData.cards);//获得牌型
        dispatcher.Dispatch(ViewConst.ShowDiscardMsg, cardType);//显示牌型
        if (recData.player.Name.Equals(gameData.PlayerSelf.Name))
        {
            dispatcher.Dispatch(ViewConst.RemoveAllDiscards);//清除自己要出的牌
        }
        if (recData.player.Name.Equals(gameData.Player2.Name))
        {
            dispatcher.Dispatch(ViewConst.UpdatePlayer2Cards, -recData.cards.Length);//更新玩家2牌数
        }
        if (recData.player.Name.Equals(gameData.Player3.Name))
        {
            dispatcher.Dispatch(ViewConst.UpdatePlayer3Cards, -recData.cards.Length);//更新玩家3牌数
        }
    }
    
}
