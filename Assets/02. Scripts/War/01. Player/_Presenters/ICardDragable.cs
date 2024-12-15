using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardDragable
{
    public void BeginDrag(Card card);
    public void EndDrag();
    public void OnDragSucceeded(Card card);
    public void OnDragFailed(Card card);
}
public interface ICardDropable
{
    public bool GetIsDropable(SelectedCardPresenter.DragType dragType, Card card);
    public void TemporarilyFilter(CardType cardType);
    public void RecoverFilter();
    public void ResetTempFilter();
    public void OnDropSucceeded(SelectedCardPresenter.DragType dragType, Card card);
}
