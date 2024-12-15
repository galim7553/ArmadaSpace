using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event UnityAction onDragBegan;
    public event UnityAction OnDragEnded;

    ScrollRect _parentScrollRect;
    bool _isActive = false;
    bool _isDragging = false;

    bool _isHidable = false;
    Transform _parent;
    int _originChildIndex;

    PointerEventData.InputButton _inputButton;

    public void Init(bool isHidable)
    {
        _isHidable = isHidable;
        _isDragging = false;

        _parentScrollRect = null;
    }

    public void SetIsActive(bool isActive)
    {
        _isActive = isActive;
    }


    void LateUpdate()
    {
        if(_isDragging == true && Input.GetMouseButton((int)_inputButton) == false)
        {
            EndDrag();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _inputButton = eventData.button;
        BeginDrag();
    }

    public void BeginDrag()
    {
        if (_isActive == false)
            return;

        if (_parentScrollRect == null)
            _parentScrollRect = GetComponentInParent<ScrollRect>();

        if (_parentScrollRect != null)
            _parentScrollRect.enabled = false;

        if(_isHidable == true)
        {
            _parent = transform.parent;
            _originChildIndex = transform.GetSiblingIndex();

            transform.SetParent(null, false);
        }


        onDragBegan?.Invoke();

        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndDrag();
    }
    public void EndDrag()
    {
        if (_isDragging == false)
            return;

        if (_parentScrollRect != null)
            _parentScrollRect.enabled = true;

        if(_isHidable == true)
        {
            transform.SetParent(_parent, false);
            transform.SetSiblingIndex(_originChildIndex);
        }

        OnDragEnded?.Invoke();

        _isDragging = false;
    }

    public void Clear()
    {
        onDragBegan = null;
        OnDragEnded = null;
    }
}