using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardToolTipHandler : MonoBehaviour
{
    // ----- Reference ----- //
    CardOnToolTipView _cardOnToolTipView;
    WaitForSeconds _waitForSeconds;
    // ----- Reference ----- //

    CardOnToolTipPresenter _cardOnToolTipPresenter;

    Card _curToolTipCard;


    Coroutine _curShowCardOnToolTipView;

    bool _isSleepMode = false;

    public void Init(CardOnToolTipView cardOnToolTipView, Color speciesColor, float toolTipWatingTime = 0.5f)
    {
        _cardOnToolTipView = cardOnToolTipView;
        _cardOnToolTipView.Init();
        SetSpeciesColor(speciesColor);
        _waitForSeconds = new WaitForSeconds(toolTipWatingTime);

        // ----- Start ----- //
        ForceShowToolTip(null);
        // ----- Start ----- //
    }

    // ----- SpeciesColor ----- //
    void SetSpeciesColor(Color color)
    {
        if(_cardOnToolTipView != null)
            _cardOnToolTipView.SetTextBoxSpeciesColor(color);
    }
    // ----- SpeciesColor ----- //

    // ----- ToolTip Show/Hide ----- //
    public void ForceShowToolTip(Card card)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (_curShowCardOnToolTipView != null)
        {
            StopCoroutine(_curShowCardOnToolTipView);
            _curShowCardOnToolTipView = null;
        }

        _curToolTipCard = card;
        ShowCardOnToolTipView();
    }
    public void ShowToolTip(Card card)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (_isSleepMode == true)
            return;

        _curToolTipCard = card;

        if (_curShowCardOnToolTipView != null)
        {
            StopCoroutine(_curShowCardOnToolTipView);
            _curShowCardOnToolTipView = null;
        }

        _curShowCardOnToolTipView = StartCoroutine(CoShowCardOnToolTipView());
    }
    public void SetSleepMode(bool isSleepMode)
    {
        _isSleepMode = isSleepMode;
        ForceShowToolTip(null);
    }
    IEnumerator CoShowCardOnToolTipView()
    {
        if (_cardOnToolTipView == null)
            yield break;

        // 현재 툴팁으로 보여야 하는 카드가 null이면 보여 주지 않는다.
        if (_curToolTipCard == null)
        {
            _cardOnToolTipView.gameObject.SetActive(false);
            yield break;
        }

        yield return _waitForSeconds;
        ShowCardOnToolTipView();
    }
    void ShowCardOnToolTipView()
    {
        // 현재 툴팁으로 보여야 하는 카드가 null이면 보여 주지 않는다.
        if (_curToolTipCard == null)
        {
            _cardOnToolTipView.gameObject.SetActive(false);
            return;
        }

        _cardOnToolTipView.gameObject.SetActive(true);
        if (_cardOnToolTipPresenter == null)
            _cardOnToolTipPresenter = new CardOnToolTipPresenter(_curToolTipCard, _cardOnToolTipView);
        else
            _cardOnToolTipPresenter.ChangeModel(_curToolTipCard);
    }
    // ----- ToolTip Show/Hide ----- //

    public void Clear()
    {
        if(_cardOnToolTipPresenter != null)
            _cardOnToolTipPresenter.Clear();
    }
}
