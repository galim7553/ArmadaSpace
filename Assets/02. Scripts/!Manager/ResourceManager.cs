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
    /// 리소스를 로드합니다. 캐시된 리소스가 있으면 캐시에서 반환합니다.
    /// </summary>
    /// <typeparam name="T">로드할 리소스의 타입</typeparam>
    /// <param name="path">Resources 폴더 내 리소스의 경로</param>
    /// <returns>로드된 리소스</returns>
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
    /// 특정 리소스를 언로드합니다.
    /// </summary>
    /// <param name="path">Resources 폴더 내 리소스의 경로</param>
    public void UnloadResource(string path)
    {
        if (_resourceCache.ContainsKey(path))
        {
            Resources.UnloadAsset(_resourceCache[path]);
            _resourceCache.Remove(path);
        }
    }

    /// <summary>
    /// 모든 캐시된 리소스를 언로드합니다.
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