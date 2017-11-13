using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class GameOverCommand : EventCommand
{
    [Inject]
    public IGameData gameData { get; set; }
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }
    public override void Execute()
    {
        RemoteCMD_Data recData = evt.data as RemoteCMD_Data;
        string result = "";
        if (recData.player.Name.Equals(gameData.Landlord))//赢的是地主
        {
            result = StringConst.Lose;
            if (recData.player.Name.Equals(gameData.PlayerSelf.Name))//自己是地主
            {
                result = StringConst.Win;
            }
            else//不是地主
            {
                result = StringConst.Lose;
            }
        }
        else//赢的是农民
        {
            if (gameData.PlayerSelf.Name.Equals(gameData.Landlord))//自己是地主
            {
                result = StringConst.Lose;
            }
            else//自己是农民，获胜
            {
                result = StringConst.Win;
            }
        }
            
        
        Debug.Log("ShowGameOverUICommand: The Winner is:" + recData.player.Name);
        Transform canvas = ContextView.transform.Find("Canvas");
        GameObject GameOverUI = canvas.Find("GameOverUI(Clone)").gameObject;
        GameOverUI.transform.SetSiblingIndex(canvas.childCount - 1);//显示在最前面
        //显示输赢信息
        dispatcher.Dispatch(ViewConst.UpdateGameOverResult, result);
        //关掉所有计时器
        dispatcher.Dispatch(ViewConst.SwitchTimer_Player1,Player1TimerStatus.GameIn_Off );
        dispatcher.Dispatch(ViewConst.SwitchTimer_Player2, false);
        dispatcher.Dispatch(ViewConst.SwitchTimer_Player3, false);
        gameData.Init();//初始化数据
        GameObject.Find("PVEController").GetComponent<LocalService_View>().Init();
    }
}