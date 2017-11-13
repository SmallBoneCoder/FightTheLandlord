using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusMediator : EventMediator {
    [Inject]
	public Status_View View { get; set; }
    public override void OnRegister()
    {
        View.Init();
        dispatcher.AddListener(ViewConst.UpdateStatus_Base, CallBack_UpdateStatus_Base);
        dispatcher.AddListener(ViewConst.UpdateStatus_Mutiple, CallBack_UpdateStatus_Mutiple);
    }
    private void CallBack_UpdateStatus_Base(IEvent evt)
    {
        View.UpdateBase((int)evt.data);
    }
    private void CallBack_UpdateStatus_Mutiple(IEvent evt)
    {
        View.UpdateMutiple((int)evt.data);
    }
    public override void OnRemove()
    {
        dispatcher.RemoveListener(ViewConst.UpdateStatus_Base, CallBack_UpdateStatus_Base);
        dispatcher.RemoveListener(ViewConst.UpdateStatus_Mutiple, CallBack_UpdateStatus_Mutiple);
    }
}
