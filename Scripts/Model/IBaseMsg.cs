using UnityEngine;

using System;

public  interface IBaseMsg
{
    RemoteCMD_Data Data { get; set; }
    string ToJson();
}
public class RemoteMsg : IBaseMsg
{
    public RemoteCMD_Data Data{ get;set; }
    public RemoteMsg(RemoteCMD_Data data)
    {
        Data = data;
    }
    public string ToJson()
    {
        string json = LitJson.JsonMapper.ToJson(Data);
        return json;
    }
}


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
public enum RemoteCMD_Const:int
{
    Match=0,//匹配
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