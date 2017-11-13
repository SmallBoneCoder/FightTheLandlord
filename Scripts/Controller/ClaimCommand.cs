
using strange.extensions.command.impl;

public class ClaimCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        if (recData.player.Name.Equals(gameData.Player2.Name))
        {
            if (gameData.ClaimCount > 1) dispatcher.Dispatch(ViewConst.ShowPlayer2Msg, StringConst.Claim);
        }
        if (recData.player.Name.Equals(gameData.Player3.Name))
        {
            if (gameData.ClaimCount > 1) dispatcher.Dispatch(ViewConst.ShowPlayer3Msg, StringConst.Claim);
        }
        //if (recData.player.Name.Equals(gameData.PlayerSelf.Name))
        //{
        //    dispatcher.Dispatch(ViewConst.ChangeOptionMenuMode, OptionMenu_Status.HideAll);
        //}
        gameData.Mutiple *= 2;//倍数翻一倍
        gameData.Landlord = recData.player.Name;//地主换位
        gameData.CurrentStatus = GameStatus.Claim;
        gameData.ClaimCount--;//争夺次数减一
        dispatcher.Dispatch(ViewConst.UpdateStatus_Mutiple, gameData.Mutiple);//更新倍数
    }
}