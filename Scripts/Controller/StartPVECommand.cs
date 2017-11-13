using UnityEngine;
using strange.extensions.command.impl;

public class StartPVECommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        gameData.CurrentMode = GameMode.SinglePlayer;//模式改为单机
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Match;
        recData.player = gameData.PlayerSelf;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//开始单机模式
    }
}