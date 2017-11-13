using System;
using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections.Generic;

public class LocalService_View : EventView
{

    public int CurrentTurn;
    public string Landlord;
    public List<string> RoomMembers;
    public int ClaimCount;
    public Dictionary<string, List<Card>> ComputerHandCards;
    public List<Card> BaseCards;
    public List<Card> DisCards;
    public Queue<RemoteCMD_Data> MessageBox;
    [Inject]
    public IGameData gameData { get; set; }
    public void Init()
    {
        MessageBox = new Queue<RemoteCMD_Data>();
        RoomMembers = new List<string>();
        DisCards = new List<Card>();
        ComputerHandCards = new Dictionary<string, List<Card>>();
        ClaimCount = 4;
        CurrentTurn = 0;
        BaseCards = new List<Card>();
        Landlord = "";
        Debug.Log("Local Service is Init...");
    }
    public void SendDataToLocal(RemoteCMD_Data recData)
    {
        //HandleRecievedData(recData);
        MessageBox.Enqueue(recData);
    }
    private void Update()
    {
        if (MessageBox.Count > 0)
        {
            RemoteCMD_Data recData = MessageBox.Dequeue();
            HandleRecievedData(recData);
        }
    }
    private void HandleRecievedData(RemoteCMD_Data recData)
    {
        
        switch (recData.cmd)
        {
            case RemoteCMD_Const.Match:
                {
                    HandleMatch(recData.player);
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
            case RemoteCMD_Const.GenerateCards:
                {
                    HandleGenerateCards((Card[])recData.cards.Clone());
                }
                break;
            case RemoteCMD_Const.Discards:
                {
                    HandleDiscards(recData);
                }
                break;
            case RemoteCMD_Const.Pass:
                {
                    HandlePass(recData);
                }
                break;
            case RemoteCMD_Const.GamerOver:
                {
                    HandleGameOver(recData);
                }
                break;
           
        }
    }

    private void HandleMatch(PlayerInfo p)
    {

        Debug.Log("房间满员，开始游戏");
        RoomMembers.Add(p.Name);
        RoomMembers.Add(StringConst.Computer1);
        RoomMembers.Add(StringConst.Computer2);
        //确定玩家顺序
        int startPlayer = 0;//从玩家起始
        CurrentTurn = startPlayer;
        int player2 = (startPlayer + 1) % 3;
        int player3 = (startPlayer + 2) % 3;
        //发送匹配成功命令
        RemoteCMD_Data rec = new RemoteCMD_Data();
        rec.cmd = RemoteCMD_Const.MatchSuccess;
        dispatcher.Dispatch(ServiceConst.Service_MatchSuccess,rec);
        //将顺序转发给房间内的玩家
        rec.cmd = RemoteCMD_Const.StartPlayer;
        rec.player.Name = RoomMembers[startPlayer];
        dispatcher.Dispatch(ServiceConst.Service_StartPlayer, rec);
        //
        rec = new RemoteCMD_Data();
        rec.cmd = RemoteCMD_Const.Player2;
        rec.player.Name = RoomMembers[player2];
        dispatcher.Dispatch(ServiceConst.Service_Player2, rec);
        //
        rec = new RemoteCMD_Data();
        rec.cmd = RemoteCMD_Const.Player3;
        rec.player.Name = RoomMembers[player3];
        dispatcher.Dispatch(ServiceConst.Service_Player3, rec);

    }
    private void HandleGenerateCards(Card[] cards)
    {
        Debug.Log("开始发牌...");
        Card[] cards1 = new Card[17];
        Array.Copy(cards, 0, cards1, 0, 17);
        Card[] cards2 = new Card[17];
        Array.Copy(cards, 17, cards2, 0, 17);
        Card[] cards3 = new Card[17];
        Array.Copy(cards, 34, cards3, 0, 17);
        Card[] baseCards = new Card[3];
        Array.Copy(cards, 51, baseCards, 0, 3);
        //手牌1
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.DealCards;
        recData.cards = (Card[])cards1.Clone();
        dispatcher.Dispatch(ServiceConst.Service_DealCards, recData);
        //手牌2

        ComputerHandCards.Add( StringConst.Computer1,new List<Card>((Card[])cards2.Clone()));
        //手牌3

        ComputerHandCards.Add(StringConst.Computer2, new List<Card>((Card[])cards3.Clone()));

        //底牌
        BaseCards = new List<Card>((Card[])baseCards.Clone());
        RemoteCMD_Data rec = new RemoteCMD_Data();
        rec.cmd = RemoteCMD_Const.BaseCards;
        rec.cards = baseCards;
        dispatcher.Dispatch(ServiceConst.Service_BaseCards, rec);
        //开始游戏
        SendTurn();
    }

    private void HandleCallLandlord(RemoteCMD_Data recData)
    {
        Console.WriteLine("玩家" + recData.player.Name + "叫地主");
        dispatcher.Dispatch(ServiceConst.Service_CallLandlord, new RemoteCMD_Data()
        {
            player=new PlayerInfo() { Name= recData.player.Name },
            cmd= RemoteCMD_Const.CallLandlord
        });
        DecideLandlord(recData.player.Name, true);
        SendTurn();
    }

    private void HandleNotCall(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "不叫地主");
        dispatcher.Dispatch(ServiceConst.Service_NotCall, new RemoteCMD_Data()
        {
            player = new PlayerInfo() { Name = recData.player.Name },
            cmd = RemoteCMD_Const.NotCall
        });
        DecideLandlord(recData.player.Name, false);
        SendTurn();
    }

