using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Player1TimerStatus{
    GameIn_On=1,
    GameIn_Off=2,
    GameBefore_On = 3,
    GameBefore_Off = 4
}
public class OptionMenu_Mediator : EventMediator {
    [Inject]
	public OptionMenu_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        //
        //dispatcher.AddListener(ViewConst.GetDiscards, CallBack_GetDiscards);
        dispatcher.AddListener(ViewConst.DiscardSuccess, CallBack_DiscardSuccess);
        dispatcher.AddListener(ViewConst.DiscardFail, CallBack_DiscardFail);
        dispatcher.AddListener(ViewConst.ChangeOptionMenuMode, CallBakc_ChangeOptionMenuMode);
        dispatcher.AddListener(ViewConst.SwitchTimer_Player1, CallBack_SwitchTimer_Player1);
        //
        View.dispatcher.AddListener(ViewConst.Click_NotClaim, CallBack_NotClaim);
        View.dispatcher.AddListener(ViewConst.Click_Claim, CallBack_Claim);
        View.dispatcher.AddListener(ViewConst.Click_Discard, CallBack_Discard);
        View.dispatcher.AddListener(ViewConst.Click_CallLandlord, CallBack_CallLandlord);
        View.dispatcher.AddListener(ViewConst.Click_NotCall, CallBack_NotCall);
        View.dispatcher.AddListener(ViewConst.Click_Reset, CallBack_Reset);
        View.dispatcher.AddListener(ViewConst.Click_Pass, CallBack_Pass);
        View.dispatcher.AddListener(ViewConst.Click_Tip, CallBack_Tip);
        View.dispatcher.AddListener(ViewConst.TimeOut, CallBack_TimeOut);
    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.DiscardSuccess, CallBack_DiscardSuccess);
        dispatcher.RemoveListener(ViewConst.DiscardFail, CallBack_DiscardFail);
        dispatcher.RemoveListener(ViewConst.ChangeOptionMenuMode, CallBakc_ChangeOptionMenuMode);
        dispatcher.RemoveListener(ViewConst.SwitchTimer_Player1, CallBack_SwitchTimer_Player1);
        //
        View.dispatcher.RemoveListener(ViewConst.Click_NotClaim, CallBack_NotClaim);
        View.dispatcher.RemoveListener(ViewConst.Click_Claim, CallBack_Claim);
        View.dispatcher.RemoveListener(ViewConst.Click_Discard, CallBack_Discard);
        View.dispatcher.RemoveListener(ViewConst.Click_CallLandlord, CallBack_CallLandlord);
        View.dispatcher.RemoveListener(ViewConst.Click_NotCall, CallBack_NotCall);
        View.dispatcher.RemoveListener(ViewConst.Click_Reset, CallBack_Reset);
        View.dispatcher.RemoveListener(ViewConst.Click_Pass, CallBack_Pass);
        View.dispatcher.RemoveListener(ViewConst.Click_Tip, CallBack_Tip);
        View.dispatcher.RemoveListener(ViewConst.TimeOut, CallBack_TimeOut);
    }
    private void CallBack_TimeOut(IEvent evt)
    {
        dispatcher.Dispatch(ViewConst.Click_Reset);//超时直接跳过
        CallBack_Pass();//发送跳过命令
    }
    private void CallBack_SwitchTimer_Player1(IEvent evt)
    {
        switch ((Player1TimerStatus)evt.data)
        {
            case Player1TimerStatus.GameIn_On:
                {
                    View.SwitchTimer(true, false);
                }break;
            case Player1TimerStatus.GameIn_Off:
                {
                    View.SwitchTimer(false, false);
                }
                break;
            case Player1TimerStatus.GameBefore_On:
                {
                    View.SwitchTimer(true, true);
                }
                break;
            case Player1TimerStatus.GameBefore_Off:
                {
                    View.SwitchTimer(false, true);
                }
                break;

        }
    }
    private void CallBack_Pass()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Pass;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBack_Reset()
    {
        dispatcher.Dispatch(ViewConst.Click_Reset);
    }
    private void CallBack_Discard()
    {
        //curStatus = OptionMenuStatus.Discard;
        //dispatcher.Dispatch(ViewConst.RequestDiscards);//请求手牌
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Discards;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBack_DiscardSuccess()
    {
        dispatcher.Dispatch(ViewConst.RemoveAllDiscards);//移除要出的牌
        Debug.Log("Discard Success");
    }
    private void CallBack_DiscardFail()
    {
        Debug.Log("Discard Fail");
    }
    private void CallBack_CallLandlord()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.CallLandlord;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBack_NotCall()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.NotCall;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBack_Claim()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.Claim;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBack_NotClaim()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.NotClaim;
        dispatcher.Dispatch(NotificationConst.Noti_SendRecData, recData);//执行出牌命令
    }
    private void CallBakc_ChangeOptionMenuMode(IEvent evt)
    {
        OptionMenu_Status status = (OptionMenu_Status)evt.data;
        View.ChangeMode(status);//改变模式
    }
    private void CallBack_Tip()
    {
        dispatcher.Dispatch(NotificationConst.Noti_Tip);
    }


}
