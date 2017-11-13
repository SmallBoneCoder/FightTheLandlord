using UnityEngine;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

public class GameLobbyUI_View : EventView
{
    private GameObject PlayerInfo_Panel;//人物信息
    private GameObject Matching_Panel;//匹配等待界面
    private GameObject Setting_Panel;//设置面板
    private Button Btn_Match;//匹配
    private Button Btn_PVE;//人机
    private Button Btn_Quit;//退出
    private Button Btn_Cancel;//取消匹配
    private Button Btn_OK;//确认按钮
    private Button Btn_Setting;//设置按钮
    private Text Name_Txt;//昵称
    private Text Grade_Txt;//分数
    private Image Role_IMG;//头像
    private Text Msg_Txt;//匹配消息
    private InputField Input_IP;//IP
    private InputField Input_Port;//Port
    public void Init()
    {
        Btn_Match = transform.Find("Btn_Match").GetComponent<Button>();
        Btn_Match.onClick.AddListener(Click_BtnMatch);//匹配
        Btn_PVE = transform.Find("Btn_PVE").GetComponent<Button>();
        Btn_PVE.onClick.AddListener(Click_BtnPVE); //人机
        Btn_Quit = transform.Find("Btn_Quit").GetComponent<Button>();
        Btn_Quit.onClick.AddListener(Click_BtnQuit);//退出
        Btn_Setting = transform.Find("Btn_Setting").GetComponent<Button>();
        Btn_Setting.onClick.AddListener(Click_Setting);//设置
        //
        PlayerInfo_Panel = transform.Find("PlayerInfo_Panel").gameObject;
        Name_Txt = PlayerInfo_Panel.transform.Find("Name_Txt").GetComponent<Text>();
        Grade_Txt = PlayerInfo_Panel.transform.Find("Grade_Txt").GetComponent<Text>();
        Role_IMG = PlayerInfo_Panel.transform.Find("Role_IMG").GetComponent<Image>();
        //
        Matching_Panel = transform.Find("Matching_Panel").gameObject;
        Msg_Txt= Matching_Panel.transform.Find("Msg_Txt").GetComponent<Text>();
        Btn_Cancel = Matching_Panel.transform.Find("Btn_Cancel").GetComponent<Button>();
        Btn_Cancel.onClick.AddListener(Click_BtnCancel);//取消匹配
        //
        Setting_Panel= transform.Find("Setting_Panel").gameObject;
        Input_IP = Setting_Panel.transform.Find("Input_IP").GetComponent<InputField>();
        Input_Port = Setting_Panel.transform.Find("Input_Port").GetComponent<InputField>();
        Btn_OK = Setting_Panel.transform.Find("Btn_Ok").GetComponent<Button>();
        Btn_OK.onClick.AddListener(Click_OK);//确认
        //
        HideMatchPanel();//先隐藏
        Msg_Txt.text = StringConst.Matching;
    }
    public void UpdatePlayerSelfName(string name)
    {
        Name_Txt.text = name;
    }
    public void ShowMatchSuccessMsg()
    {
        Msg_Txt.text = StringConst.MatchSuccess;
        Btn_Cancel.interactable = false;
    }
    public void UpdatePlayerSelfGrade(int grade)
    {
        Grade_Txt.text = grade.ToString();
    }
    public void UpdatePlayerSelfRoleIMG(Sprite img)
    {
        Role_IMG.sprite = img;
    }
    public void ShowMatchPanel()
    {
        Matching_Panel.SetActive(true);
    }
    public void HideMatchPanel()
    {
        Matching_Panel.SetActive(false);
    }
    public void HideSettingPanel()
    {
        Setting_Panel.SetActive(false);
    }
    private void Click_BtnMatch()
    {
        Msg_Txt.text = StringConst.Matching;
        Btn_Cancel.interactable = true;
        dispatcher.Dispatch(ViewConst.Click_Match);
    }
    private void Click_BtnPVE()
    {
        dispatcher.Dispatch(ViewConst.Click_PVE);
    }
    private void Click_BtnQuit()
    {
        Application.Quit();
    }
    private void Click_Setting()
    {
        Setting_Panel.SetActive(true);
        Input_IP.text = ServiceConst.IPAddress_Server;
        Input_Port.text = ServiceConst.Port_Server;
    }
    private void Click_OK()
    {
        IPAndPort ip = new IPAndPort();
        ip.IPAddr = Input_IP.text;
        ip.Port = Input_Port.text;
        dispatcher.Dispatch(ServiceConst.UpdateIPAndPort,ip);
        HideSettingPanel();
    }
    private void Click_BtnCancel()
    {
        dispatcher.Dispatch(ViewConst.Click_CancelMatch);
    }
}
public struct IPAndPort{
    public string IPAddr;
    public string Port;
}