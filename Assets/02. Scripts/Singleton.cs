using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 클래스.
/// </summary>
/// <typeparam name="T">싱글톤 클래스로 사용할 타입</typeparam>
public class GSingleton<T> : MonoBehaviour where T : GSingleton<T>
{
    static T _inst;
    static readonly object _lock = new object();

    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                lock (_lock)
                {
                    if (_inst == null)
                    {
                        _inst = FindObjectOfType<T>();

                        if (_inst == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
                            _inst = go.AddComponent<T>();

                            DontDestroyOnLoad(go);
                        }
                    }
                }
            }
            return _inst;
        }
    }

    protected virtual void Awake()
    {
        if (_inst == null)
        {
            _inst = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}

public class ASingleton<T> : MonoBehaviour where T : ASingleton<T>
{
    static T _inst;
    static readonly object _lock = new object();

    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                lock (_lock)
                {
                    if (_inst == null)
                    {
                        _inst = FindObjectOfType<T>();

                        if (_inst == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
                            _inst = go.AddComponent<T>();
                        }
                    }
                }
            }
            return _inst;
        }
    }

    protected virtual void Awake()
    {
        if (_inst == null)
        {
            _inst = this as T;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if(_inst == this)
            _inst = null;
    }
}