using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
namespace GameServer_FightTheLandlord
{
    public class Room
    {
        public List<string> Members { get;}
        public int RoomId { get;}
        public string LandlordIP { get; set; }//地主的IP
        public int CurrentTurn;//当前行动玩家索引
        public int ClaimCount;//争夺地主次数
        public bool IsFull {
            get
            {
                return Members.Count >=Capacity;
            }
        }
        public int Capacity { get;}
        public Room(int capacity,int id)
        {
            Members = new List<string>();
            Capacity = capacity;
            RoomId = id;
            ClaimCount = 4;//最多4次
        }
        public bool DelMember(string name)
        {
            if(Members.Count > 1)
            {
                if (Members.Contains(name)) return false;
                Members.Remove(name);
                return true;
            }
            return false;
        }
        public void ClearMembers()
        {
            Members.Clear();
            ClaimCount = 3;
            LandlordIP = null;
        }
        public bool AddMember(string name)
        {
            if (Members.Count < Capacity)
            {
                if (Members.Contains(name)) return false;
                Members.Add(name);
                return true;
            }
            return false;
        }
    }
    public class RoomManager
    {
        private Queue<int> freeRoomId;
        private Dictionary<int, Room> rooms;
        public RoomManager(int count)
        {
            freeRoomId = new Queue<int>();
            rooms = new Dictionary<int, Room>();
            Init(count);
        }
        public void Init(int count)
        {
            for(int i = 0; i < count; i++)
            {
                rooms.Add(i + 1, new Room(3, i + 1));
                freeRoomId.Enqueue(i + 1);
            }
        }
        public Room GetRoom()
        {
            if (freeRoomId.Count < 1)
            {
                rooms.Add(rooms.Count + 1, new Room(3, rooms.Count + 1));
                return rooms[rooms.Count];
            }
            return rooms[freeRoomId.Dequeue()];
        }
        public void ReleaseRoom(Room r)
        {
            r.ClearMembers();
            freeRoomId.Enqueue(r.RoomId);
        }
        public Room QueryRoom(int id)
        {
            if (rooms.ContainsKey(id))
            {
                return rooms[id];
            }
            return null;
        }
        public Room this[int id]
        {
            get
            {
                if (rooms.ContainsKey(id))
                {
                    return rooms[id];
                }
                return null;
            }
        }
    }


    public class MyGameServer : Server
    {
        private RoomManager roomManager;
        private Dictionary<string, int> player_Room;//玩家的房间号
        private Dictionary<string, string> ipname_PlayerName;//ip--昵称
        private Room curFreeRoom;
        private Random rand;
        private MyParser parser;
        public MyGameServer(string ip, string port) : base(ip, port)
        {
            rand = new Random();
            roomManager = new RoomManager(10);
            player_Room = new Dictionary<string, int>();
            curFreeRoom = roomManager.GetRoom();
            parser = new MyParser();
            ipname_PlayerName = new Dictionary<string, string>();
        }

