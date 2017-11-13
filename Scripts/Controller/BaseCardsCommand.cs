
using strange.extensions.command.impl;
using UnityEngine;

public class BaseCardsCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        Debug.Log("BaseCards:"+LitJson.JsonMapper.ToJson(recData));
        Card[] baseCard = new Card[recData.cards.Length];
        recData.cards.CopyTo(baseCard,0);
        gameData.BaseCards.AddRange(baseCard);//保存底牌数据
        dispatcher.Dispatch(ViewConst.ShowBaseCards_Back);//显示底牌背面
    }
}