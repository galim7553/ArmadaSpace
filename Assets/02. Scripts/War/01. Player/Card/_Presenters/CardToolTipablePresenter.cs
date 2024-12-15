using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardToolTipablePresenter : CardPresenterBase
{
    PointerEnterHandler _pointerEnterHandler;

    UnityAction<Card> _onPointerEnterAction;
    UnityAction _onPointerExitAction;

    public CardToolTipablePresenter(Card model, CardView cardView, UnityAction<Card> onPointerEnterAction, UnityAction onPointerExitAction) : base(model, cardView)
    {
        _pointerEnterHandler = _view.gameObject.GetOrAddComponent<PointerEnterHandler>();
        _onPointerEnterAction = onPointerEnterAction;
        _onPointerExitAction = onPointerExitAction;

        RegisterPointerEnterHandler();
    }


    void RegisterPointerEnterHandler()
    {
        _pointerEnterHandler.Clear();
        _pointerEnterHandler.onPointerEntered += OnPointerEnter;
        _pointerEnterHandler.onPointerExited += OnPointerExit;
    }


    protected virtual void OnPointerEnter()
    {
        _onPointerEnterAction?.Invoke(_model);
    }
    protected virtual void OnPointerExit()
    {
        _onPointerExitAction?.Invoke();
    }

    public override void Clear()
    {
        base.Clear();

        _pointerEnterHandler.Clear();
    }
}