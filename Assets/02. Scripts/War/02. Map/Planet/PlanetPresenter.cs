using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetPresenter : PresenterBase
{
    const float TRANSLUCENT_ALPHA = 0.6078F;

    // ----- Reference ----- //
    Planet _model;
    PlanetView _view;
    UnityAction<Planet> _onViewClickedAction;
    UnityAction<Planet> _onPointerEnteredAction;
    UnityAction _onPointerExitedAction;

    PointerClickHandler _pointerClickHandler;
    PointerEnterHandler _pointerEnterHandler;
    // ----- Reference ----- //

    // ----- Looking Player ----- //
    Player LookingPlayer => _model.RootMap.War.CurPlayer;
    // ----- Looking Player ----- //

    public PlanetPresenter(Planet model, PlanetView view, UnityAction<Planet> onViewClickedAction, UnityAction<Planet> onPointerEnteredAction, UnityAction onPointerExitedAction)
    {
        // ----- Reference ----- //
        _model = model;
        _view = view;
        _pointerClickHandler = _view.gameObject.GetOrAddComponent<PointerClickHandler>();
        _onViewClickedAction = onViewClickedAction;
        _pointerEnterHandler = _view.gameObject.GetOrAddComponent<PointerEnterHandler>();
        _onPointerEnteredAction = onPointerEnteredAction;
        _onPointerExitedAction = onPointerExitedAction;
        // ----- Reference ----- //

        // ----- Event ----- //
        RegisterViewActions();

        SubscribeOwnerChangedEvent();
        SubscribeCardAssignedEvent();
        SubscribeTargetingFleetsChangedEvent();
        // ----- Event ----- //

        // ----- Start ----- //
        Start();
        // ----- Start ----- //
    }

    void Start()
    {
        UpdateView();
        UpdateMainPlanetEffect();
        UpdatePlanetOwnerEffect();
        UpdateBattleCardMark();
    }

    // ----- Update View ----- //
    void UpdateView()
    {
        _view.SetPosition(_model.PlanetInfo.PosX, _model.PlanetInfo.PosY);
        _view.SetPlanetImage(s_ResourceManager.LoadResource<Sprite>(_model.PlanetImagePath));
        _view.SetPlanetNameText(_model.PlanetName);
    }
    void UpdateMainPlanetEffect()
    {
        if (_model.PlanetInfo.PlanetType == PlanetType.Main)
        {
            _view.SetMainPlanetOutlineActive(true);
            _view.SetMainPlanetOutlineColor(_model.OwnerPlayer.SpeciesColor);
        }
        else
            _view.SetMainPlanetOutlineActive(false);
    }
    void UpdatePlanetOwnerEffect()
    {
        if (_model.OwnerPlayer != null)
        {
            _view.SetPlanetCoverImageActive(true);
            _view.SetPlanetConverImageColor(_model.OwnerPlayer.SpeciesColor);
        }
        else
            _view.SetPlanetCoverImageActive(false);
    }
    void UpdateBattleCardMark()
    {
        IReadOnlyList<Card> soldierCards = _model.GetCardsByCardType(CardType.Soldier_Card);
        IReadOnlyList<Card> battleshipCards = _model.GetCardsByCardType(CardType.Battleship_Card);
        if (soldierCards.Count <= 0 && battleshipCards.Count <= 0)
            _view.SetBattleCardMarkActive(false);
        else
        {
            _view.SetBattleCardMarkActive(true);
            if (soldierCards.Count > 0)
                _view.SetBattleCardMark(soldierCards[0].Player.SpeciesMarkSprite);
            if (battleshipCards.Count > 0)
                _view.SetBattleCardMark(battleshipCards[0].Player.SpeciesMarkSprite);
        }
    }

    void UpdateSpinImage()
    {
        if (LookingPlayer == null)
        {
            _view.SetInnerSpinImageActive(false);
            _view.SetOuterSpinImageActive(false);
            return;
        }
            
        IReadOnlyList<Fleet> playerFleets = _model.GetTargetingFleets(LookingPlayer.Index);
        if (playerFleets != null && playerFleets.Count > 0)
        {
            _view.SetInnerSpinImageColor(LookingPlayer.SpeciesColor);
            _view.SetInnerSpinImageActive(true);
        }
        else
            _view.SetInnerSpinImageActive(false);

        Player enemyPlayer = LookingPlayer.OpponentPlayer;
        if (enemyPlayer != null)
        {
            IReadOnlyList<Fleet> enemyFleets = _model.GetTargetingFleets(enemyPlayer.Index);
            if (enemyFleets != null && enemyFleets.Count > 0)
            {
                _view.SetOuterSpinImageColor(enemyPlayer.SpeciesColor);
                _view.SetOuterSpinImageActive(true);
            }
            else
                _view.SetOuterSpinImageActive(false);
        }
        else
            _view.SetOuterSpinImageActive(false);
    }
    // ----- Update View ----- //

    // ----- Control ----- //
    public void EnterMoveFleetMode(Player player, Airway airway, UnityAction<Airway, Planet> callback)
    {
        _view.SetSpinImageAlpha(1.0f);
        _view.SetInnerSpinImageColor(player.SpeciesColor);
        _view.SetInnerSpinImageActive(true);
        UnityAction buttonCallback = () => { callback(airway, _model); };
        _view.SetMoveFleetButton($"{airway.GetPhaseCount(player)}P", buttonCallback);
        _view.SetMoveFleetButtonActive(true);
    }
    public void ExitMoveFleetMode()
    {
        _view.SetSpinImageAlpha(1.0f);
        UpdateSpinImage();
        _view.SetMoveFleetButtonActive(false);
    }

    public void OnTurnChanged()
    {
        UpdateSpinImage();
    }

    public void SetSpinImageTranslucent(bool translucent)
    {
        _view.SetSpinImageAlpha(translucent ? TRANSLUCENT_ALPHA : 1.0f);
    }
    // ----- Control ----- //

    // ----- Register View Action ----- //
    void RegisterViewActions()
    {
        _pointerClickHandler.Clear();
        _pointerClickHandler.onPointerClicked += OnViewClicked;

        _pointerEnterHandler.Clear();
        _pointerEnterHandler.onPointerEntered += OnPointerEntered;
        _pointerEnterHandler.onPointerExited += OnPointerExited;
        
    }
    void OnViewClicked()
    {
        _onViewClickedAction?.Invoke(_model);
    }
    void OnPointerEntered()
    {
        _onPointerEnteredAction?.Invoke(_model);
    }
    void OnPointerExited()
    {
        _onPointerExitedAction?.Invoke();
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void SubscribeOwnerChangedEvent()
    {
        _model.OnOwnerChanged += OnOwnerChanged;
    }
    void SubscribeCardAssignedEvent()
    {
        _model.OnCardAdded += OnCardChanged;
        _model.OnCardRemoved += OnCardChanged;
    }
    void SubscribeTargetingFleetsChangedEvent()
    {
        _model.OnTargetingFleetsChanged += OnTargetingFleetsChanged;
    }
    void OnOwnerChanged()
    {
        UpdatePlanetOwnerEffect();
    }
    void OnCardChanged(Card card)
    {
        UpdateBattleCardMark();
    }
    void OnTargetingFleetsChanged()
    {
        UpdateSpinImage();
    }
    // ----- Event ----- //

    public void Clear()
    {
        _pointerClickHandler.Clear();
        _pointerEnterHandler.Clear();
    }
}
