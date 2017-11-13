using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentCard_View : EventView {
    private GameObject Card;
    private Transform CardPos;
    private Text CardMsg_Txt;
    public void Init()
    {
        Card = Resources.Load<GameObject>("Prefabs/Card");
        CardPos = transform.Find("CardPos");
        CardMsg_Txt= transform.Find("CardMsg_Txt").GetComponent<Text>();
        CardMsg_Txt.text = "";
    }
    public void UpdateCards(Card[] cards)
    {
        RemoveAllCards();
        Sprite[] sprites = LoadSprites(cards);
        //装载所有卡片对象
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject c = Instantiate(Card, CardPos);
            
            c.GetComponent<Image>().sprite = sprites[i];
        }
    }
    private void RemoveAllCards()
    {
        for(int i = 0; i < CardPos.childCount; i++)
        {
            Destroy(CardPos.GetChild(i).gameObject);
        }
    }
    public void ShowBackCard()
    {
        Debug.Log("BaseCardBack");
        RemoveAllCards();
        GameObject back= Resources.Load<GameObject>("Prefabs/Card_Back");
        for(int i = 0; i < 3; i++)
        {
            Instantiate(back, CardPos);
        }
    }
    public void UpdateCardMsg(string msg)
    {
        CardMsg_Txt.text = msg;
    }
    public void ResetGame()
    {
        CardMsg_Txt.text = "";
        for (int i = 0; i < CardPos.childCount; i++)
        {
            Destroy(CardPos.GetChild(i).gameObject);
        }
    }
    private Sprite[] LoadSprites(Card[] cards)
    {
        Sprite[] array = new Sprite[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            //获取图片名字
            string name = global::Card.GetSpriteName(cards[i]);
            //加载图片
            array[i] = Resources.Load<Sprite>("Image/Pokers/" + name);
        }
        return array;
    }
}
