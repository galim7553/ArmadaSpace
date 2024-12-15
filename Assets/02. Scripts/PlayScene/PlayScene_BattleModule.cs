using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayScene_Controller))]
public class PlayScene_BattleModule : MonoBehaviour
{
    const float BATTLE_WARNING_ALARM_DURATION = 4.0F;

    // ----- Battle ----- //
    Queue<Battle> _battleQueue;
    Coroutine _curExecuteBattlesCo;

    BattlePresenter[] _battlePresenters = new BattlePresenter[War.PLAYER_NUM];
    int _battleAnimCounter = 0;
    // ----- Battle ----- //

    // ----- Alarm ----- //
    [SerializeField] GameObject _eventSystemGo;
    [SerializeField] GameObject _alarmCanvas;
    WaitForSeconds _waitForAlarm = new WaitForSeconds(BATTLE_WARNING_ALARM_DURATION);
    // ----- Alarm ----- //

    // ----- Callback ----- //
    UnityAction _battlesStartedAction;
    UnityAction _battlesEndedAction;
    // ----- Callback ----- //


    public void Init(BattlePresenter[] battlePresenters, UnityAction startBattlesAction, UnityAction endBattlesAction)
    {
        _battlePresenters = battlePresenters;
        _battlesStartedAction = startBattlesAction;
        _battlesEndedAction = endBattlesAction;
    }


    public void StartBattles(Queue<Battle> battleQueue)
    {
        _battleQueue = battleQueue;

        if (_battleQueue.Count <= 0)
            return;

        if (_curExecuteBattlesCo != null)
        {
            StopCoroutine(_curExecuteBattlesCo);
            _curExecuteBattlesCo = null;
        }
        _curExecuteBattlesCo = StartCoroutine(ExecuteBattlesCo());
    }

    IEnumerator ExecuteBattlesCo()
    {
        yield return new WaitForSeconds(PlayScene_Controller.ALRAM_DURATION);

        _battlesStartedAction?.Invoke();
        yield return ShowBattleAlarmCo();

        ExecuteNextBattle();
    }

    IEnumerator ShowBattleAlarmCo()
    {
        _eventSystemGo.gameObject.SetActive(false);
        _alarmCanvas.SetActive(true);
        yield return _waitForAlarm;
        _eventSystemGo.gameObject.SetActive(true);
        _alarmCanvas.SetActive(false);
    }

    void ExecuteNextBattle()
    {
        // 다음 Battle이 없으면
        if(_battleQueue.Count <= 0)
        {
            EndBattles();
        }
        // 다음 Battle이 있으면
        else
        {
            Battle battle = _battleQueue.Dequeue();
            battle.ExecuteBeforeBattle();
            battle.ExecuteAfterBattle();
            battle.ExecutePostBattle();
            StartBattleAnim(battle);
        }
    }
    void StartBattleAnim(Battle battle)
    {
        _battleAnimCounter = 0;
        foreach (BattlePresenter battlePresenter in _battlePresenters)
        {
            battlePresenter.EnterBattleMode(battle);
            battlePresenter.PlayBattleAnim(EndBattleAnim);
        }
    }
    void EndBattleAnim()
    {
        _battleAnimCounter++;
        if(_battleAnimCounter >= War.PLAYER_NUM)
        {
            foreach(BattlePresenter battlePresenter in _battlePresenters)
                battlePresenter.ExitBattleMode();
            ExecuteNextBattle();
        }
    }

    void EndBattles()
    {
        foreach (BattlePresenter battlePresenter in _battlePresenters)
            battlePresenter.ExitBattleMode();

        Debug.Log("모든 전투 끝");
        _battlesEndedAction?.Invoke();
    }
}