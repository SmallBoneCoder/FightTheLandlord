using UnityEngine;

using strange.extensions.command.impl;

public class UpdateTimerCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        bool isGameIn = (bool)evt.data;
        string player1 = gameData.PlayerSelf.Name,
            player2 = gameData.Player2.Name,
            player3 = gameData.Player3.Name;
        //关掉当前的
        if (gameData.Players[gameData.CurrentTurn].Name.Equals(player1))//这一回合是自己
        {
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player1, 
                isGameIn? Player1TimerStatus.GameIn_On: Player1TimerStatus.GameBefore_On);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player2, false);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player3, false);
        }
        if (gameData.Players[gameData.CurrentTurn].Name.Equals(player2))//这一回合是player2
        {
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player1,
               isGameIn ? Player1TimerStatus.GameIn_Off : Player1TimerStatus.GameBefore_Off);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player2, true);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player3, false);
        }
        if (gameData.Players[gameData.CurrentTurn].Name.Equals(player3))//这一回合是player3
        {
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player1,
               isGameIn ? Player1TimerStatus.GameIn_Off : Player1TimerStatus.GameBefore_Off);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player2, false);
            dispatcher.Dispatch(ViewConst.SwitchTimer_Player3, true);
        }
        ////打开下一次的
        //if (gameData.Players[gameData.CurrentTurn].Name.Equals(player1))//下一回合是自己
        //{
        //    dispatcher.Dispatch(ViewConst.SwitchTimer_Player1,
        //        isGameIn ? Player1TimerStatus.GameIn_On : Player1TimerStatus.GameBefore_On);
        //}
        //if (gameData.Players[gameData.CurrentTurn].Name.Equals(player2))//下一回合是player2
        //{
        //    dispatcher.Dispatch(ViewConst.SwitchTimer_Player2, true);
        //}
        //if (gameData.Players[gameData.CurrentTurn].Name.Equals(player3))//下一回合是player3
        //{
        //    dispatcher.Dispatch(ViewConst.SwitchTimer_Player3, true);
        //}
    }
}