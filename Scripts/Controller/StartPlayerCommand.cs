using UnityEngine;

using strange.extensions.command.impl;

public class StartPlayerCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
       
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        Debug.Log("StartPlayer:"+ recData.player.Name);
        gameData.Players.Add(recData.player);
    }
}