using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattlePresenter : RootPresenterBase
{
    public const float START_ANIM_TIME = 1.0F;
    public const float BATTLE_CARDS_APPEAR_ANIM_TIME = 1.5F;
    public const float BATTLE_CARDS_DAMAGE_ANIM_AFTER_WAIT = 0.5F;
    public const float BATTLE_CARDS_BONUS_DAMAGE_ANIM_AFTER_TIME = 0.5F;
    public const float SUPPORTER_CARDS_BONUS_DAMAGE_ANIM_AFTER_TIME = 1.0F;
    public const float WINNER_N_DAMAGED_ANIM_TIME = 2.0F;
    public const float WINNER_ANIM_TEXT_ANIM_TIME = 1.0F;
    public const float WINNER_ANIM_TEXT_SCALE_FACTOR = 1.3F;
    public const float DESTROY_ANIM_TIME = 1.0F;
    public const float DESTORY_ANIM_AFTER_TIME = 1.0F;
    public const float RESULT_ANIM_AFTER_TIME = 1.0F;
    public const float AFTER_BATTLE_COMMAND_ANIMS_AFTER_TIME = 1.0F;

    public const float FADE_ANIM_TIME = 0.5F;



    PlayerPresenter _playerPresenter;

    public Player Model { get; private set; }
    BattleView _view;

    public Battle CurBattle {  get; private set; }
    public BattleSquad CurPlayerBattleSquad => CurBattle == null ? null : CurBattle.BattleSquads[Model.Index];
    public BattleSquad CurEnemyBattleSquad => CurBattle == null ? null : CurBattle.BattleSquads[Model.OpponentPlayer.Index];

    // ----- Presenter Modules ----- //
    BattleInfoPresenter _battleInfoPresenter;
    BattleStatPresenter _battleStatPresenter;
    BattleMapPresenter _battleMapPresenter;
    BattleCardsAreaPresenter[] _battleCardsAreaPresenters = new BattleCardsAreaPresenter[War.PLAYER_NUM];
    BattleCardsAreaPresenter PlayerBattleCardsAreaPresenter => _battleCardsAreaPresenters[Model.Index];
    BattleCardsAreaPresenter EnemyBattleCardsAreaPresenter => _battleCardsAreaPresenters[Model.OpponentPlayer.Index];

    SupporterCardsAreaPresenter[] _supporterCardsAreaPresenters = new SupporterCardsAreaPresenter[War.PLAYER_NUM];
    SupporterCardsAreaPresenter PlayerSupporterCardsAreaPresenter => _supporterCardsAreaPresenters[Model.Index];
    SupporterCardsAreaPresenter EnemySupporterCardsAreaPresenter => _supporterCardsAreaPresenters[Model.OpponentPlayer.Index];

    BattleResultPresenter _battleResultPresenter;

    public BattleStatPresenter BattleStatPresenter => _battleStatPresenter;
    // ----- Presenter Modules ----- //

    // ----- ViewFactory ----- //
    ViewFactory<CardOnBattleView> _playerCobvFactory;
    ViewFactory<CardOnBattleView> _enemyCobvFactory;

    ViewFactory<CardOnBattleView> _playerSupporterCardOnBattleViewFactory;
    ViewFactory<CardOnBattleView> _enemySupporterCardOnBattleViewFactory;
    // ----- ViewFactory ----- //

    // ----- Card ToolTip ----- //
    CardToolTipHandler _playerCardToolTipHandler;
    CardToolTipHandler _enemyCardToolTipHandler;
    // ----- Card ToolTip ----- //

    // ----- Ability Effect Target ----- //
    Transform[] _abilityEffectTargets = new Transform[War.PLAYER_NUM];
    public IReadOnlyList<Transform> AbilityEffectTargets => _abilityEffectTargets;
    // ----- Ability Effect Target ----- //

    public BattlePresenter(PlayerPresenter playerPresenter, Player model, BattleView view)
    {
        _playerPresenter = playerPresenter;
        Model = model;
        _view = view;

        CreateFactories();

        _view.gameObject.SetActive(true);

        _abilityEffectTargets[Model.Index] = _view.BattleStatView.PlayerDamageTextTransform;
        _abilityEffectTargets[Model.OpponentPlayer.Index] = _view.BattleStatView.EnemyDamageTextTransform;

        _battleInfoPresenter = new BattleInfoPresenter(this, _view.BattleInfoView);
        _battleStatPresenter = new BattleStatPresenter(this, _view.BattleStatView);
        _battleMapPresenter = new BattleMapPresenter(this, _view.BattleMapView);

        _battleCardsAreaPresenters[Model.Index] = new BattleCardsAreaPresenter(this, _view.PlayerBattleCardsAreaView, _playerCobvFactory, _abilityEffectTargets[Model.Index]);
        _battleCardsAreaPresenters[Model.OpponentPlayer.Index] = new BattleCardsAreaPresenter(this, _view.EnemyBattleCardsAreaView, _enemyCobvFactory, _abilityEffectTargets[Model.OpponentPlayer.Index]);

        _supporterCardsAreaPresenters[Model.Index] = new SupporterCardsAreaPresenter(this, _view.PlayerSupporterCardsAreaView,
            _playerSupporterCardOnBattleViewFactory);
        _supporterCardsAreaPresenters[Model.OpponentPlayer.Index] = new SupporterCardsAreaPresenter(this, _view.EnemySupporterCardsAreaView,
            _enemySupporterCardOnBattleViewFactory);

        _battleResultPresenter = new BattleResultPresenter(this, _view.BattleResultView);

        InitCardToolTipHandlers();

        _view.gameObject.SetActive(false);
    }

    void CreateFactories()
    {
        _playerCobvFactory = new ViewFactory<CardOnBattleView>(s_PoolManager, "PlayerCardOnBattleView");
        _enemyCobvFactory = new ViewFactory<CardOnBattleView>(s_PoolManager, "EnemyCardOnBattleView");

        _playerSupporterCardOnBattleViewFactory = new ViewFactory<CardOnBattleView>(s_PoolManager, "PlayerSupporterCardOnBattleView");
        _enemySupporterCardOnBattleViewFactory = new ViewFactory<CardOnBattleView>(s_PoolManager, "EnemySupporterCardOnBattleView");
    }

    public void EnterBattleMode(Battle battle)
    {
        CurBattle = battle;

        _view.gameObject.SetActive(true);
    }
    public void ExitBattleMode()
    {
        CurBattle = null;

        _view.gameObject.SetActive(false);
    }


    // ----- Card ToolTip ----- //
    void InitCardToolTipHandlers()
    {
        _playerCardToolTipHandler = _view.PlayerCardToolTipHandler;
        _enemyCardToolTipHandler = _view.EnemyCardToolTipHandler;
        _playerCardToolTipHandler.Init(_view.PlayerCardOnToolTipView, Model.SpeciesColor);
        _enemyCardToolTipHandler.Init(_view.EnemyCardOnToolTipView, Model.OpponentPlayer.SpeciesColor);
    }

    public void ForceShowToolTip(Card card)
    {
        ForceShowToolTip(card, true);
    }
    public void ForceShowToolTip(Card card, bool isActive)
    {
        CardToolTipHandler cardToolTipHandler = Model == card.Player ? _playerCardToolTipHandler : _enemyCardToolTipHandler;
        if (isActive == true)
            cardToolTipHandler.ForceShowToolTip(card);
        else
            cardToolTipHandler.ForceShowToolTip(null);
    }
    void SleepCardToolTipHandlers(bool isSleep)
    {
        _playerCardToolTipHandler.SetSleepMode(isSleep);
        _enemyCardToolTipHandler.SetSleepMode(isSleep);
    }

    public void OnCardPointerEnterAction(Card card)
    {
        if (Model == card.Player)
            _playerCardToolTipHandler.ShowToolTip(card);
        else
            _enemyCardToolTipHandler.ShowToolTip(card);
    }
    public void OnCardPointerExitAction(Card card)
    {
        if (Model == card.Player)
            _playerCardToolTipHandler.ShowToolTip(null);
        else
            _enemyCardToolTipHandler.ShowToolTip(null);
    }
    // ----- Card ToolTip ----- //

    // ----- Animation ----- //
    public void PlayBattleAnim(UnityAction callback)
    {
        TaskSequencer sequencer = new TaskSequencer(_view, callback);

        sequencer.AddAction(() => ReadyBattleAnim())  // 전투 애니메이션 시작 준비
                 .AddAction(() => PlayStartAnim())    // 전투 시작 애니메이션 재생
                 .AddWait(START_ANIM_TIME)                       // 1초 대기

                 .AddAction(() => PlayBattleCardsAppearAnim())  // 전투 카드 등장 애니메이션 재생
                 .AddAction(() => PlayBattleMapUnitsAppearAnim()) // 전투 맵 유닛 등장 애니메이션 재생
                 .AddWait(BATTLE_CARDS_APPEAR_ANIM_TIME)                      // 1.5초 대기

                 .AddCoroutine(PlayBattleCardsDamageAnimCo())  // 전투 카드 대미지 합산 애니메이션 재생
                 .AddWait(BATTLE_CARDS_DAMAGE_ANIM_AFTER_WAIT)                     // 0.5초 대기

                 .AddAction(() => SleepCardToolTipHandlers(true))  // 카드 툴팁 기능 슬립

                 .AddCoroutine(PlayBattleCardsBonusDamageAnimsCo())  // 전투 카드 보너스 대미지 애니메이션 순차 재생
                 .AddWait(BATTLE_CARDS_BONUS_DAMAGE_ANIM_AFTER_TIME)  // 0.5초 대기

                 .AddCoroutine(PlaySupporterCardsBonusDamageAnimsCo())  // 지원 카드 보너스 대미지 애니메이션 순차 재생
                 .AddWait(SUPPORTER_CARDS_BONUS_DAMAGE_ANIM_AFTER_TIME)  // 1초 대기

                 .AddAction(() => _battleStatPresenter.PlayWinnerAnim())  // 승자 애니메이션 재생
                 .AddAction(() => _battleMapPresenter.PlayDamagedAnim())  // 패자 피격 애니메이션 재생
                 .AddWait(WINNER_N_DAMAGED_ANIM_TIME)  // 2초 대기

                 .AddAction(() => PlayDestroyAnim())  // 패자 파괴 애니메이션 재생
                 .AddWait(DESTROY_ANIM_TIME)  // 1초 대기
                 .AddAction(() => HideLoserBattleCards())   // 패자 전투 카드 숨김 처리
                 .AddWait(DESTORY_ANIM_AFTER_TIME)  // 1초 대기


                 .AddAction(() => _battleResultPresenter.PlayAppearAnim()) // 결과 애니메이션 재생
                 .AddWait(RESULT_ANIM_AFTER_TIME)   // 1초 대기

                 .AddCoroutine(PlayBattleCardAfterBattleCommandAnimsCo()) // 전투 카드 전투 후 효과 애니메이션 재생
                 .AddCoroutine(PlaySupporterCardAfterBattleCommandAnimsCo())     // 지원 카드 전투 후 효과 애니메이션 재생
                 .AddWait(AFTER_BATTLE_COMMAND_ANIMS_AFTER_TIME)

                 .AddAction(() => OnBattleAnimEnded())  // 종료 처리
                 .Play();
    }
    void ReadyBattleAnim()
    {
        SleepCardToolTipHandlers(false);
        _battleInfoPresenter.UpdateView();
        _battleStatPresenter.UpdateView();
        _battleMapPresenter.UpdateBattleMap();
        _battleResultPresenter.UpdateView();
    }

    void PlayStartAnim()
    {
        _view.PlayStartAnim();
    }
    void PlayBattleCardsAppearAnim()
    {
        PlayerBattleCardsAreaPresenter.SetBattleSquad(CurPlayerBattleSquad);
        EnemyBattleCardsAreaPresenter.SetBattleSquad(CurEnemyBattleSquad);

        foreach (var bcp in _battleCardsAreaPresenters)
            bcp.PlayAppearAnim();
    }
    void PlayBattleMapUnitsAppearAnim()
    {
        _battleMapPresenter.PlayAppearAnims();
    }
    IEnumerator PlayBattleCardsDamageAnimCo()
    {
        // 전투 카드 활성화 이펙트 표시
        foreach (var bcp in _battleCardsAreaPresenters)
            bcp.ToggleActiveEffects(true);

        yield return new WaitForSeconds(1.0f);

        // 전투 카드 어빌리티 이펙트 표시
        foreach (var bcp in _battleCardsAreaPresenters)
            bcp.ToggleAbilityEffects(true);

        yield return new WaitForSeconds(0.5f);

        // 대미지 상승 애니메이션 재생
        _battleStatPresenter.PlayBattleCardsDamageAnims();
        yield return new WaitForSeconds(0.5f);

        // 이펙트 비활성화
        foreach (var bcp in _battleCardsAreaPresenters)
        {
            bcp.ToggleActiveEffects(false);
            bcp.ToggleAbilityEffects(false);
        }
    }


    IEnumerator PlayBattleCardsBonusDamageAnimsCo()
    {
        int maxCount = 0;
        List<BonusDamageAnim>[] bonusDamageAnims = new List<BonusDamageAnim>[War.PLAYER_NUM];

        for(int i = 0; i < bonusDamageAnims.Length; i++)
        {
            bonusDamageAnims[i] = _battleCardsAreaPresenters[i].ComputeBattleCardsBonusDamageAnims();
            maxCount = Mathf.Max(bonusDamageAnims[i].Count, maxCount);
        }

        BonusDamageAnim[] bonusDamageAnimPair = new BonusDamageAnim[War.PLAYER_NUM];
        for (int i = 0; i < maxCount; i++)
        {
            bonusDamageAnimPair[Model.Index] = i < bonusDamageAnims[Model.Index].Count ? bonusDamageAnims[Model.Index][i] : null;
            bonusDamageAnimPair[Model.OpponentPlayer.Index] = i < bonusDamageAnims[Model.OpponentPlayer.Index].Count ? bonusDamageAnims[Model.OpponentPlayer.Index][i] : null;
            yield return PlayBattleCardsBonusDamageAnimCo(bonusDamageAnimPair);
        }
            
    }
    IEnumerator PlayBattleCardsBonusDamageAnimCo(BonusDamageAnim[] bonusDamageAnims)
    {
        foreach(var bda in bonusDamageAnims)
        {
            if(bda == null) continue;

            bda.ToggleActiveEffect(true);
            bda.ToggleForceShowToolTip(true);
            bda.ToggleAbilityEffect(true);
        }
        yield return new WaitForSeconds(0.5f);

        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.ToggleAbilityEffect(false);
        }

        yield return new WaitForSeconds(0.5f);

        foreach(var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.PlayAddDamageAnim();
        }

        yield return new WaitForSeconds(0.5f);

        foreach(var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.ToggleActiveEffect(false);
            bda.ToggleForceShowToolTip(false);
        }
    }

    IEnumerator PlaySupporterCardsBonusDamageAnimsCo()
    {
        PlayerSupporterCardsAreaPresenter.SetBattleSquad(CurPlayerBattleSquad);
        EnemySupporterCardsAreaPresenter.SetBattleSquad(CurEnemyBattleSquad);


        // 각 SupporterCardsAreaPresenter에서 AbilityEffectAnim 받아 와서 리스트화
        // Pair로 만들어서
        // 개별 Pair 코루틴에서 실행
        int maxCount = 0;
        IReadOnlyList<BonusDamageAnim>[] bonusDamageAnims = new List<BonusDamageAnim>[War.PLAYER_NUM];

        for (int i = 0; i < bonusDamageAnims.Length; i++)
        {
            bonusDamageAnims[i] = _supporterCardsAreaPresenters[i].SupporterCardBonusDamageAnims;
            maxCount = Mathf.Max(bonusDamageAnims[i].Count, maxCount);
        }

        BonusDamageAnim[] bonusDamageAnimPair = new BonusDamageAnim[War.PLAYER_NUM];
        for (int i = 0; i < maxCount; i++)
        {
            bonusDamageAnimPair[Model.Index] = i < bonusDamageAnims[Model.Index].Count ? bonusDamageAnims[Model.Index][i] : null;
            bonusDamageAnimPair[Model.OpponentPlayer.Index] = i < bonusDamageAnims[Model.OpponentPlayer.Index].Count ? bonusDamageAnims[Model.OpponentPlayer.Index][i] : null;
            yield return PlaySupporterCardsBonusDamageAnimCo(bonusDamageAnimPair);
        }
    }

    IEnumerator PlaySupporterCardsBonusDamageAnimCo(BonusDamageAnim[] bonusDamageAnims)
    {
        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.Hide(false);

            bda.ToggleActiveEffect(true);
            bda.ToggleForceShowToolTip(true);
            bda.ToggleAbilityEffect(true);
        }
        yield return new WaitForSeconds(0.5f);

        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.ToggleAbilityEffect(false);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.PlayAddDamageAnim();
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.ToggleActiveEffect(false);
            bda.ToggleForceShowToolTip(false);
            bda.PlayFadeOutAnim();
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var bda in bonusDamageAnims)
        {
            if (bda == null) continue;

            bda.Hide(true);
        }
    }

    void PlayDestroyAnim()
    {
        _battleMapPresenter.PlayResultAnim();

        if(CurBattle.BattleResultType == BattleResultType.Draw)
        {
            foreach (var bcp in _battleCardsAreaPresenters)
                bcp.PlayDestroyAnims();
        }
        else
        {
            _battleCardsAreaPresenters[CurBattle.Loser.Player.Index].PlayDestroyAnims();
        }
    }
    void HideLoserBattleCards()
    {
        if (CurBattle.BattleResultType == BattleResultType.Draw)
        {
            foreach (var bcp in _battleCardsAreaPresenters)
                bcp.HideBattleCards();
        }
        else
        {
            _battleCardsAreaPresenters[CurBattle.Loser.Player.Index].HideBattleCards();
        }
    }

    IEnumerator PlayBattleCardAfterBattleCommandAnimsCo()
    {
        int maxCount = 0;
        List<AfterBattleCommandAnim>[] afterBattleCommandAnims = new List<AfterBattleCommandAnim>[War.PLAYER_NUM];

        for (int i = 0; i < afterBattleCommandAnims.Length; i++)
        {
            afterBattleCommandAnims[i] = _battleCardsAreaPresenters[i].ComputeBattleCardsAfterBattleCommandAnims();
            maxCount = Mathf.Max(afterBattleCommandAnims[i].Count, maxCount);
        }

        if (maxCount > 0)
            yield return new WaitForSeconds(FADE_ANIM_TIME);

        AfterBattleCommandAnim[] afterBattleCommandAnimPair = new AfterBattleCommandAnim[War.PLAYER_NUM];
        for (int i = 0; i < maxCount; i++)
        {
            afterBattleCommandAnimPair[Model.Index] = i < afterBattleCommandAnims[Model.Index].Count ? afterBattleCommandAnims[Model.Index][i] : null;
            afterBattleCommandAnimPair[Model.OpponentPlayer.Index] = i < afterBattleCommandAnims[Model.OpponentPlayer.Index].Count ? afterBattleCommandAnims[Model.OpponentPlayer.Index][i] : null;
            yield return PlayBattleCardAfterBattleCommandAnimCo(afterBattleCommandAnimPair);
        }
    }
    IEnumerator PlayBattleCardAfterBattleCommandAnimCo(AfterBattleCommandAnim[] afterBattleCommandAnims)
    {
        foreach (var abca in afterBattleCommandAnims)
        {
            if (abca == null) continue;

            abca.ToggleActiveEffect(true);
            abca.ToggleForceShowToolTip(true);
        }
        yield return new WaitForSeconds(2.0f);

        foreach (var abca in afterBattleCommandAnims)
        {
            if (abca == null) continue;

            abca.ToggleActiveEffect(false);
            abca.ToggleForceShowToolTip(false);
        }
    }

    IEnumerator PlaySupporterCardAfterBattleCommandAnimsCo()
    {
        // 각 SupporterCardsAreaPresenter에서 AbilityEffectAnim 받아 와서 리스트화
        // Pair로 만들어서
        // 개별 Pair 코루틴에서 실행
        int maxCount = 0;
        IReadOnlyList<AfterBattleCommandAnim>[] afterBattleCommandAnims = new List<AfterBattleCommandAnim>[War.PLAYER_NUM];

        for (int i = 0; i < afterBattleCommandAnims.Length; i++)
        {
            afterBattleCommandAnims[i] = _supporterCardsAreaPresenters[i].SupporterCardAfterBattleCommandAnims;
            maxCount = Mathf.Max(afterBattleCommandAnims[i].Count, maxCount);
        }

        AfterBattleCommandAnim[] afterBattleCommandAnimPair = new AfterBattleCommandAnim[War.PLAYER_NUM];
        for (int i = 0; i < maxCount; i++)
        {
            afterBattleCommandAnimPair[Model.Index] = i < afterBattleCommandAnims[Model.Index].Count ? afterBattleCommandAnims[Model.Index][i] : null;
            afterBattleCommandAnimPair[Model.OpponentPlayer.Index] = i < afterBattleCommandAnims[Model.OpponentPlayer.Index].Count ? afterBattleCommandAnims[Model.OpponentPlayer.Index][i] : null;
            yield return PlaySupporterCardAfterBattleCommandAnimCo(afterBattleCommandAnimPair);
        }
    }
    IEnumerator PlaySupporterCardAfterBattleCommandAnimCo(AfterBattleCommandAnim[] afterBattleCommandAnims)
    {
        foreach (var abca in afterBattleCommandAnims)
        {
            if (abca == null) continue;

            abca.Hide(false);

            abca.ToggleActiveEffect(true);
            abca.ToggleForceShowToolTip(true);
        }
        yield return new WaitForSeconds(2.0f);

        foreach (var abca in afterBattleCommandAnims)
        {
            if (abca == null) continue;

            abca.ToggleActiveEffect(false);
            abca.ToggleForceShowToolTip(false);
            abca.PlayFadeOutAnim();
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var abca in afterBattleCommandAnims)
        {
            if (abca == null) continue;

            abca.Hide(true);
        }
    }


    void OnBattleAnimEnded()
    {
        foreach (var bcp in _battleCardsAreaPresenters)
            bcp.ClearCardOnBattlePresenters();

        foreach (var scp in _supporterCardsAreaPresenters)
            scp.ClearSupporterCardPresenters();
    }
    // ----- Animation ----- //


    public void Clear()
    {
        _enemyCardToolTipHandler.Clear();
        _playerCardToolTipHandler.Clear();

        _battleInfoPresenter.Clear();
        _battleStatPresenter.Clear();
        _battleResultPresenter.Clear();
    }

}