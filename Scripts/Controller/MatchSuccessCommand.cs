using UnityEngine;

using strange.extensions.command.impl;

public class MatchSuccessCommand : EventCommand
{
    [Inject]
    public IClientService clientService { get; set; }
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        Debug.Log("Match Success");
        //显示加载游戏中UI
        dispatcher.Dispatch(ViewConst.ShowMatchSuccessMsg);
    }
}