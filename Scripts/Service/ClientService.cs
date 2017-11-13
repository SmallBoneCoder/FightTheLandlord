using UnityEngine;

using strange.extensions.dispatcher.eventdispatcher.api;
using TestClient;
using LitJson;
using System;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System.Collections;

public class ClientService : EventView
{
    public Client _client;
    public void Init()
    {
        _client = new Client(ServiceConst.IPAddress_Server, ServiceConst.Port_Server);
        _client.StartConnect();
    }
    public void UpdateIPAndPort(string ip,string port)
    {
        ServiceConst.IPAddress_Server = ip;
        ServiceConst.Port_Server = port;
        _client.InitClient(ip, port);
        _client.StartConnect();
    }
    private void FixedUpdate()
    {
        //Debug.Log(_client);
        if (_client.MessageBox.Count > 0)
        {
            string data = "";
            lock (_client.MessageBox)
            {
                data = _client.MessageBox.Dequeue();//取数据
            }
            Debug.Log(data);
            RemoteCMD_Data recData = JsonMapper.ToObject<RemoteCMD_Data>(data);
            switch (recData.cmd)
            {
                case RemoteCMD_Const.MatchSuccess:
                    {
                        HandleMatchSuccess(recData);
                    }break;
                case RemoteCMD_Const.DealCards:
                    {
                        HandleDealCards(recData);
                    }
                    break;
                case RemoteCMD_Const.BaseCards:
                    {
                        HandleBaseCards(recData);
                    }
                    break;
                case RemoteCMD_Const.StartPlayer:
                    {
                        HandleStartPlayer(recData);
                    }
                    break;
                case RemoteCMD_Const.Player2:
                    {
                        HandlePlayer2(recData);
                    }
                    break;
                case RemoteCMD_Const.Player3:
                    {
                        HandlePlayer3(recData);
                    }
                    break;
                case RemoteCMD_Const.Pass:
                    {
                        HandlePass(recData);
                    }
                    break;
                case RemoteCMD_Const.Discards:
                    {
                        HandleDiscards(recData);
                    }
                    break;
                case RemoteCMD_Const.CallLandlord:
                    {
                        HandleCallLandlord(recData);
                    }
                    break;
                case RemoteCMD_Const.NotCall:
                    {
                        HandleNotCall(recData);
                    }
                    break;
                case RemoteCMD_Const.Claim:
                    {
                        HandleClaim(recData);
                    }
                    break;
                case RemoteCMD_Const.NotClaim:
                    {
                        HandleNotClaim(recData);
                    }
                    break;
                case RemoteCMD_Const.GameTurn:
                    {
                        HandleGameTurn(recData);
                    }
                    break;
                case RemoteCMD_Const.GamerOver:
                    {
                        HandleGameOver(recData);
                    }
                    break;
            }
        }
    }

    private void HandleNotClaim(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_NotClaim,recData);
    }

    private void HandleGameOver(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Gameover, recData);
    }

    private void HandleMatchSuccess(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_MatchSuccess);
    }
    private void HandleDealCards(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_DealCards,recData);
    }
    private void HandleBaseCards(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_BaseCards, recData);
    }
    private void HandleStartPlayer(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_StartPlayer, recData);
    }
    private void HandlePlayer2(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Player2, recData);
    }
    private void HandlePlayer3(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Player3, recData);
    }
    private void HandlePass(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Pass, recData);
    }
    private void HandleDiscards(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Discard, recData);
    }
    private void HandleCallLandlord(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_CallLandlord, recData);
    }
    private void HandleNotCall(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_NotCall, recData);
    }
    private void HandleClaim(RemoteCMD_Data recData)
    {
        dispatcher.Dispatch(ServiceConst.Service_Claim, recData);
    }
    private void HandleGameTurn(RemoteCMD_Data recData )
    {
        dispatcher.Dispatch(ServiceConst.Service_GameTurn, recData);
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        //关闭后台服务
        _client.CloseClient();
    }
}
//----------------------------------------------//
//通讯过程
//1.客户端向服务器发送Match(匹配)命令，界面跳转到匹配等待界面
//2.服务器处理匹配信息，房间满了就发送MatchSuccess(匹配成功)命令
//3.客户端接收MatchSuccess命令，界面跳转到游戏加载界面
//4.服务器向确定玩家的起始顺序并发送StartPlayer、Player2、Player3命令
//5.客户端接收StartPlayer、Player2、Player3命令初始化玩家位置和信息
//6.客户端所在玩家为StartPlayer的客户端生成一副牌并发送给服务器，GenerateCards(生成卡牌)
//7.服务器接收GenerateCards(生成卡牌)命令处理卡牌信息，并将各自的牌和底牌发送给客户端
//8.客户端接收DealCards(发牌)命令、BaseCards(底牌)命令来接收自己的牌和底牌，并显示游戏界面
//9.服务器向当前行动客户端发送GameTurn(行动)命令，并记录行动顺序
//10.客户端接收GameTurn(行动)命令，根据当前游戏状态显示操控界面（叫地主）
//11.服务器根据客户端发送的争夺地主命令（CallLandlord、NotCall、Claim、NotClaim）来确定最后的地主
//12.客户端根据接收的争夺地主命令（CallLandlord、NotCall、Claim、NotClaim）来显示界面和更新游戏状态
//13.服务器确认完地主后更新行动顺序（地主先出），给地主发送GameTurn(行动)命令
//14.客户端接收GameTurn(行动)命令后根据游戏状态判断为开始游戏后发送Discards(出牌)命令和出牌信息
//15.服务器接收Discards(出牌)、Pass(跳过)命令后转发给所有客户端，并更新行动顺序
//16.客户端收到Discards(出牌)、Pass(跳过)命令后，更新出牌数据，并更新UI显示出牌
//17.客户端牌出完以后发送GameOver(游戏结束)命令
//18.服务器接收GameOver(游戏结束)命令并转发给所有客户端，随后释放房间信息
//19.客户端接收GameOver(游戏结束)命令，根据所剩下手牌判断胜负并显示界面