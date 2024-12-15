using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedCardViewHandler : MonoBehaviour
{
    EventSystem _eventSystem;
    PointerEventData _eventData;
    List<RaycastResult> _raycastResults = new List<RaycastResult>();

    bool _prevIsInDropArea = false;
    bool _curIsInDropArea = false;

    SelectedCardPresenter.DropAreaType _dropAreaType;

    public event UnityAction onDropAreaEntered;
    public event UnityAction onDropAreaExited;

    Image _outlineImage;
    Material _outlineMaterial;

    Vector3 _pos;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _eventData = new PointerEventData(_eventSystem);

        _outlineImage = gameObject.FindChild<Image>("OutlineImage");
        _outlineImage.material = new Material(_outlineImage.material);
        _outlineMaterial = _outlineImage.material;
    }
    private void OnEnable()
    {
        SetPosition(Input.mousePosition);
        _prevIsInDropArea = false;
        _curIsInDropArea = false;
    }

    void SetPosition(Vector3 mousePos)
    {
        _pos = Camera.main.ScreenToWorldPoint(mousePos);
        _pos.z = transform.position.z;
        transform.position = _pos;
    }

    public void SetTargetDropArea(SelectedCardPresenter.DropAreaType dropAreaType)
    {
        _dropAreaType = dropAreaType;
    }
    public void AddDropAreaAction(UnityAction enterDropAreaAction, UnityAction exitDropAreaAction)
    {
        onDropAreaEntered += enterDropAreaAction;
        onDropAreaExited += exitDropAreaAction;
    }


    private void Update()
    {
        SetPosition(Input.mousePosition);
        _raycastResults.Clear();
        _eventData.position = Input.mousePosition;
        _eventSystem.RaycastAll(_eventData, _raycastResults);
        _curIsInDropArea = false;
        foreach(RaycastResult raycastResult in _raycastResults)
        {
            if (raycastResult.gameObject.CompareTag(_dropAreaType.ToString()))
            {
                _curIsInDropArea = true;
                break;
            }
        }
        if(_curIsInDropArea == true)
        {
            if (_prevIsInDropArea == false)
                onDropAreaEntered?.Invoke();
        }
        else
        {
            if(_prevIsInDropArea == true)
                onDropAreaExited?.Invoke();
        }
        _prevIsInDropArea = _curIsInDropArea;
    }

    public void SetOutlineActive(bool isActive)
    {
        _outlineImage.gameObject.SetActive(isActive);
    }
    public void SetSpeciesColor(Color color)
    {
        _outlineMaterial.SetColor("_OutlineColor", color);
    }

    public void Clear()
    {
        onDropAreaEntered = null;
        onDropAreaExited = null;
    }
}
