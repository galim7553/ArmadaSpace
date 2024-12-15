using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerDownHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    UnityAction _pointerDownAction;
    UnityAction _pointerUpAction;

    public void AddPointerDownAction(UnityAction pointerDownAction)
    {
        _pointerDownAction += pointerDownAction;
    }
    public void AddPointerUpAction(UnityAction pointerUpAction)
    {
        _pointerUpAction += pointerUpAction;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownAction?.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _pointerUpAction?.Invoke();
    }

    public void Clear()
    {
        _pointerDownAction = null;
        _pointerUpAction = null;
    }
}
