using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardOnBattlePresenter : LanguageChangablePresenter, IAbilityAnimatable
{
    Player _lookingPlayer;
    Card _model;
    CardOnBattleView _view;

    UnityAction<Card> _onPointerEnterAction;
    UnityAction<Card> _onPointerExitAction;

    PointerEnterHandler _pointerEnterHandler;

    public int CardCode => _model.CardInfo.UniqueCode;

    int _cardCount = 0;

    public bool IsHidden { get; private set; } = false;

    public CardOnBattlePresenter(Player lookingPlayer, Card model, CardOnBattleView view,
        UnityAction<Card> onPointerEnterAction, UnityAction<Card> onPointerExitAction) : base()
    {
        _lookingPlayer = lookingPlayer;
        _model = model;
        _view = view;
        _onPointerEnterAction = onPointerEnterAction;
        _onPointerExitAction = onPointerExitAction;

        _pointerEnterHandler = _view.gameObject.GetOrAddComponent<PointerEnterHandler>();

        RegisterPointerEnterHandler();

        _cardCount = 1;

        Start();
    }


    void Start()
    {
        UpdateLanguageTexts();
        UpdateCardCountText();
        UpdateSpeciesColor();
    }


    // ----- Update View ----- //
    protected override void UpdateLanguageTexts()
    {
        _view.SetCardNameText(s_LanguageManager.GetString(_model.CardInfo.CardNameCode));
    }
    void UpdateCardCountText()
    {
        if (_lookingPlayer == _model.Player)
            _view.SetCardCountText($" {_cardCount} X");
        else
            _view.SetCardCountText($"X {_cardCount} ");
    }
    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.Player.SpeciesColor);
    }
    // ----- Update View ----- //

    // ----- Control ----- //
    public void AddCardCount()
    {
        _cardCount++;
        UpdateCardCountText();
    }
    public void SetCardCount(int val)
    {
        _cardCount = val;
        UpdateCardCountText();
    }
    public void ToggleActiveEffect(bool isActive)
    {
        _view.SetDistortedOutlineActive(isActive);
    }
    public void SetAbilityEffectTarget(Transform target)
    {
        _view.SetAbilityEffectTarget(target);
    }
    public void ToggleAbilityEffect(bool isActive)
    {
        _view.PlayAbilityEffect(isActive);
    }

    public void PlayFadeOutAnim(float animTime)
    {
        _view.PlayFadeOutAnim(animTime);
    }
    public void PlayFadeInAnim(float animTime)
    {
        _view.PlayFadeInAnim(animTime);
    }
    public void Hide(bool isHidden)
    {
        IsHidden = isHidden;
        _view.gameObject.SetActive(!IsHidden);
    }

    // ----- Control ----- //

    // ----- Register View Actions ----- //
    void RegisterPointerEnterHandler()
    {
        _pointerEnterHandler.Clear();
        _pointerEnterHandler.onPointerEntered += OnPointerEnter;
        _pointerEnterHandler.onPointerExited += OnPointerExit;
    }
    void OnPointerEnter()
    {
        _onPointerEnterAction?.Invoke(_model);
    }
    void OnPointerExit()
    {
        _onPointerExitAction?.Invoke(_model);
    }
    // ----- Register View Actions ----- //


    public override void Clear()
    {
        base.Clear();

        _pointerEnterHandler.Clear();

        _view.gameObject.DestroyOrReturnToPool();
    }

    //~CardOnBattlePresenter()
    //{
    //    Debug.Log("º“∏Í¿⁄ »£√‚!");
    //}
}
