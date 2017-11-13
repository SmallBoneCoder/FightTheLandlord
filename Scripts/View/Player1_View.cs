using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player1_View : EventView {
    private Image Role_IMG;
    private Image Identity_IMG;
    private Text Name_Txt;
    private Transform CardPos;
    private Transform DiscardPos;
    private GameObject Card;
    private Dictionary<string, Card> cardValues;
    private Dictionary<string, Transform> cardTranfroms;
    private List<Card> selectedCards;
	public void Init()
    {
        selectedCards = new List<Card>();
        cardValues = new Dictionary<string, Card>();
        cardTranfroms = new Dictionary<string, Transform>();
        Role_IMG = transform.Find("Role_IMG").GetComponent<Image>();
        Identity_IMG = transform.Find("Identity_IMG").GetComponent<Image>();
        Name_Txt = transform.Find("Name_Txt").GetComponent<Text>();
        CardPos = transform.Find("CardPos");
        DiscardPos= transform.Find("DiscardPos");
        Card = Resources.Load<GameObject>("Prefabs/Card");
    }
    public void SelectedCard(int index)
    {
        //解除手牌列表和卡牌的父子关系
        Transform card = CardPos.GetChild(index);
        card.SetParent(DiscardPos);//放到出牌列表
        selectedCards.Add(
            cardValues[card.GetComponent<Image>().sprite.name]);
        SortSelected();//排序选择的牌
        dispatcher.Dispatch(ViewConst.Click_SelectCard, selectedCards.ToArray());
    }
    public void UpdatePlayerIdentity(Sprite id)
    {
        Identity_IMG.sprite = id;
    }
    public void UpdatePlayerName(string name)
    {
        Name_Txt.text = name;
    }
    public void DisSelectedCard(int index)
    {
        //解除出牌列表和卡牌的父子关系
        Transform card = DiscardPos.GetChild(index);
        card.SetParent(CardPos);//放到手牌列表
        selectedCards.Remove(
            cardValues[card.GetComponent<Image>().sprite.name]);
        SortHand();//排序手牌
        dispatcher.Dispatch(ViewConst.Click_DisSelectCard, selectedCards.ToArray());
    }
    private Sprite[] LoadSprites(Card[] cards)
    {
        Sprite[] array = new Sprite[cards.Length];
        for(int i = 0; i < cards.Length; i++)
        {
            //获取图片名字
            string name = global::Card.GetSpriteName(cards[i]);
            //加载图片
            Debug.Log(name);
            array[i] = Resources.Load<Sprite>("Image/Pokers/"+name);
        }
        return array;
    }
    private void SortSelected()
    {
        selectedCards.Sort((a, b) => { return a.Value.CompareTo(b.Value); });
        for(int i = 0; i < DiscardPos.childCount; i++)
        {
            int index = selectedCards.FindIndex(
                (c) =>
                {
                    return global::Card.GetSpriteName(c) ==
                    DiscardPos.GetChild(i).GetComponent<Image>().sprite.name;
                });
            DiscardPos.GetChild(i).SetSiblingIndex(index);
        }
    }
    private void SortHand()
    {
        List<Card> list = new List<Card>();
        for(int i = 0; i < CardPos.childCount; i++)
        {
            //找到手牌对应的Card
            list.Add(cardValues[
                CardPos.GetChild(i).
                GetComponent<Image>().sprite.name]);
        }
        //排序
        list.Sort((a, b) => { return -a.Value.CompareTo(b.Value); });
        for(int i = 0; i < list.Count; i++)
        {
            //把排好序后的Card对应的transform位置一一对应
            cardTranfroms[global::Card.GetSpriteName(list[i])].SetSiblingIndex(i);
        }
    }
    public IEnumerator ConstructCards(Card[] cards,float time)
    {
        //cardValues.Clear();
        //cardTranfroms.Clear();
        List<Card> list = new List<Card>(cards);
        Debug.Log(list.Count);
        list.Sort((a, b) => { return -a.Value.CompareTo(b.Value); });//按大小排序
        Sprite[] sprites= LoadSprites(list.ToArray());//获取图片
        //创建一个trigger用来响应图片点击
        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerDown;
        click.callback.AddListener(Click_SelectCard);
        //装载所有卡片对象
        for(int i = 0; i < sprites.Length; i++)
        {
            GameObject c = Instantiate(Card,CardPos);
            cardValues.Add(sprites[i].name, list[i]);//保存所有card
            cardTranfroms.Add(sprites[i].name, c.transform);//保存所有transform
            c.GetComponent<Image>().sprite = sprites[i];
            c.AddComponent<EventTrigger>();
            c.GetComponent<EventTrigger>().triggers.Add(click);
            yield return new WaitForSeconds(time);
        }
        SortHand();
    }
    public void RemoveAllDiscards()
    {
        for (int i = 0; i < DiscardPos.childCount; i++)
        {
            Destroy(DiscardPos.GetChild(i).gameObject);
        }
        selectedCards.Clear();//清空
    }
    public void DisSelectedAllCards()
    {
        int count = DiscardPos.childCount;
        for(int i = 0; i < count; i++)
        {
            DisSelectedCard(0);
        }
    }
    public void ResetGame()
    {
        selectedCards.Clear();
        cardValues.Clear();
        cardTranfroms.Clear();
        for(int i = 0; i < CardPos.childCount; i++)
        {
            Destroy(CardPos.GetChild(i).gameObject);
        }
        for (int i = 0; i < DiscardPos.childCount; i++)
        {
            Destroy(DiscardPos.GetChild(i).gameObject);
        }
    }
    public Card[] GetSelectedCards()
    {
        return selectedCards.ToArray();
    }
    private void Click_SelectCard(BaseEventData data)
    {
        PointerEventData pdata = data as PointerEventData;
        if (pdata.pointerEnter == null)
        {
            return;
        }
        //选牌
        if (pdata.pointerEnter.transform.parent == CardPos)
        {
            SelectedCard(pdata.pointerEnter.transform.GetSiblingIndex());
        }
        //取消选牌
        else
        {
            DisSelectedCard(pdata.pointerEnter.transform.GetSiblingIndex());
        }
    }
    public void SelectedAllCards(Card[] cards)
    {
        DisSelectedAllCards();//上次选的取消
        for(int i = 0; i < cards.Length; i++)
        {
            for(int j = 0; j < CardPos.childCount; j++)
            {
                //找到对应的手牌索引
                if(CardPos.GetChild(j).GetComponent<Image>().sprite.name==
                    global::Card.GetSpriteName(cards[i]))
                {
                    //解除手牌列表和卡牌的父子关系
                    Transform card = CardPos.GetChild(j);
                    card.SetParent(DiscardPos);//放到出牌列表
                    selectedCards.Add(
                        cardValues[card.GetComponent<Image>().sprite.name]);
                }
            }
        }
        SortHand();//排序手牌
        //不需要发送更新消息，因为已经更新过了
    }
}
