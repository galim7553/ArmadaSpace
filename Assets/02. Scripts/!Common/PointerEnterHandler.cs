using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEnterHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event UnityAction onPointerEntered;
    public event UnityAction onPointerExited;

    bool _hasPointerEntered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BeginPointerEnter();
    }

    void BeginPointerEnter()
    {
        if (_hasPointerEntered == true)
            return;

        _hasPointerEntered = true;
        onPointerEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EndPointerEnter();
    }

    void EndPointerEnter()
    {
        if (_hasPointerEntered == false)
            return;

        _hasPointerEntered = false;
        onPointerExited?.Invoke();
    }

    public void Clear()
    {
        EndPointerEnter();

        onPointerEntered = null;
        onPointerExited = null;
    }

    private void OnDisable()
    {
        EndPointerEnter();
    }
}
