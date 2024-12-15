using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenterBase
{
    public static ResourceManager s_ResourceManager => GameManager.Inst.ResourceManager;
    public static LanguageManager s_LanguageManager => GameManager.Inst.LanguageManager;
}

public abstract class LanguageChangablePresenter : PresenterBase
{
    public LanguageChangablePresenter()
    {
        SubscribeLanguageChangedEvent();
    }
    protected abstract void UpdateLanguageTexts();
    void OnLanguageChanged()
    {
        UpdateLanguageTexts();
    }
    void SubscribeLanguageChangedEvent()
    {
        s_LanguageManager.onLanguageChanged += OnLanguageChanged;
    }
    public virtual void Clear()
    {
        s_LanguageManager.onLanguageChanged -= OnLanguageChanged;
    }
}

public class RootPresenterBase
{
    public static PoolManager s_PoolManager => GameManager.Inst.PoolManager;
}
