using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterCardPresenter : LanguageChangablePresenter, IAbilityAnimatable
{
    Player _lookingPlayer;
    Player _onwerPlayer;
    string _cardNameCode;
    int _cardCount;
    Card _model;

    CardOnBattleView _view;

    public SupporterCardPresenter(Player lookingPlayer, Card model, int count, CardOnBattleView view) : base()
    {
        _lookingPlayer = lookingPlayer;
        _model = model;
        _onwerPlayer = _model.Player;
        _cardNameCode = _model.CardInfo.CardNameCode;
        _cardCount = count;
        _view = view;

        Start();
    }
    public SupporterCardPresenter(Player lookingPlayer, Player ownerPlayer, string cardNameCode, CardOnBattleView view) : base()
    {
        _lookingPlayer = lookingPlayer;
        _onwerPlayer = ownerPlayer;
        _cardNameCode = cardNameCode;
        _cardCount = 1;
        _view = view;

        Start();
    }

    void Start()
    {
        UpdateCardNameText();
        UpdateCardCountText();
        UpdateSpeciesColor();
    }

    // ----- Update View ----- //
    void UpdateCardNameText()
    {
        _view.SetCardNameText(s_LanguageManager.GetString(_cardNameCode));
    }
    void UpdateCardCountText()
    {
        if (_lookingPlayer == _onwerPlayer)
            _view.SetCardCountText($" {_cardCount} X");
        else
            _view.SetCardCountText($"X {_cardCount} ");
    }
    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_onwerPlayer.SpeciesColor);
    }

    protected override void UpdateLanguageTexts()
    {
        UpdateCardNameText();
    }
    // ----- Update View ----- //

    // ----- Control ----- //
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
        _view.gameObject.SetActive(!isHidden);
    }
    // ----- Control ----- //

    public override void Clear()
    {
        base.Clear();

        _view.gameObject.DestroyOrReturnToPool();
    }
}
