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
#if UNITY_CLIENT
        //客户端请求
        return;
#else
        List<List<CardData>> list = DealCard(vPlayerNum, vCardNum);

        defaultproto.UpdateDealCard rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.UpdateDealCard>();
        for (int i = 0; i < vPlayerNum; i++)
        {
            List<CardData> cardDatas = list[i];

            //call  server's function
            EntityGambler entityGambler = vRequester.OnHandleDealCard(cardDatas, i);

            //send msg to client 
            foreach (CardData item in cardDatas)
            {
                defaultproto.CardData cardData = new defaultproto.CardData();
                cardData.cardType = (defaultproto.CardType)item.m_CardType;
                cardData.rank = item.m_Rank;
                rep_pool.cardDatas.Add(cardData);
            }
            UtilityMsgHandle.SendMsgWithPlayerId(CommandName.UPDATEDEALCARD, rep_pool, entityGambler.GetPlayerId());
            rep_pool.cardDatas.Clear();
        }
        UtilityObjectPool.Instance.Enqueue<defaultproto.UpdateDealCard>(rep_pool);

        //call  server's function
        vRequester.OnHandleDealCardFinish();

        //send msg to all client 
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.UPDATEDEALCARDFISH, null, vRequester.Id2EntityGambler.Values.ToArray());
#endif  

    }
}
