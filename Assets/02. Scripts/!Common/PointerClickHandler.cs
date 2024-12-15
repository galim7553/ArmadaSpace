using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerClickHandler : MonoBehaviour, IPointerClickHandler
{
    public event UnityAction onPointerClicked;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClicked?.Invoke();
    }

    public void Clear()
    {
        onPointerClicked = null;
    }
}
