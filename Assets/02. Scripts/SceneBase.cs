using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SceneBase : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
            Instantiate(Resources.Load<GameObject>("UI/EventSystem")).name = "@EventSystem";
    }
    public abstract void Clear();
}
