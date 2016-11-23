using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 这是一盒牌，不能实例化
/// </summary>
public static class CardBox
{
    public static List<CardData> m_TotalCardList;

    static CardBox()
    {
        m_TotalCardList = new List<CardData>();
        CardType cardType;
        for (int i = 0; i < (int)CardType.Max; i++)
        {
            cardType = (CardType)i;
            for (int j = 1; j <= 13; j++)
            {
                CardData cardData = new CardData();
                //4种牌
                cardData.m_CardType = cardType;
                //1，2，。。。。。13
                cardData.m_Rank = j;
         }
        }

    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <returns></returns>
     static List<List<CardData>> DealCard(int vPlayerNum, int vCardNum)
    {
        List<List<CardData>> cardDatasList = new List<List<CardData>>(vPlayerNum);
        for (int i = 0; i < vPlayerNum; i++)
        {
            List<CardData> cardDatas = new List<CardData>();
            cardDatasList.Add(cardDatas);
            for (int j = 0; j < vCardNum; j++)
            {
                //临时代码
                CardData cardData = new CardData();
                Random ran = new Random();
                cardData.m_Rank = ran.Next(1, 14);
                cardData.m_CardType = (CardType)ran.Next(0, 4);
                cardDatas.Add(cardData);
            }
        }
        return cardDatasList;
    }

    /// <summary>
    /// 请求发牌
    /// </summary>
    /// <param name="vPlayerNum"></param>
    /// <param name="vCardNum"></param>
    public static void ReqDealCard(int vPlayerNum, int vCardNum, BattleController vRequester)
    {
        List<List<CardData>> list = DealCard(vPlayerNum, vCardNum);

        //服务器直接处理，客户端需要等待发牌消息
        vRequester.OnHandleDealCard(list);
#if UNITY_CLIENT
        
#endif
        List<EntityGambler> entityGamblers = vRequester.GetEntityGamblers();
        if (!entityGamblers.IsNullOrEmpty())
        {
            for (int i = 0; i < entityGamblers.Count; i++)
            {
                EntityGambler entityGambler = entityGamblers[i];
#error 每个人发自己的牌，其他人的牌不要发
            }
        }
    }
}
