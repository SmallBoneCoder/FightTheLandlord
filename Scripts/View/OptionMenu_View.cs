using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public enum OptionMenu_Status
{
    CallLandlord=1,
    ClaimLandlord=2,
    FightLandlord=3,
    HideAll=4
}
public class OptionMenu_View : EventView {
    private Transform GameIn; //打牌中
    private Transform GameBefore;//打牌前
    private Button Btn_Pass;//不出
    private Button Btn_Tip;//提示
    private Button Btn_Discard;//出牌
    private Button Btn_Reset;//重选
    private Text GameInTimer_Txt;//游戏中时间显示
    private Button Btn_CallLandlord;//叫地主
    private Button Btn_NotCall;//不叫
    private Button Btn_Claim;//抢地主
    private Button Btn_NotClaim;//不抢
    private Text GameBeforeTimer_Txt;//叫地主时间显示
    //private GameObject EventSystem;//文件系统
    public void Init()
    {
        GameIn = transform.Find("GameIn");
        Btn_Pass = GameIn.Find("Btn_Pass").GetComponent<Button>();
        Btn_Pass.onClick.AddListener(Click_BtnPass);
        Btn_Tip = GameIn.Find("Btn_Tip").GetComponent<Button>();
        Btn_Tip.onClick.AddListener(Click_BtnTip);
        Btn_Discard = GameIn.Find("Btn_Discard").GetComponent<Button>();
        Btn_Discard.onClick.AddListener(Click_BtnDiscard);
        Btn_Reset = GameIn.Find("Btn_Reset").GetComponent<Button>();
        Btn_Reset.onClick.AddListener(Click_BtnReset);
        GameInTimer_Txt = GameIn.Find("GameInTimer_Txt").GetComponent<Text>();
        //----------------------------//
        GameBefore = transform.Find("GameBefore");
        Btn_CallLandlord = GameBefore.Find("Btn_CallLandlord").GetComponent<Button>();
        Btn_CallLandlord.onClick.AddListener(Click_BtnCallLandlord);
        Btn_NotCall = GameBefore.Find("Btn_NotCall").GetComponent<Button>();
        Btn_NotCall.onClick.AddListener(Click_BtnNotCall);
        Btn_Claim = GameBefore.Find("Btn_Claim").GetComponent<Button>();
        Btn_Claim.onClick.AddListener(Click_BtnClaim);
        Btn_NotClaim = GameBefore.Find("Btn_NotClaim").GetComponent<Button>();
        Btn_NotClaim.onClick.AddListener(Click_BtnNotClaim);
        GameBeforeTimer_Txt = GameBefore.Find("GameBeforeTimer_Txt").GetComponent<Text>();
        //
        ChangeMode(OptionMenu_Status.HideAll);
    }


    public void ChangeMode(OptionMenu_Status mode)
    {
        switch (mode)
        {
            case OptionMenu_Status.CallLandlord://叫地主
                {
                    GameIn.gameObject.SetActive(false);
                    GameBefore.gameObject.SetActive(true);
                    Btn_CallLandlord.gameObject.SetActive(true);
                    Btn_NotCall.gameObject.SetActive(true);
                    Btn_Claim.gameObject.SetActive(false);
                    Btn_NotClaim.gameObject.SetActive(false);
                }
                break;
            case OptionMenu_Status.ClaimLandlord://抢地主
                {
                    GameIn.gameObject.SetActive(false);
                    GameBefore.gameObject.SetActive(true);
                    Btn_CallLandlord.gameObject.SetActive(false);
                    Btn_NotCall.gameObject.SetActive(false);
                    Btn_Claim.gameObject.SetActive(true);
                    Btn_NotClaim.gameObject.SetActive(true);
                }
                break;
            case OptionMenu_Status.FightLandlord://斗地主，游戏中
                {
                    GameIn.gameObject.SetActive(true);
                    GameBefore.gameObject.SetActive(false);
                }
                break;
            case OptionMenu_Status.HideAll://隐藏所有
                {
                    GameIn.gameObject.SetActive(false);
                    GameBefore.gameObject.SetActive(false);
                }
                break;
        }
    }
    public void SwitchTimer(bool on_off,bool before_in)
    {
        if (on_off)
        {
            StopAllCoroutines();
            if(before_in)StartCoroutine(ShowTime_Before());
            else StartCoroutine(ShowTime_In());
        }
        else
        {
            StopAllCoroutines();
            GameBeforeTimer_Txt.text = "";
            GameInTimer_Txt.text = "";
        }
    }
    private IEnumerator ShowTime_Before()
    {
        int time = 30;
        while (time > 0)
        {
            GameBeforeTimer_Txt.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        GameBeforeTimer_Txt.text = "";
        dispatcher.Dispatch(ViewConst.TimeOut);//超时
        //ChangeMode(OptionMenu_Status.HideAll);//隐藏界面
    }
    private IEnumerator ShowTime_In()
    {
        int time = 30;
        while (time > 0)
        {
            GameInTimer_Txt.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        GameInTimer_Txt.text = "";
        dispatcher.Dispatch(ViewConst.TimeOut);//超时
        ChangeMode(OptionMenu_Status.HideAll);//隐藏界面
    }

    //----------------------------//
    private void Click_BtnPass()
    {
        dispatcher.Dispatch(ViewConst.Click_Pass);
    }
    private void Click_BtnTip()
    {
        dispatcher.Dispatch(ViewConst.Click_Tip);
    }
    private void Click_BtnDiscard()
    {
        dispatcher.Dispatch(ViewConst.Click_Discard);
    }
    private void Click_BtnReset()
    {
        dispatcher.Dispatch(ViewConst.Click_Reset);
    }
    private void Click_BtnCallLandlord()
    {
        dispatcher.Dispatch(ViewConst.Click_CallLandlord);
        //ChangeMode(OptionMenu_Status.ClaimLandlord);
    }
    private void Click_BtnNotCall()
    {
        dispatcher.Dispatch(ViewConst.Click_NotCall);
    }
    private void Click_BtnClaim()
    {
        dispatcher.Dispatch(ViewConst.Click_Claim);
        //ChangeMode(OptionMenu_Status.FightLandlord);
    }
    private void Click_BtnNotClaim()
    {
        dispatcher.Dispatch(ViewConst.Click_NotClaim);
    }

}
