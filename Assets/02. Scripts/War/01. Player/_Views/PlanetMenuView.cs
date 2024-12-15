using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlanetMenuView : RootBase
{
    enum TMPs
    {
        PlanetNameText,
        CardCountText,
        OrbitToggleLabel,
        GroundToggleLabel,
        FactoryToggleLabel,
        FleetMenuButtonLabel
    }
    enum Images
    {
        OwnerMark,
        OrbitCheckmark,
        GroundCheckmark,
        FactoryCheckmark,
        SlotBackground
    }
    enum Toggles
    {
        OrbitToggle,
        GroundToggle,
        FactoryToggle
    }

    enum GameObjects
    {
        PlanetCardBox
    }
    enum CardViews
    {
        PlanetCardView,
    }
    enum Transforms
    {
        CardCounterViewContent
    }
    enum Buttons
    {
        FleetMenuButton
    }

    public event UnityAction<CardType> onCardTypeSelected;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
        Bind<Toggle>(typeof(Toggles));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));
        Bind<CardView>(typeof(CardViews));

        SubscribeToggles();
    }

    public void AddFleetMenuButtonClickedAction(UnityAction action)
    {
        GetButton((int)Buttons.FleetMenuButton).onClick.AddListener(action);
    }

    void SubscribeToggles()
    {
        Get<Toggle>((int)Toggles.OrbitToggle).onValueChanged.AddListener(OnOrbitToggleChanged);
        Get<Toggle>((int)Toggles.GroundToggle).onValueChanged.AddListener(OnGroundToggleChanged);
        Get<Toggle>((int)Toggles.FactoryToggle).onValueChanged.AddListener(OnFactoryToggleChanged);
    }
    public void ResetToggles()
    {
        Get<Toggle>((int)Toggles.OrbitToggle).SetIsOnWithoutNotify(true);
    }

    public void SetPlanetNameText(string planetName)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlanetNameText).text = planetName;
    }
    public void SetCardCountText(string damageSum)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardCountText).text = damageSum;
    }
    public void SetCardCountText(int curCount, int maxCount)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardCountText).text = $"{curCount} / {maxCount}";
    }
    public void SetOwnerMarkImage(Sprite sprite)
    {
        GetImage((int)Images.OwnerMark).sprite = sprite;
    }
    public void ShowOwnerMarkImage(bool value)
    {
        GetImage((int)Images.OwnerMark).gameObject.SetActive(value);
    }

    public void SetSlotBackgroundImageActive(bool isActive)
    {
        GetImage((int)Images.SlotBackground).gameObject.SetActive(isActive);
    }
    public void SetSlotBackgroundImage(Sprite sprite)
    {
        GetImage((int)Images.SlotBackground).sprite = sprite;
    }

    public CardView GetPlanetCardView()
    {
        return Get<CardView>((int)CardViews.PlanetCardView);
    }
    public void ShowPlanetCardView(bool value)
    {
        Get<CardView>((int)CardViews.PlanetCardView).gameObject.SetActive(value);
    }
    public void SetPlanetCardBoxActive(bool isActive)
    {
        Get<GameObject>((int)GameObjects.PlanetCardBox).SetActive(isActive);
    }

    public void AddCardCounterView(CardCounterView cardCounterView)
    {
        Transform content = Get<Transform>((int)Transforms.CardCounterViewContent);
        cardCounterView.transform.SetParent(content, false);
        cardCounterView.transform.SetAsLastSibling();
    }

    public void SetOwnerMarkSpeciesColor(Color color)
    {
        GetImage((int)Images.OwnerMark).color = color;
    }
    public void SetToggleSpeciesColor(Color color)
    {
        GetImage((int)Images.OrbitCheckmark).color = color;
        GetImage((int)Images.GroundCheckmark).color = color;
        GetImage((int)Images.FactoryCheckmark).color = color;
    }

    public void SetToggleLabels(string orbitLabel, string groundLabel, string factoryLabel)
    {
        Get<TextMeshProUGUI>((int)TMPs.OrbitToggleLabel).text = orbitLabel;
        Get<TextMeshProUGUI>((int)TMPs.GroundToggleLabel).text = groundLabel;
        Get<TextMeshProUGUI>((int)TMPs.FactoryToggleLabel).text = factoryLabel;
    }
    public void SetToggleOn(int toggleIndex)
    {
        Get<Toggle>(toggleIndex).SetIsOnWithoutNotify(true);
    }

    public void SetFleetMenuButtonActive(bool isActive)
    {
        GetButton((int)Buttons.FleetMenuButton).gameObject.SetActive(isActive);
    }
    public void SetFleetMenuButtonLabel(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.FleetMenuButtonLabel).text = str;
    }

    void OnOrbitToggleChanged(bool value)
    {
        if (value == false)
            return;

        onCardTypeSelected?.Invoke(CardType.Battleship_Card);
    }
    void OnGroundToggleChanged(bool value)
    {
        if (value == false)
            return;

        onCardTypeSelected?.Invoke(CardType.Soldier_Card);
    }
    void OnFactoryToggleChanged(bool value)
    {
        if (value == false)
            return;

        onCardTypeSelected?.Invoke(CardType.Factory_Card);
    }

    public void Clear()
    {
        onCardTypeSelected = null;
        GetButton((int)Buttons.FleetMenuButton).onClick.RemoveAllListeners();
    }
}
