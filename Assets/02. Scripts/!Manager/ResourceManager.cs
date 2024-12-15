using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourceManager
{
    private Dictionary<string, Object> _resourceCache;

    public ResourceManager()
    {
        _resourceCache = new Dictionary<string, Object>();
    }

    /// <summary>
    /// ���ҽ��� �ε��մϴ�. ĳ�õ� ���ҽ��� ������ ĳ�ÿ��� ��ȯ�մϴ�.
    /// </summary>
    /// <typeparam name="T">�ε��� ���ҽ��� Ÿ��</typeparam>
    /// <param name="path">Resources ���� �� ���ҽ��� ���</param>
    /// <returns>�ε�� ���ҽ�</returns>
    public T LoadResource<T>(string path) where T : Object
    {
        if (_resourceCache.ContainsKey(path))
        {
            return _resourceCache[path] as T;
        }

        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogWarning($"Resource at path({path}) could not be found.");
        }
        else
        {
            _resourceCache.Add(path, resource);
        }
        return resource;
    }

    /// <summary>
    /// Ư�� ���ҽ��� ��ε��մϴ�.
    /// </summary>
    /// <param name="path">Resources ���� �� ���ҽ��� ���</param>
    public void UnloadResource(string path)
    {
        if (_resourceCache.ContainsKey(path))
        {
            Resources.UnloadAsset(_resourceCache[path]);
            _resourceCache.Remove(path);
        }
    }

    /// <summary>
    /// ��� ĳ�õ� ���ҽ��� ��ε��մϴ�.
    /// </summary>
    public void UnloadAllResources()
    {
        foreach (var resource in _resourceCache.Values)
        {
            Resources.UnloadAsset(resource);
        }
        _resourceCache.Clear();
    }
}