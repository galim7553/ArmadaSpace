using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterCardsAreaView : RootBase
{
    public void AddCardOnBattleView(CardOnBattleView cardOnBattleView)
    {
        cardOnBattleView.transform.SetParent(transform, false);
        cardOnBattleView.transform.SetAsLastSibling();
    }
}
