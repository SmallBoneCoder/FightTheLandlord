using UnityEngine;

using strange.extensions.command.impl;

public class PassCommand :EventCommand 
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        Debug.Log("Pass Command is Executed");
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        if (recData.player.Name.Equals(gameData.Player2.Name))
        {
            dispatcher.Dispatch(ViewConst.ShowPlayer2Msg, StringConst.Pass);
        }
        if (recData.player.Name.Equals(gameData.Player3.Name))
        {
            dispatcher.Dispatch(ViewConst.ShowPlayer3Msg, StringConst.Pass);
        }
    }
}