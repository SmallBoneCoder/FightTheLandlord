using UnityEngine;

using System.Collections.Generic;
using System;
public enum GameMode
{
    MutiPlayer,
    SinglePlayer
}
public interface IGameData 
{
    List<PlayerInfo> Players { get; set; }
    List<Card> SelectedCards { get; set; }
    List<Card> HandCards { get; set; }
    PlayerInfo PlayerSelf{ get; set; }
    PlayerInfo Player2 { get; set; }
    PlayerInfo Player3 { get; set; }
    List<Card> BaseCards { get; set; }
    List<Card> CurrentDiscards { get; set; }
    GameStatus CurrentStatus { get; set; }
    int CurrentTurn { get; set; }
    int BaseScore { get; set; }
    int Mutiple { get; set; }
    int ClaimCount { get; set; }
    string Landlord { get; set; }
    int RestCardNum { get; set; }
    string LastPlayer { get; set; }//上次出牌玩家
    GameMode CurrentMode { get; set; }
    //
    List<Card> GenerateCards();
    bool CardsCompareToDicards(Card[] cards);
    string GetCardsType(Card[] cards);
    void Init();
    List<Card> GetEnabledDisCards(List<Card> cards,string name);
}
public class GameDataCenter : IGameData
{
    public GameMode CurrentMode { get; set; }
    public string LastPlayer { get; set; }
    public int ClaimCount { get; set; }
    public int CurrentTurn { get; set; }
    public List<PlayerInfo> Players { get; set; }
    public PlayerInfo PlayerSelf { get; set; }
    public PlayerInfo Player2 { get; set; }
    public PlayerInfo Player3 { get; set; }
    public string Landlord { get; set; }
    public GameStatus CurrentStatus { get; set; }
    public List<Card> SelectedCards { get ; set ; }
    public List<Card> BaseCards { get; set; }
    public List<Card> HandCards { get; set; }
    public List<Card> CurrentDiscards { get; set; }
    public int Mutiple { get; set; }
    public int BaseScore { get; set; }
    public int RestCardNum { get; set; }
    public GameDataCenter()
    {
        PlayerSelf = new PlayerInfo();
        Players = new List<PlayerInfo>();
        SelectedCards = new List<Card>();
        HandCards = new List<Card>();
        CurrentDiscards = new List<Card>();
        BaseCards = new List<Card>();
        CurrentStatus = GameStatus.CallLandlord;
        ClaimCount = 4;
        CurrentTurn = 0;
        BaseScore = 1000;
        RestCardNum = 17;
        Mutiple = 1;//默认一倍
        CurrentMode = GameMode.MutiPlayer;//默认联网
        Landlord = "";
    }
    public void Init()
    {
        Players.Clear();
        SelectedCards.Clear();
        HandCards.Clear();
        CurrentDiscards.Clear();
        BaseCards.Clear();
        CurrentStatus = GameStatus.CallLandlord;
        ClaimCount = 4;
        CurrentTurn = 0;
        RestCardNum = 17;
        Mutiple = 1;
        CurrentMode = GameMode.MutiPlayer;
        LastPlayer = "";
        Landlord = "";
        BaseScore = 1000;
    }
    public  List<Card> GetEnabledDisCards(List<Card> cards,string name)
    {
        List<Card> Result = new List<Card>();

        Card[] temp = (Card[])cards.ToArray().Clone();
        List<Card> yourCards = new List<Card>(temp);
        //
        yourCards.Sort((a, b) => { return a.Value.CompareTo(b.Value); });//升序排序
        CurrentDiscards.Sort((a, b) => { return a.Value.CompareTo(b.Value); });
        string sourceCardType = GetCardsType(CurrentDiscards.ToArray());//得到出的牌型
        List<List<Card>> repeats_YourCards = CountRepeats(yourCards.ToArray());//按值重复次数分类
        List<List<Card>> repeats_SourceCards = CountRepeats(CurrentDiscards.ToArray());//按值重复次数分类
        if (name.Equals(LastPlayer)||string.IsNullOrEmpty(LastPlayer))//上次是自己
        {
            for (int i = repeats_YourCards.Count-2; i >=0; i--)
            {
                if (repeats_YourCards[i].Count > 0)
                {
                    List<Card> range = yourCards.FindAll(
                    (c) => {
                        return c.Value == repeats_YourCards[i][repeats_YourCards[i].Count-1].Value;
                    });//从3连对开始找,找最大的
                    Result.AddRange(range);
                    //移除
                    for (int k = 0; k < range.Count; k++)
                    {
                        yourCards.Remove(range[k]);
                    }
                    cards.Clear();
                    cards.AddRange(yourCards);//替换当前手牌
                    return Result;
                }
            }
        }
        switch (sourceCardType)
        {
            case CardsType.Bomb:
                {
                    //炸弹，查找第一个重复次数为4的大于上家的重复次数为4的牌
                    for (int i = 0; i < repeats_YourCards[3].Count; i++)
                    {
                        if (repeats_YourCards[3][i].Value > repeats_SourceCards[3][0].Value)
                        {
                            for (int j = 0; j < yourCards.Count; j++)
                            {
                                if (yourCards[j].Value == repeats_YourCards[3][i].Value)
                                {
                                    Result.Add(yourCards[j]);//找到不同花色的重复的牌
                                }
                            }
                            yourCards.RemoveAll((c) =>
                            {
                                return c.Value == repeats_YourCards[3][i].Value;
                            });//删除所有要出的牌
                            break;//跳出
                        }
                    }
                }
                break;
            case CardsType.Double:
                {
                    //对子，查找第一个重复次数为2的大于上家的重复次数为3的牌
                    for (int i = 0; i < repeats_YourCards[1].Count; i++)
                    {
                        if (repeats_YourCards[1][i].Value > repeats_SourceCards[1][0].Value)
                        {
                            for (int j = 0; j < yourCards.Count; j++)
                            {
                                if (yourCards[j].Value == repeats_YourCards[1][i].Value)
                                {
                                    Result.Add(yourCards[j]);//找到不同花色的重复的牌
                                }
                            }
                            yourCards.RemoveAll((c) =>
                            {
                                return c.Value == repeats_YourCards[1][i].Value;
                            });//删除所有要出的牌
                            break;//跳出
                        }
                    }
                }
                break;
            case CardsType.Triple:
                {
                    //三张，查找第一个重复次数为3的大于上家的重复次数为3的牌
                    for (int i = 0; i < repeats_YourCards[2].Count; i++)
                    {
                        if (repeats_YourCards[2][i].Value > repeats_SourceCards[2][0].Value)
                        {
                            for(int j = 0; j < yourCards.Count; j++)
                            {
                                if(yourCards[j].Value== repeats_YourCards[2][i].Value)
                                {
                                    Result.Add(yourCards[j]);//找到不同花色的重复的牌
                                }
                            }
                            yourCards.RemoveAll((c) =>
                            {
                                return c.Value == repeats_YourCards[2][i].Value;
                            });//删除所有要出的牌
                            break;//跳出
                        }
                    }
                }
                break;
            case CardsType.Single:
                {
                    //单张，查找第一个大于上家的牌
                    for(int i = 0; i < yourCards.Count; i++)
                    {
                        if (yourCards[i].Value >CurrentDiscards[0].Value)
                        {
                            Result.Add(yourCards[i]);//添加到待出牌
                            yourCards.RemoveAt(i);//移除出的牌
                            break;//跳出
                        }
                    }
                }break;
            case CardsType.Straight:
                {
                    //单顺子
                    bool flag = true;
                    for (int i = CurrentDiscards.Count-1; i >=0; i--)
                    {
                        Card card = null;
                        if ((card=yourCards.Find((c) => 
                            {
                                return c.Value == CurrentDiscards[i].Value+1;
                             }))!=null)//必须要有比出的顺子的每一张牌都大1的牌
                        {
                            Result.Add(card);//结果添加
                            yourCards.Remove(card);//移除
                        }
                        else
                        {
                            flag = false;//没有找到就退出
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Result.Clear();//清除结果
                    }
                }
                break;
            case CardsType.DoubleStraight:
                {
                    //双顺子
                    bool flag = true;
                    for (int i = repeats_SourceCards[1].Count - 1; i >= 0; i--)
                    {
                        Card card = null;
                        if ((card = repeats_YourCards[1].Find((c) =>
                        {
                            return c.Value == repeats_SourceCards[1][i].Value + 1;
                        })) != null)//必须要有比出的顺子的每一张牌都大1的牌
                        {
                            List<Card> range = yourCards.FindAll(
                                (c) => { return c.Value == card.Value; });//找到所有重复的牌
                            if (range.Count > 2)
                            {
                                range.RemoveRange(2, range.Count - 1);//只留两张牌
                            }
                            Result.AddRange(range);//结果添加
                            //移除
                            for(int k = 0; k < range.Count; k++)
                            {
                                yourCards.Remove(range[k]);
                            }
                        }
                        else
                        {
                            flag = false;//没有找到就退出
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Result.Clear();//清除结果
                    }
                }
                break;
            case CardsType.TripleStraight:
                {
                    //三顺子
                    bool flag = true;
                    for (int i = repeats_SourceCards[2].Count - 1; i >= 0; i--)
                    {
                        Card card = null;
                        if ((card = repeats_YourCards[2].Find((c) =>
                        {
                            return c.Value == repeats_SourceCards[2][i].Value + 1;
                        })) != null)//必须要有比出的顺子的每一张牌都大1的牌
                        {
                            List<Card> range = yourCards.FindAll(
                                (c) => { return c.Value == card.Value; });//找到所有重复的牌
                            if (range.Count > 3)
                            {
                                range.RemoveRange(3, range.Count - 1);//只留三张牌
                            }
                            Result.AddRange(range);//结果添加
                            //移除
                            for (int k = 0; k < range.Count; k++)
                            {
                                yourCards.Remove(range[k]);
                            }
                        }
                        else
                        {
                            flag = false;//没有找到就退出
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Result.Clear();//清除结果
                    }
                }
                break;
            case CardsType.TriplePlusOne:
                {
                    //三带一，只比较第三张牌
                    int value = -1;
                    for (int i = 0; i < repeats_YourCards[2].Count; i++)
                    {
                        if (repeats_YourCards[2][i].Value > repeats_SourceCards[2][0].Value)
                        {
                            value = repeats_YourCards[2][i].Value;
                            break;
                        }
                    }
                    if (value != -1)
                    {
                        List<Card> range = yourCards.FindAll((c) => { return c.Value == value; });
                        if (range.Count > 3)
                        {
                            range.RemoveAt(3);//只要三个
                        }
                        Result.AddRange(range);//添加进结果
                        //移除
                        for (int k = 0; k < range.Count; k++)
                        {
                            yourCards.Remove(range[k]);
                        }
                        Result.Add(yourCards[0]);//带一张
                        yourCards.RemoveAt(0);//移除
                    }
                }
                break;
            case CardsType.TriplePlusDouble:
                {
                    //三带一对
                    int value = -1;
                    if (repeats_YourCards[1].Count <= 0) break;//没有对子
                    for (int i = 0; i < repeats_YourCards[2].Count; i++)
                    {
                        if (repeats_YourCards[2][i].Value > repeats_SourceCards[2][0].Value)
                        {
                            value = repeats_YourCards[2][i].Value;
                            break;
                        }
                    }
                    if (value != -1)//有比上家大的3连
                    {
                        List<Card> range = yourCards.FindAll((c) => { return c.Value == value; });
                        if (range.Count > 3)
                        {
                            range.RemoveAt(3);//只要三个
                        }
                        Result.AddRange(range);//添加进结果
                        //移除
                        for (int k = 0; k < range.Count; k++)
                        {
                            yourCards.Remove(range[k]);
                        }
                        range= yourCards.FindAll((c) => 
                        { return c.Value == repeats_YourCards[1][0].Value; });//找到第一个对子
                        Result.AddRange(range);//添加进结果
                        //移除
                        for (int k = 0; k < range.Count; k++)
                        {
                            yourCards.Remove(range[k]);
                        }
                    }
                }
                break;
            case CardsType.QuartePlusTwo:
                {
                    //四带2单
                    int value = -1;
                    for (int i = 0; i < repeats_YourCards[3].Count; i++)
                    {
                        if (repeats_YourCards[3][i].Value > repeats_SourceCards[3][0].Value)
                        {
                            value = repeats_YourCards[3][i].Value;
                            break;
                        }
                    }
                    if (value != -1)//有比上家大的4连
                    {
                        List<Card> range = yourCards.FindAll((c) => { return c.Value == value; });
                        Result.AddRange(range);//添加进结果
                        //移除
                        for (int k = 0; k < range.Count; k++)
                        {
                            yourCards.Remove(range[k]);
                        }
                        //加两个单牌
                        Result.Add(yourCards[0]);
                        yourCards.RemoveAt(0);
                        Result.Add(yourCards[0]);
                        yourCards.RemoveAt(0);
                    }
                }
                break;
            case CardsType.QuartePlusDouble:
                {
                    //四带2对
                    int value = -1;
                    for (int i = 0; i < repeats_YourCards[3].Count; i++)
                    {
                        if (repeats_YourCards[3][i].Value > repeats_SourceCards[3][0].Value)
                        {
                            value = repeats_YourCards[3][i].Value;
                            break;
                        }
                    }
                    if (value != -1)//有比上家大的4连
                    {
                        List<Card> range = yourCards.FindAll((c) => { return c.Value == value; });
                        Result.AddRange(range);//添加进结果
                        //移除
                        for (int k = 0; k < range.Count; k++)
                        {
                            yourCards.Remove(range[k]);
                        }
                        //加两个对牌
                        for(int i = 0; i < repeats_YourCards[1].Count; i++)
                        {
                            range = yourCards.FindAll((c) =>
                            {
                                return c.Value == repeats_YourCards[1][i].Value;
                            });
                            Result.AddRange(range);
                            //移除
                            for (int j = 0; j < range.Count; j++)
                            {
                                yourCards.Remove(range[j]);
                            }
                        }
                    }
                }
                break;
            case CardsType.PlanePlusWings:
                {
                    //飞机带翅膀，比较重复次数为3的最大的一张牌
                    //三顺子
                    bool flag = true;
                    for (int i = repeats_SourceCards[2].Count - 1; i >= 0; i--)
                    {
                        Card card = null;
                        if ((card = repeats_YourCards[2].Find((c) =>
                        {
                            return c.Value == repeats_SourceCards[2][i].Value + 1;
                        })) != null)//必须要有比出的顺子的每一张牌都大1的牌
                        {
                            List<Card> range = yourCards.FindAll(
                                (c) => { return c.Value == card.Value; });//找到所有重复的牌
                            if (range.Count > 2)
                            {
                                range.RemoveRange(2, range.Count - 1);//只留两张牌
                            }
                            Result.AddRange(range);//结果添加
                            //移除
                            for (int k = 0; k < range.Count; k++)
                            {
                                yourCards.Remove(range[k]);
                            }
                        }
                        else
                        {
                            flag = false;//没有找到就退出
                            break;
                        }
                    }
                    //
                    if (!flag)
                    {
                        Result.Clear();//清除结果
                    }
                    else
                    {
                        //带的是单牌
                        if (repeats_SourceCards[2].Count == repeats_SourceCards[0].Count)
                        {
                            for(int i=0;i< repeats_SourceCards[2].Count;i++)
                            {
                                Result.Add(yourCards[0]);
                                yourCards.RemoveAt(0);
                            }
                        }
                        //带的是对牌
                        if (repeats_SourceCards[2].Count == repeats_SourceCards[1].Count)
                        {
                            for (int i = 0; i < repeats_YourCards[1].Count; i++)
                            {
                                List<Card> range = yourCards.FindAll(
                                    (c) => { return c.Value == repeats_YourCards[1][i].Value; });//找到对牌
                                Result.AddRange(range);
                                //移除
                                for (int k = 0; k < range.Count; k++)
                                {
                                    yourCards.Remove(range[k]);
                                }
                            }

                        }
                    }
                }
                break;

        }
        if (Result.Count == CurrentDiscards.Count)
        {
            cards.Clear();
            cards.AddRange(yourCards);//替换当前手牌
        }
        else
        {
            Result.Clear();
            for (int j = 0; j < yourCards.Count; j++)
            {
                if (yourCards[j].Value == (int)PokerValue.BlackJoker ||
                    yourCards[j].Value == (int)PokerValue.RedJoker)
                {
                    Result.Add(yourCards[j]);
                }
            }
            if (Result.Count == 2)//大小王都在，王炸
            {
                yourCards.RemoveAll((c) =>
                {
                    return (c.Value == (int)PokerValue.BlackJoker ||
                    c.Value == (int)PokerValue.RedJoker);
                });//删除出牌
                cards.Clear();
                cards.AddRange(yourCards);//替换当前手牌
            }
            else
            {
                Result.Clear();//清除单王
            }
            //有炸弹,并且上家不是炸弹
            if (repeats_YourCards[3].Count > 1&&sourceCardType!=CardsType.Bomb)
            {
                List<Card> range = yourCards.FindAll(
                (c) => { return c.Value == repeats_YourCards[3][0].Value; });//找到炸弹
                Result.AddRange(range);
                //移除
                for (int k = 0; k < range.Count; k++)
                {
                    yourCards.Remove(range[k]);
                }
                cards.Clear();
                cards.AddRange(yourCards);//替换当前手牌
            }
            
        }
        return Result;//返回出的牌
    }
    public List<Card> GenerateCards()
    {
        List<Card> sourceCards = new List<Card>();
        System.Random rand = new System.Random();
        string[] suit = {
            PokerConst.Club,//梅花
            PokerConst.Diamond,//方块
            PokerConst.Heart,//红桃
            PokerConst.Spade};//黑桃
        for(int i = 0; i < 4; i++)
        {
            for(int j = 1; j < 14; j++)
            {
                sourceCards.Add(new Card(suit[i], j));//每种花色生成13张
            }
        }
        sourceCards.Add(new Card("", 14));//小王
        sourceCards.Add(new Card("", 15));//大王
        List<Card> randCards = new List<Card>();
        int index = 0;
        for(int i = 0; i < 54; i++)
        {
            index = rand.Next(0, sourceCards.Count);
            randCards.Add(sourceCards[index]);//随机添加
            sourceCards.RemoveAt(index);//加完以后删除防止重复
        }
        return randCards;
    }
    /// <summary>
    /// 和上家出的牌进行比较
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public bool CardsCompareToDicards(Card[] cards)
    {
        if (CurrentDiscards.Count <= 0 || CurrentDiscards == null)
        {
            return true;//第一个出牌
        }
        string discardType = GetCardsType(CurrentDiscards.ToArray());
        string cardType = GetCardsType(cards);
        if (cardType.Equals(CardsType.Mussy)){
            return false;//杂牌不能出
        }
        List<List<Card>> repeats_Cards=null;
        List<List<Card>> repeats_Discards = null;
        if (discardType.Equals(CardsType.Rocket))return false;//上家是王炸
        if (cardType.Equals(CardsType.Rocket))
        {
            return true;//自己是王炸
        }
        if (cardType.Equals(CardsType.Bomb)&&!discardType.Equals(CardsType.Bomb))
        {
            return true;//自己是炸弹，上家不是
        }
        if (!cardType.Equals(CardsType.Bomb) &&discardType.Equals(CardsType.Bomb))
        {
            return false;//上家是炸弹，自己不是
        }
        if (cardType.Equals(discardType))//牌型相等才能比较
        {
            switch (cardType)
            {
                case CardsType.Bomb:
                case CardsType.Double:
                case CardsType.Triple:
                case CardsType.Single:
                    {
                        //单张或重复，只比较第一张
                        return cards[0].Value > CurrentDiscards[0].Value;
                    }
                case CardsType.Straight:
                case CardsType.DoubleStraight:
                case CardsType.TripleStraight:
                    {
                        //顺子，比较最后一张
                        return cards[cards.Length - 1].Value > 
                            CurrentDiscards[CurrentDiscards.Count - 1].Value;
                    }
                case CardsType.TriplePlusOne:
                case CardsType.TriplePlusDouble:
                    {
                        //三带一，只比较第三张牌
                        return cards[2].Value >
                            CurrentDiscards[2].Value;
                    }
                case CardsType.QuartePlusTwo:
                    {
                        //四带二单张，比较第3张牌
                        return cards[2].Value >
                            CurrentDiscards[2].Value;
                    }
                case CardsType.QuartePlusDouble:
                    {
                        //四带二对，比较重复次数为4的牌
                        repeats_Cards = CountRepeats(cards);
                        repeats_Discards = CountRepeats(cards);
                        return repeats_Cards[3][0].Value >
                            repeats_Discards[3][0].Value;
                    }
                case CardsType.PlanePlusWings:
                    {
                        //飞机带翅膀，比较重复次数为3的最大的一张牌
                        if (repeats_Cards == null)
                        {
                            repeats_Cards = CountRepeats(cards);
                            repeats_Discards = CountRepeats(cards);
                        }
                        return repeats_Cards[2][repeats_Cards[2].Count - 1].Value >
                            repeats_Discards[2][repeats_Cards[2].Count - 1].Value;
                    }

            }
        }
        return false;//牌型和上家不同，无法出牌
    }

    public string GetCardsType(Card[] cards)
    {
        List<Card> lst = new List<Card>(cards);
        lst.Sort((a, b) => { return a.Value.CompareTo(b.Value); });//升序排序
        if (lst.Count <= 4)
        {
            if(lst.Count == 1)
            {
                return CardsType.Single;
            }
            if (lst.Count == 2)
            {
                if (lst[0].Value == lst[1].Value)
                {
                    return CardsType.Double;//对子
                }
                if(lst[0].Value==(int)PokerValue.BlackJoker||lst[0].Value== (int)PokerValue.RedJoker&&
                   lst[1].Value == (int)PokerValue.BlackJoker || lst[1].Value == (int)PokerValue.RedJoker)
                {
                    return CardsType.Rocket;
                }
            }
            if(lst.Count == 3)
            {
                if (lst[0].Value == lst[2].Value)
                {
                    return CardsType.Triple;
                }
            }
            if (lst.Count == 4)
            {
                //四张都相等
                if (lst[0].Value == lst[3].Value)
                {
                    return CardsType.Bomb;
                }
                //1到3或2到4相等
                if (lst[0].Value == lst[2].Value || lst[1].Value == lst[3].Value)
                {
                    return CardsType.TriplePlusOne;
                }
            }
        }
        else
        {
            List<List<Card>> repeats = new List<List<Card>>() {
                new List<Card>(),new List<Card>(),new List<Card>(),new List<Card>()};
            bool hasTwoOrBJokerOrRJoker = false;
            for(int i = 0; i < lst.Count; i++)
            {
                int count = 0;
                if (lst[i].Value == (int)PokerValue.Two || lst[i].Value == (int)PokerValue.BlackJoker ||
                    lst[i].Value == (int)PokerValue.RedJoker)
                {
                    hasTwoOrBJokerOrRJoker = true;//有大王或小王或2
                }
                for(int j = 0; j < lst.Count; j++)
                {
                    if (lst[j].Value == lst[i].Value)
                    {
                        count++;//重复次数+1
                    }
                }
                //重复数据的值只记录一次，如333中的3只记录一次
                if(!repeats[count-1].Exists((c)=> { return c.Value == lst[i].Value;}))
                repeats[count-1].Add(lst[i]);//根据重复次数分别记录（1,2,3,4）
            }
            //5张牌，重复3张一次，重复两张一次，如 444+99
            if (lst.Count == 5 && repeats[2].Count == 1 && repeats[1].Count == 1)
            {
                return CardsType.TriplePlusDouble;
            }
            //6张牌，重复4张一次，如4444+3+8 或 4444+33)
            if (lst.Count == 6 && repeats[3].Count == 1 )
            {
                return CardsType.QuartePlusTwo;
            }
            //8张牌，重复4张一次，如4444＋55＋77)
            if (lst.Count == 8 && repeats[3].Count == 1&& repeats[1].Count == 2)
            {
                return CardsType.QuartePlusDouble;
            }
            //三顺＋同数量的对牌，如：333444555+7799JJ
            if (repeats[2].Count >= 2 && repeats[2].Count == lst.Count / 5 &&
                repeats[2][repeats[2].Count - 1].Value - repeats[2][0].Value ==
                lst.Count / 5 - 1)
            {
                return CardsType.PlanePlusWings;
            }
            //三顺＋同数量的单牌，如：444555+79
            if (repeats[2].Count >= 2 && repeats[2].Count == lst.Count / 4 &&
                repeats[2][repeats[2].Count - 1].Value - repeats[2][0].Value ==
                lst.Count / 4 - 1)
            {
                return CardsType.PlanePlusWings;
            }
            
            if (!hasTwoOrBJokerOrRJoker)//没有大小王2
            {
                //任意五张或五张以上点数相连的牌，如：45678或78910JQK。不包括 2和双王
                if (lst.Count == repeats[0].Count && (lst[lst.Count-1].Value-lst[0].Value)==lst.Count-1)
                {
                    return CardsType.Straight;
                }
                //三对或更多的连续对牌，如：334455、7788991010JJ。不包括 2 点和双王
                if (lst.Count/2 >= 3 && lst.Count%2==0&&repeats[1].Count==lst.Count/2&&
                    (lst[lst.Count - 1].Value - lst[0].Value) == lst.Count/2 - 1)
                {
                    return CardsType.DoubleStraight;
                }
                //二个或更多的连续三张牌，如：333444 、555666777888。不包括 2 点和双王:
                if (lst.Count / 3 == repeats[2].Count && lst.Count % 3 == 0 && 
                    (lst[lst.Count - 1].Value - lst[0].Value) == lst.Count / 3 - 1)
                {
                    if (repeats[2].Count == repeats[0].Count||
                        repeats[2].Count == repeats[1].Count)
                    {
                        return CardsType.PlanePlusWings;//三顺+同样数量的单牌或对子
                    }
                    return CardsType.TripleStraight;
                }
            }
        }
        return CardsType.Mussy;
    }
    private List<List<Card>> CountRepeats(Card[] cards)
    {
        List<Card> lst = new List<Card>(cards);
        lst.Sort((a, b) => { return a.Value.CompareTo(b.Value); });//升序排序
        List<List<Card>> repeats = new List<List<Card>>() {
                new List<Card>(),new List<Card>(),new List<Card>(),new List<Card>()};
        for (int i = 0; i < lst.Count; i++)
        {
            int count = 0;
            for (int j = 0; j < lst.Count; j++)
            {
                if (cards[j].Value == cards[i].Value)
                {
                    count++;//重复次数+1
                }
            }
            //重复数据的值只记录一次，如333中的3只记录一次
            if (!repeats[count-1].Exists((c) => { return c.Value == lst[i].Value; }))
                repeats[count-1].Add(lst[i]);//根据重复次数分别记录（1,2,3,4）
        }
        return repeats;
    }
}
public enum PokerValue : int
{
    Three=1,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace,
    Two,
    BlackJoker,
    RedJoker
}
public class CardsType
{
    public const string Straight = "顺子";
    public const string DoubleStraight = "连对";
    public const string TripleStraight = "飞机";
    public const string Single = "单张";
    public const string Double = "对子";
    public const string Triple = "三张";
    public const string TriplePlusOne = "三带一";
    public const string TriplePlusDouble = "三带一对";
    public const string QuartePlusTwo = "四带二";
    public const string QuartePlusDouble = "四带二对";
    public const string PlanePlusWings = "飞机带翅膀";
    public const string Bomb = "炸弹";
    public const string Rocket = "火箭";//王炸
    public const string Mussy = "杂牌";
}
public enum GameStatus
{
    CallLandlord=0,
    Claim=1,
    FightLandlord=2
}
[Serializable]
public class PlayerInfo
{
    public string Name;
}