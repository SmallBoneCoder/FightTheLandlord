using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer_FightTheLandlord
{
    [Serializable]
    public class RemoteCMD_Data
    {
        public RemoteCMD_Const cmd;
        public PlayerInfo player;
        public Card[] cards;
        public RemoteCMD_Data()
        {
            player = new PlayerInfo();
        }
    }
    [Serializable]
    public class PlayerInfo
    {
        public string Name;
    }
    [Serializable]
    public class Card
    {

        public string Suit;
        public int Value;
        public Card() { }
        public Card(string suit, int value)
        {
            Suit = suit;
            Value = value;
        }
        public static string GetSpriteName(Card card)
        {
            string name = card.Suit;
            switch (card.Value)
            {

                case 1: name += PokerConst.Three; break;
                case 2: name += PokerConst.Four; break;
                case 3: name += PokerConst.Five; break;
                case 4: name += PokerConst.Six; break;
                case 5: name += PokerConst.Seven; break;
                case 6: name += PokerConst.Eight; break;
                case 7: name += PokerConst.Nine; break;
                case 8: name += PokerConst.Ten; break;
                case 9: name += PokerConst.Jack; break;
                case 10: name += PokerConst.Queen; break;
                case 11: name += PokerConst.King; break;
                case 12: name += PokerConst.Ace; break;
                case 13: name += PokerConst.Two; break;
                case 14: name += PokerConst.BlackJoker; break;
                case 15: name += PokerConst.RedJoker; break;
            }
            return name;
        }
    }
    public class PokerConst
    {
        public const string Spade = "Spade";
        public const string Heart = "Heart";
        public const string Club = "Club";
        public const string Diamond = "Diamond";
        public const string Ace = "Ace";
        public const string Two = "Two";
        public const string Three = "Three";
        public const string Four = "Four";
        public const string Five = "Five";
        public const string Six = "Six";
        public const string Seven = "Seven";
        public const string Eight = "Eight";
        public const string Nine = "Nine";
        public const string Ten = "Ten";
        public const string Jack = "Jack";
        public const string Queen = "Queen";
        public const string King = "King";
        public const string BlackJoker = "SJoker";
        public const string RedJoker = "LJoker";
    }
    [Serializable]
    public enum RemoteCMD_Const : int
    {
        Match = 0,//匹配
        MatchSuccess,//匹配成功
        GenerateCards,//生成牌
        DealCards,//发牌
        BaseCards,//底牌
        Player2,//指定玩家2
        Player3,//指定玩家3
        StartPlayer,//游戏开始的玩家
        CallLandlord,//叫地主
        NotCall,//不叫
        Claim,//抢地主
        NotClaim,//不抢
        Pass,//跳过
        Discards,//出牌
        GamerOver,//游戏结束
        GameTurn,//行动回合
        CancelMatch//取消匹配
    }
}
