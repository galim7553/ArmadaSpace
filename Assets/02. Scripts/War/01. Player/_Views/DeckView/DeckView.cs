using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckView : RootBase
{ 
    public enum FilterDeckButtonType
    {
        NextFilterDeckButton,
        FilterDeckButton_Planet,
        FilterDeckButton_Battleship,
        FilterDeckButton_Soldier,
        FilterDeckButton_Factory
    }
    enum Images
    {
        NextFilterDeckButton,
        Background,
        FilterDeckButton_Planet,
        FilterDeckButton_Battleship,
        FilterDeckButton_Soldier,
        FilterDeckButton_Factory,
        CardBehindSpeciesMark
    }
    enum Transforms
    {
        DeckContent,
    }
    enum TMPs
    {
        DeckCardNumText
    }
    enum GameObjects
    {
        SleepModeHiddenObject
    }
    enum Buttons
    {
        GraveButton
    }

    // ----- Grave Button ----- //
    public event UnityAction onGraveButtonClicked;
    // ----- Grave Button ----- //

    // ===== Filtering Deck ===== //
    public event UnityAction<CardType> onFilterDeckButtonClicked;
    FilterDeckButtonType _curFilterDeckButtonType = FilterDeckButtonType.NextFilterDeckButton;

    // ----- 외부 클릭 시 FilterDeckButtons를 꺼 주기 위한 변수 ----- //
    bool _isFilterDeckButtonsHidden = true;
    PointerEventData _pointerEventData;
    List<RaycastResult> _rayCastResults = new List<RaycastResult>();
    // ----- 외부 클릭 시 FilterDeckButtons를 꺼 주기 위한 변수 ----- //

    // ----- FilterDeckButtonResource ----- //
    Sprite[] _filterDeckButtonSprites = new Sprite[(int)FilterDeckButtonType.FilterDeckButton_Factory + 1];
    // ----- FilterDeckButtonResource ----- //
    // ===== Filtering Deck ===== //

    private void Awake()
    {
        Bind<Transform>(typeof(Transforms));
        Bind<FilterDeckButton>(typeof(FilterDeckButtonType));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.GraveButton).onClick.AddListener(() => onGraveButtonClicked?.Invoke());
        SetFilterDeckButtons();
    }

    private void Update()
    {
        if (_isFilterDeckButtonsHidden == true)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            _rayCastResults.Clear();
            EventSystem.current.RaycastAll(_pointerEventData, _rayCastResults);
            if (_rayCastResults != null && _rayCastResults.Count > 0)
            {
                if (_rayCastResults[0].gameObject.name.Contains("FilterDeckButton") == false)
                    HideFilterDeckButtons();
            }
            else
                HideFilterDeckButtons();
        }
    }

    // ----- Filtering Deck ----- //
    void SetFilterDeckButtons()
    {
        FilterDeckButton filterDeckButton;
        for(int i = (int)FilterDeckButtonType.NextFilterDeckButton; i <= (int)FilterDeckButtonType.FilterDeckButton_Factory; i++)
        {
            filterDeckButton = Get<FilterDeckButton>(i);
            filterDeckButton.SetFilterDeckButton((FilterDeckButtonType)i, this);
            if(i != (int)FilterDeckButtonType.NextFilterDeckButton)
                _filterDeckButtonSprites[i] = filterDeckButton.gameObject.FindChild<Image>("Icon", false).sprite;                
        }
    }
    public void UpdateNextFilterDeckButton(CardType cardType)
    {
        if (cardType == CardType.Leader_Card || cardType == CardType.Count)
            return;

        _curFilterDeckButtonType = (FilterDeckButtonType)cardType;

        GetImage((int)Images.NextFilterDeckButton).sprite = _filterDeckButtonSprites[(int)_curFilterDeckButtonType];
    }
    public void SetCurFilterDeckButtonType(FilterDeckButtonType filterDeckButtonType)
    {
        if(filterDeckButtonType == FilterDeckButtonType.NextFilterDeckButton)
        {
            int index = (int)_curFilterDeckButtonType;
            index = (index + 1) % ((int)FilterDeckButtonType.FilterDeckButton_Factory + 1);
            _curFilterDeckButtonType = (FilterDeckButtonType)index;
            if (_curFilterDeckButtonType == FilterDeckButtonType.NextFilterDeckButton)
                _curFilterDeckButtonType = FilterDeckButtonType.FilterDeckButton_Planet;
        }
        else
        {
            _curFilterDeckButtonType = filterDeckButtonType;
        }
        onFilterDeckButtonClicked?.Invoke((CardType)_curFilterDeckButtonType);
    }
    public void ShowFilterDeckButtons(bool forceShow = false)
    {
        if (_isFilterDeckButtonsHidden == true && forceShow == false)
            return;

        GameObject go = null;
        for(int i = (int)FilterDeckButtonType.FilterDeckButton_Planet; i <= (int)FilterDeckButtonType.FilterDeckButton_Factory; i++)
        {
            go = Get<FilterDeckButton>(i).gameObject;
            if (i == (int)_curFilterDeckButtonType)
                go.SetActive(false);
            else
                go.SetActive(true);
        }
        _isFilterDeckButtonsHidden = false;
    }
    void HideFilterDeckButtons()
    {
        for (int i = (int)FilterDeckButtonType.FilterDeckButton_Planet; i <= (int)FilterDeckButtonType.FilterDeckButton_Factory; i++)
            Get<FilterDeckButton>(i).gameObject.SetActive(false);
        _isFilterDeckButtonsHidden = true;
    }

    public void AddCardOnDeckView(CardView cardView)
    {
        cardView.transform.SetParent(Get<Transform>((int)Transforms.DeckContent), false);
        cardView.transform.SetAsLastSibling();
    }
    // ----- Filtering Deck ----- //


    public void UpdateDeckCardNumText(int value)
    {
        Get<TextMeshProUGUI>((int)TMPs.DeckCardNumText).text = value.ToString();
    }

    // ----- SpeciesColor ----- //
    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.Background).color = color;
        GetImage((int)Images.FilterDeckButton_Planet).color = color;
        GetImage((int)Images.FilterDeckButton_Battleship).color = color;
        GetImage((int)Images.FilterDeckButton_Soldier).color = color;
        GetImage((int)Images.FilterDeckButton_Factory).color = color;
    }
    // ----- SpeciesColor ----- //

    public void SetSleepModeHiddenObjectActive(bool isActive)
    {
        Get<GameObject>((int)GameObjects.SleepModeHiddenObject).SetActive(isActive);
    }

    public void SetCardBehindSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.CardBehindSpeciesMark).sprite = sprite;
    }
}
