using UnityEngine;

using strange.extensions.command.impl;

public class SendRecDataCommand : EventCommand
{
    [Inject]
    public IClientService clientService { get; set; }
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        recData.player = gameData.PlayerSelf;
        bool isEnd = false;
        if(recData.cmd== RemoteCMD_Const.Discards)
        {
            recData.cards = gameData.SelectedCards.ToArray();
            Debug.Log("要出的牌:"+LitJson.JsonMapper.ToJson(recData.cards));
            Debug.Log("上次出牌人：" + gameData.LastPlayer);
            if (gameData.GetCardsType(recData.cards)!=CardsType.Mussy &&//不能是杂牌
                (gameData.CardsCompareToDicards(recData.cards)||// 大过上家
                gameData.PlayerSelf.Name.Equals(gameData.LastPlayer)))//上一次是自己
            {
                dispatcher.Dispatch(ViewConst.DiscardSuccess);//出牌成功
                gameData.RestCardNum -= recData.cards.Length;//更新剩余牌数
                if (gameData.RestCardNum <= 0)//牌出完了
                {
                    isEnd = true;//游戏结束
                }
            }
            else
            {
                dispatcher.Dispatch(ViewConst.DiscardFail);
                return;//出牌失败
            }

        }
        gameData.SelectedCards = new System.Collections.Generic.List<Card>();//清空
        clientService.SendDataToServer(new RemoteMsg(recData));//调用服务，发送命令和数据
        if(isEnd)clientService.SendDataToServer(new RemoteMsg(new RemoteCMD_Data()
        {
            cmd = RemoteCMD_Const.GamerOver,
            player = gameData.PlayerSelf
        }));//游戏结束
        dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.HideAll);//回合结束隐藏界面
        
    }
}