    private void HandleClaim(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "抢地主");
        dispatcher.Dispatch(ServiceConst.Service_Claim, new RemoteCMD_Data()
        {
            player = new PlayerInfo() { Name = recData.player.Name },
            cmd = RemoteCMD_Const.Claim
        });
        DecideLandlord(recData.player.Name, true);
        SendTurn();
    }

    private void HandleNotClaim(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "不抢地主");
        dispatcher.Dispatch(ServiceConst.Service_NotClaim, new RemoteCMD_Data()
        {
            player = new PlayerInfo() { Name = recData.player.Name },
            cmd = RemoteCMD_Const.NotClaim
        });
        DecideLandlord(recData.player.Name,false);
        SendTurn();
    }

    private void DecideLandlord(string name,bool update)
    {
        if (update)
        {
            Landlord = name;//更新地主
        }
        ClaimCount--;
        if (ClaimCount <= 0)
        {
            Debug.Log("地主是:"+Landlord);
            CurrentTurn =RoomMembers.IndexOf(Landlord);//起始玩家改为地主
            if (ComputerHandCards.ContainsKey(Landlord))//电脑是地主
            {
                ComputerHandCards[Landlord].AddRange(BaseCards);//把底牌加入
            }
        }
    }

    private void HandleDiscards(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "出牌");
        dispatcher.Dispatch(ServiceConst.Service_Discard, recData);
        SendTurn();
    }

    private void HandlePass(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "跳过");
        dispatcher.Dispatch(ServiceConst.Service_Pass, recData);
        SendTurn();
    }

    private void HandleGameOver(RemoteCMD_Data recData)
    {
        Debug.Log("玩家" + recData.player.Name + "获得胜利，游戏结束");
        dispatcher.Dispatch(ServiceConst.Service_Gameover, recData);
    }
    private void SendTurn()
    {
        RemoteCMD_Data recData = new RemoteCMD_Data();
        recData.cmd = RemoteCMD_Const.GameTurn;
        Debug.Log("Local:"+CurrentTurn);
        int index = CurrentTurn;//当前回合索引
        recData.player.Name = RoomMembers[index];//当前玩家昵称
              //string targetIP = roomManager[player_Room[ipname]].Members[index];//IP
        dispatcher.Dispatch(ServiceConst.Service_GameTurn,recData);//转发回合
        if (!RoomMembers[CurrentTurn].Equals(RoomMembers[0]))//不是玩家的回合
        {
            HandleComputerTurn();//处理电脑的操作
        }
        
        CurrentTurn++;//当前回合数增加
        CurrentTurn %= 3;

    }

    private void HandleComputerTurn()
    {
        if (ClaimCount > 0)//争夺地主阶段
        {
            if (string.IsNullOrEmpty(Landlord))//玩家未抢地主
            {
                SendDataToLocal(new RemoteCMD_Data()
                {
                    cmd = RemoteCMD_Const.Claim,
                    player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] }
                });
                return;
            }
            SendDataToLocal(new RemoteCMD_Data()//玩家抢了就不抢了
            {
                cmd = RemoteCMD_Const.NotClaim,
                player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] }
            });
            return;
        }
        //出牌阶段

        if(gameData.LastPlayer== RoomMembers[CurrentTurn]|| //上次是自己
            gameData.CurrentDiscards.Count<=0)//自己第一个出牌
        {
            List<Card> disCards = new List<Card>();
            //disCards.Add(ComputerHandCards[RoomMembers[CurrentTurn]][0]);//放水，出单张
            //ComputerHandCards[RoomMembers[CurrentTurn]].RemoveAt(0);
            disCards=gameData.GetEnabledDisCards(ComputerHandCards[RoomMembers[CurrentTurn]], RoomMembers[CurrentTurn]);//获取出牌
            SendDataToLocal(new RemoteCMD_Data()//出牌
            {
                cmd = RemoteCMD_Const.Discards,
                player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] },
                cards = disCards.ToArray()

            });
        }
        else
        {
            List<Card> disCards = gameData.
            GetEnabledDisCards(ComputerHandCards[RoomMembers[CurrentTurn]], RoomMembers[CurrentTurn]);//获取出牌
            if (gameData.CardsCompareToDicards(disCards.ToArray()))//再比一次大小
            {
                SendDataToLocal(new RemoteCMD_Data()//出牌
                {
                    cmd = RemoteCMD_Const.Discards,
                    player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] },
                    cards = disCards.ToArray()

                });
            }
            else
            {
                SendDataToLocal(new RemoteCMD_Data()//跳过
                {
                    cmd = RemoteCMD_Const.Pass,
                    player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] },
                });
            }
        }
        //出完牌后判断胜负
        if (ComputerHandCards[RoomMembers[CurrentTurn]].Count <= 0)
        {
            SendDataToLocal(new RemoteCMD_Data()//电脑胜利
            {
                cmd = RemoteCMD_Const.GamerOver,
                player = new PlayerInfo() { Name = RoomMembers[CurrentTurn] },
            });
        }
    }

    
}