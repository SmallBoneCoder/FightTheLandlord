using UnityEngine;

using strange.extensions.command.impl;

public class GameTurnCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        Debug.Log("ClaimCount:"+gameData.ClaimCount);
        
        RemoteCMD_Data rec = evt.data as RemoteCMD_Data;
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.player.Name = rec.player.Name;
        recData.cmd = rec.cmd;
        Debug.Log(LitJson.JsonMapper.ToJson(recData));
        if (gameData.ClaimCount==1)//只剩下最后一次争夺
        {
            if (gameData.Landlord.Equals(recData.player.Name))//没人抢地主
            {
                Debug.Log("没人抢地主");
                recData.cmd = RemoteCMD_Const.Claim;
                recData.player = gameData.PlayerSelf;
                dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//最后一次如果是自己默认抢一次
                return;
            }
        }
        if (gameData.ClaimCount <= 0)
        {
            gameData.CurrentStatus = GameStatus.FightLandlord;
            int index = gameData.Players.FindIndex(
            (p) => {
                return p.Name == gameData.Landlord;
            });//找到地主所在的顺序
            gameData.CurrentTurn = index;//当前回合变成地主
            dispatcher.Dispatch(ViewConst.ShowBaseCards_Value, gameData.BaseCards.ToArray());//显示底牌
            if (gameData.Landlord.Equals(gameData.PlayerSelf.Name))//自己是地主
            {
                dispatcher.Dispatch(ViewConst.ShowHandCards, gameData.BaseCards.ToArray());//添加底牌
            }
            dispatcher.Dispatch(NotificationConst.Noti_UpdatePlayerIdentity);//更新地主头像
            gameData.ClaimCount = 4;
        }
        if (recData.player.Name.Equals(gameData.PlayerSelf.Name))//当前回合是自己
        {
            switch (gameData.CurrentStatus)
            {
                case GameStatus.CallLandlord:
                    {
                        //显示叫地主
                        dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.CallLandlord);
                    }
                    break;
                case GameStatus.Claim:
                    {
                        //显示抢地主
                        dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.ClaimLandlord);
                    }
                    break;
                case GameStatus.FightLandlord:
                    {
                        //显示斗地主
                        dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.FightLandlord);
                    }
                    break;
            }
        }
        if (recData.player.Name.Equals(gameData.Player2.Name))//当前回合是玩家2
        {
            dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.HideAll);
        }
        if (recData.player.Name.Equals(gameData.Player3.Name))//当前回合是玩家3
        {
            dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.HideAll);
        }
        if(gameData.CurrentStatus== GameStatus.FightLandlord)
        {
            dispatcher.Dispatch(NotificationConst.Noti_UpdatetTimer, true);//更新游戏中的计时器
        }
        else
        {
            dispatcher.Dispatch(NotificationConst.Noti_UpdatetTimer, false);//更新游戏前的计时器
        }

        gameData.CurrentTurn++;
        gameData.CurrentTurn %= 3;//顺序递增
    }
}