        public override void HandleRecievedData(string ipname, string data)
        {
            Console.WriteLine(data);
            RemoteCMD_Data recData = JsonMapper.ToObject<RemoteCMD_Data>(data);
            switch (recData.cmd)
            {
                case RemoteCMD_Const.Match:
                    {
                        HandleMatch(ipname, recData.player);
                    } break;
                case RemoteCMD_Const.CallLandlord:
                    {
                        HandleCallLandlord(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.NotCall:
                    {
                        HandleNotCall(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.Claim:
                    {
                        HandleClaim(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.NotClaim:
                    {
                        HandleNotClaim(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.GenerateCards:
                    {
                        HandleGenerateCards(recData.cards, ipname);
                    }
                    break;
                case RemoteCMD_Const.Discards:
                    {
                        HandleDiscards(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.Pass:
                    {
                        HandlePass(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.GamerOver:
                    {
                        HandleGameOver(ipname, recData);
                    }
                    break;
                case RemoteCMD_Const.CancelMatch:
                    {
                        HandleCancelMatch(ipname, recData);
                    }
                    break;
            }
        }

        private void HandleCancelMatch(string ipname, RemoteCMD_Data recData)
        {
            roomManager[player_Room[ipname]].DelMember(ipname);//移除房间中的玩家
            Console.WriteLine("玩家："+ipname_PlayerName[ipname]+"退出匹配");
        }

        public void HandleMatch(string ipname, PlayerInfo p)
        {
            lock (curFreeRoom)
            {
                Console.WriteLine("玩家：" + p.Name + "加入房间,开始匹配");
                curFreeRoom.AddMember(ipname);//加入当前房间
                                              //绑定ip和昵称
                if (!ipname_PlayerName.ContainsKey(ipname))
                {
                    ipname_PlayerName.Add(ipname, p.Name);
                }
                ipname_PlayerName[ipname] = p.Name;
                //p.RoomId curFreeRoom.RoomId;
                //绑定玩家和房间号
                if (!player_Room.ContainsKey(ipname))
                {
                    player_Room.Add(ipname, curFreeRoom.RoomId);
                }
                player_Room[ipname] = curFreeRoom.RoomId;
                //当前房间满了
                if (curFreeRoom.IsFull)
                {
                    Console.WriteLine("房间满员，开始游戏");
                    //确定玩家顺序
                    int startPlayer = rand.Next(0, 3);//随机起始
                    curFreeRoom.CurrentTurn = startPlayer;
                    int player2 = (startPlayer+1) % 3;
                    int player3 = (startPlayer+2) % 3;
                    //发送匹配成功命令
                    RemoteCMD_Data rec = new RemoteCMD_Data();
                    rec.cmd = RemoteCMD_Const.MatchSuccess;
                    TransmitRecCMD(ipname, rec);
                    //将顺序转发给房间内的玩家
                    rec.cmd = RemoteCMD_Const.StartPlayer;
                    rec.player.Name = ipname_PlayerName[curFreeRoom.Members[startPlayer]];
                    TransmitRecCMD(ipname, rec);
                    //
                    rec.cmd = RemoteCMD_Const.Player2;
                    rec.player.Name = ipname_PlayerName[curFreeRoom.Members[player2]];
                    TransmitRecCMD(ipname, rec);
                    //
                    rec.cmd = RemoteCMD_Const.Player3;
                    rec.player.Name = ipname_PlayerName[curFreeRoom.Members[player3]];
                    TransmitRecCMD(ipname, rec);
                    ////新开一个房间
                    curFreeRoom = roomManager.GetRoom();
                }
            }
        }

        public void HandleGenerateCards(Card[] cards, string ipname)
        {
            Console.WriteLine("开始发牌...");
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
            recData.cards = cards1;
            SendRecCMD(roomManager[player_Room[ipname]].Members[0], recData);
            //手牌2
            recData.cmd = RemoteCMD_Const.DealCards;
            recData.cards = cards2;
            SendRecCMD(roomManager[player_Room[ipname]].Members[1], recData);
            //手牌3
            recData.cmd = RemoteCMD_Const.DealCards;
            recData.cards = cards3;
            SendRecCMD(roomManager[player_Room[ipname]].Members[2], recData);
            //底牌
            recData.cmd = RemoteCMD_Const.BaseCards;
            recData.cards = baseCards;
            TransmitRecCMD(ipname, recData);
            //开始游戏
            SendTurn(ipname);
        }

        public void HandleCallLandlord(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家"+recData.player.Name+"叫地主");
            TransmitRecCMD(ipname, recData);
            DecideLandlord(ipname, true);
            SendTurn(ipname);
        }

        public void HandleNotCall(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "不叫地主");
            TransmitRecCMD(ipname, recData);
            DecideLandlord(ipname, false);
            SendTurn(ipname);
        }

        public void HandleClaim(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "抢地主");
            TransmitRecCMD(ipname, recData);
            DecideLandlord(ipname, true);
            SendTurn(ipname);
        }

        public void HandleNotClaim(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "不抢地主");
            TransmitRecCMD(ipname, recData);
            DecideLandlord(ipname, false);
            SendTurn(ipname);
        }

        private void DecideLandlord(string ipname,bool update)
        {
            if(update)
            {
                roomManager[player_Room[ipname]].LandlordIP = ipname;//更新地主
            }
            roomManager[player_Room[ipname]].ClaimCount--;
            if (roomManager[player_Room[ipname]].ClaimCount <= 0)
            {
                roomManager[player_Room[ipname]].CurrentTurn =
                    roomManager[player_Room[ipname]].Members.IndexOf(
                        roomManager[player_Room[ipname]].LandlordIP);//起始玩家改为地主
            }
        }

        public void HandleDiscards(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "出牌");
            TransmitRecCMD(ipname, recData);
            SendTurn(ipname);
        }

        public void HandlePass(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "跳过");
            TransmitRecCMD(ipname, recData);
            SendTurn(ipname);
        }

        public void HandleGameOver(string ipname, RemoteCMD_Data recData)
        {
            Console.WriteLine("玩家" + recData.player.Name + "获得胜利，游戏结束");
            TransmitRecCMD(ipname, recData);
            ReleaseRoom(ipname);//释放房间
        }

        private void ReleaseRoom(string ipname)
        {
            for (int i = 0; i < roomManager[player_Room[ipname]].Members.Count; i++)
            {
                ipname_PlayerName.Remove(roomManager[player_Room[ipname]].Members[i]);
            }
            roomManager.ReleaseRoom(roomManager[player_Room[ipname]]);
            player_Room.Remove(ipname);
        }

        private void TransmitRecCMD(string ipname,RemoteCMD_Data recData)
        {
            string json = JsonMapper.ToJson(recData);
            byte[] buf = parser.EncoderData(json);
            TransmitMsg(roomManager[player_Room[ipname]].Members.ToArray(), buf, null);
        }

        private void SendRecCMD(string ipname, RemoteCMD_Data recData)
        {
            string json = JsonMapper.ToJson(recData);
            byte[] buf = parser.EncoderData(json);
            SendDataBegin(ipname, buf);
        }

        private void SendTurn(string ipname)
        {
            RemoteCMD_Data recData = new RemoteCMD_Data();
            recData.cmd = RemoteCMD_Const.GameTurn;
            int index = roomManager[player_Room[ipname]].CurrentTurn;//当前回合索引
            recData.player.Name = ipname_PlayerName[
                roomManager[player_Room[ipname]].Members[index]
                ];//当前玩家昵称
            //string targetIP = roomManager[player_Room[ipname]].Members[index];//IP
            TransmitRecCMD(ipname, recData);//转发回合
            roomManager[player_Room[ipname]].CurrentTurn++;//当前回合数增加
            roomManager[player_Room[ipname]].CurrentTurn %= 3;
            
        }

        public override void HandleDisconnected(string ipname)
        {
            ipname_PlayerName.Remove(ipname);
            //断开连接释放房间
            if (!player_Room.ContainsKey(ipname)) return;//已经释放
            ReleaseRoom(ipname);
            
        }
        public override void HandleAccepted(string ipName)
        {
            
        }
    }
}
