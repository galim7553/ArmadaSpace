using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GSingleton<GameManager>
{

    public ResourceManager ResourceManager { get; private set; } 
    public InfoManager InfoManager { get; private set; }
    public LanguageManager LanguageManager { get; private set; }
    public PoolManager PoolManager { get; private set; }
    public SoundManager SoundManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        InitializeManagers();
    }

    private void InitializeManagers()
    {
        ResourceManager = new ResourceManager();
        InfoManager = new InfoManager();
        LanguageManager = new LanguageManager();
        PoolManager = new PoolManager();
        SoundManager = new SoundManager();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Clear()
    {
        SoundManager.Clear();
    }

}
