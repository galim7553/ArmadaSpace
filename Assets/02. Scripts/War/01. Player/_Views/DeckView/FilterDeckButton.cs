using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FilterDeckButton : MonoBehaviour, IPointerClickHandler
{

    DeckView _deckView;
    DeckView.FilterDeckButtonType _filterDeckButtonType;


    public void SetFilterDeckButton(DeckView.FilterDeckButtonType filterDeckButtonType, DeckView deckView)
    {
        _deckView = deckView;
        _filterDeckButtonType = filterDeckButtonType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_deckView == null)
            {
                Debug.LogError($"DeckView is missing at FilterDeckButton GameObject name : {gameObject.name}");
                return;
            }
            _deckView.SetCurFilterDeckButtonType(_filterDeckButtonType);
            _deckView.ShowFilterDeckButtons();
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (_filterDeckButtonType == DeckView.FilterDeckButtonType.NextFilterDeckButton)
                _deckView.ShowFilterDeckButtons(true);
        }

    }
}
