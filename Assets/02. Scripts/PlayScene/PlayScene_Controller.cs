using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayScene_Controller : MonoBehaviour
{
    // ----- Module ----- //
    PlayScene_UIModule _uiModule;
    PlayScene_BattleModule _battleModule;
    public PlayScene_UIModule UIModule => _uiModule;
    public PlayScene_BattleModule BattleModule => _battleModule;

    [Header("---- CameraCtrl -----")]
    [SerializeField] CameraMoveCtrl2D _cameraMoveCtrl;
    [SerializeField] CameraZoomCtrl2D _cameraZoomCtrl;
    // ----- Module ----- //

    // ----- Createad Model ----- //
    public War War {  get; private set; }
    // ----- Createad Model ----- //

    // ----- Presenter ----- //
    WarPresenter _warPresenter;
    // ----- Presenter ----- //

    // ----- OnSceneViews ----- //
    [Header("----- Views -----")]
    [SerializeField] MapView _mapView;
    [SerializeField] PlayerView[] _playerViews;
    [SerializeField] GraveView _graveView;
    [SerializeField] ResultView _resultView;
    [SerializeField] SurrenderView _surrenderView;
    [SerializeField] Vector2 _hiddenPlayerViewPos;
    // ----- OnSceneViews ----- //


    // ----- Camera ----- //
    Vector2 _cameraFocusPos = Vector2.zero;
    // ----- Camera ----- //

    // ----- ShowPlanetFleetsMeunu ----- //
    Coroutine _curShowPlanetFleetsMenuCo;
    readonly WaitForSeconds _showPlanetFleetsMenuWait = new WaitForSeconds(0.5f);
    // ----- ShowPlanetFleetsMeunu ----- //

    // ----- Alarm ----- //
    [Header("----- Alarm -----")]
    [SerializeField] PlayStartAlarm _playStartAlarm;
    [SerializeField] TurnStartAlarm _turnStartAlarm;
    const string ASSIGN_TURN_ALARM_TEXT = "Summon Turn";
    const string MOVE_TURN_ALARM_TEXT = "Battle Turn";
    public const float ALRAM_DURATION = 1.0F;
    readonly WaitForSeconds _alarmWait = new WaitForSeconds(ALRAM_DURATION);
    bool _isAlarming = false;
    // ----- Alarm ----- //

    private void Awake()
    {
        _uiModule = GetComponent<PlayScene_UIModule>();
        _battleModule = GetComponent<PlayScene_BattleModule>();
    }

    private void Start()
    {
        CreateWar();
        War.Start();
    }


    void CreateWar()
    {
        List<int> GetCardCodes(int deckCode)
        {
            List<DeckNodeInfo> deckNodeInfos = GameManager.Inst.InfoManager.GetDeckInfo(deckCode);
            List<int> rst = new List<int>();
            foreach (DeckNodeInfo deckNodeInfo in deckNodeInfos)
            {
                for (int i = 0; i < deckNodeInfo.CardNum; i++)
                    rst.Add(deckNodeInfo.CardCode);
            }
            return rst;
        }

        DuelSetting duelSetting = DuelSettingContainer.Inst.DuelSetting;

        MapInfo mapInfo = duelSetting.MapInfo;
        List<int>[] cardCodes = new List<int>[War.PLAYER_NUM];

        for(int i = 0; i < cardCodes.Length; i++)
            cardCodes[i] = GetCardCodes(duelSetting.PlayerSettings[i].DeckCode);
        
        War = new War(cardCodes, mapInfo);

        _warPresenter = new WarPresenter(War, _playerViews, _mapView, _graveView, _resultView, _surrenderView, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            GameManager.Inst.LanguageManager.ChangeLanguage(LanguageType.Korean);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            GameManager.Inst.LanguageManager.ChangeLanguage(LanguageType.English);


        if (_isAlarming == true)
            return;

        _warPresenter.OnUpdate();
    }

    public void SetPlayerViewPos(int playerIndex)
    {
        // playerIndex의 PlayerView는 화면 가운데
        // 아닌 것은 PlayerViewHiddenPos에 위치

        foreach (PlayerView playerView in _playerViews)
            playerView.transform.localPosition = _hiddenPlayerViewPos;

        _playerViews[playerIndex].transform.localPosition = Vector3.zero;
    }

    public void PlayAlarm(Player player, bool withPlayStart = false)
    {
        StartCoroutine(PlayAlarmCo(player, withPlayStart));
    }

    IEnumerator PlayAlarmCo(Player player, bool withPlayStart = false)
    {
        _isAlarming = true;
        if(withPlayStart == true)
        {
            _playStartAlarm.gameObject.SetActive(true);
            _playStartAlarm.SetSpeciesColor(player.SpeciesColor);
            _playStartAlarm.SetSpeciesMarkImage(player.SpeciesMarkSprite);
            
            yield return _alarmWait;
            _playStartAlarm.gameObject.SetActive(false);
        }
        _turnStartAlarm.gameObject.SetActive(true);
        _turnStartAlarm.SetSpeciesColor(player.SpeciesColor);
        _turnStartAlarm.SetSpeciesMarkImage(player.SpeciesMarkSprite);
        if (player.IsAssignTurn)
            _turnStartAlarm.SetTurnStartText(ASSIGN_TURN_ALARM_TEXT);
        else if (player.IsMoveTurn)
            _turnStartAlarm.SetTurnStartText(MOVE_TURN_ALARM_TEXT);
        
        yield return _alarmWait;
        _turnStartAlarm.gameObject.SetActive(false);
        _isAlarming = false;
    }

    public void ResetCamera()
    {
        _cameraMoveCtrl.ResetPos();
        _cameraZoomCtrl.ResetZoom();
    }

    public void SetCameraMoveLimit(Vector2 maxPos, Vector2 minPos)
    {
        _cameraMoveCtrl.SetLimit(maxPos, minPos);
    }
    public void FocusCamera(float posX, float posY)
    {
        _cameraFocusPos.x = posX;
        _cameraFocusPos.y = posY;

        _cameraMoveCtrl.Focus(_cameraFocusPos, true);
    }
    public void FreezeCamera(bool isFreeze)
    {
        _cameraMoveCtrl.Freeze(isFreeze);
    }

    public void ReserveShowPlanetFleetsMenuAction(UnityAction action)
    {
        CancelShowPlanetFleetsMenuAction();
        _curShowPlanetFleetsMenuCo = StartCoroutine(ShowPlanetFleetsMenuActionCo(action));
    }
    public void CancelShowPlanetFleetsMenuAction()
    {
        if (_curShowPlanetFleetsMenuCo != null)
            StopCoroutine(_curShowPlanetFleetsMenuCo);
    }
    IEnumerator ShowPlanetFleetsMenuActionCo(UnityAction action)
    {
        yield return _showPlanetFleetsMenuWait;
        action?.Invoke();
    }

    public void ExitPlayScene()
    {
        _warPresenter.Clear();
        GameManager.Inst.LoadScene("Menu");
    }
}
