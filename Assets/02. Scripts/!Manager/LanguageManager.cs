using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum LanguageType
{
    Korean,
    English
}

public interface ILanguageChangedListener
{
    void Subscribe();
    void OnLanguageChanged();
}

public class LanguageManager
{
    CodeStringContainer _codeStringContainer;
    Dictionary<string, string> _langDic = new Dictionary<string, string>();

    LanguageType _curLanguageType;
    public event UnityAction onLanguageChanged;
    public LanguageType CurrentLanguageType => _curLanguageType;

    public LanguageManager()
    {
        _codeStringContainer = JsonUtility.FromJson<CodeStringContainer>(Resources.Load<TextAsset>("lang").text);
        if (_codeStringContainer == null)
        {
            Debug.LogError($"Failed to load CodeStringContainer. path: ({"lang"})");
            return;
        }

        _curLanguageType = LanguageType.Korean;
        LoadCodeStrings(_curLanguageType);
    }
    void LoadCodeStrings(LanguageType languageType)
    {
        _langDic.Clear();
        switch (languageType)
        {
            case LanguageType.Korean:
                foreach (CodeString codeString in _codeStringContainer.CodeStrings)
                    _langDic[codeString.Code] = codeString.Kor;
                break;
            default:
                foreach (CodeString codeString in _codeStringContainer.CodeStrings)
                    _langDic[codeString.Code] = codeString.Eng;
                break;
        }
    }
    public void ChangeLanguage(LanguageType languageType)
    {
        if (_curLanguageType == languageType)
            return;
        _curLanguageType = languageType;
        LoadCodeStrings(languageType);
        onLanguageChanged?.Invoke();
    }

    public string GetString(string code)
    {
        string str = string.Empty;
        if(_langDic.TryGetValue(code, out str) == false)
        {
            str = string.Empty;
            Debug.LogWarning($"There is no string for code : ({code})");
        }
        return str;
    }
}

[System.Serializable]
public class CodeStringContainer
{
    [SerializeField] List<CodeString> codeStrings;
    public IReadOnlyList<CodeString> CodeStrings => codeStrings.AsReadOnly();
}

[System.Serializable]
public class CodeString
{
    [SerializeField] string code;
    [SerializeField] string kor;
    [SerializeField] string eng;

    public string Code => code;
    public string Kor => kor;
    public string Eng => eng;
}