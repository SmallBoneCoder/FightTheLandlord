using UnityEngine;

using strange.extensions.command.impl;

public class Player2Command : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    public override void Execute()
    {
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        Debug.Log("Player2:" + recData.player.Name);
        gameData.Players.Add(recData.player);
    }
}