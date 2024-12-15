using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    Dictionary<string, Pool> _poolDictionary;

    public PoolManager()
    {
        _poolDictionary = new Dictionary<string, Pool>();
    }

    public void CreatePool(string prefabName, int size = 1)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if(prefab == null)
        {
            Debug.LogError($"Resource at path(Prefabs/{prefabName}) could not be found.");
            return;
        }
        CreatePool(prefabName, prefab, size);
    }

    void CreatePool(string prefabName, GameObject prefab, int size = 1)
    {
        if (!_poolDictionary.ContainsKey(prefabName))
        {
            Transform parent = new GameObject($"@Pool_{prefabName}").transform;
            Object.DontDestroyOnLoad(parent);
            Pool pool = new Pool(prefab, parent, size);
            _poolDictionary.Add(prefabName, pool);
        }
    }

    public GameObject GetFromPool(string prefabName)
    {
        // Create되지 않은 Pool이면 생성 시도
        if (_poolDictionary.ContainsKey(prefabName) == false)
            CreatePool(prefabName);

        // Create 시도했음에도 존재하지 않는 Pool이면 null return
        if (_poolDictionary.ContainsKey(prefabName) == false)
            return null;

        return _poolDictionary[prefabName].Get();
    }
}

public class Pool
{
    private Stack<GameObject> _stackPool;
    private GameObject _prefab;
    private Transform _parent;

    public Pool(GameObject prefab, Transform parent, int initialSize)
    {
        _prefab = prefab;
        _parent = parent;
        _stackPool = new Stack<GameObject>();

        for (int i = 0; i < initialSize; i++)
            CreatePoolObj();
    }

    void CreatePoolObj()
    {
        GameObject obj = Object.Instantiate(_prefab);
        obj.transform.SetParent(_parent, false);
        obj.SetActive(false);
        obj.AddComponent<PoolObject>().SetPool(this);
        _stackPool.Push(obj);
    }

    public GameObject Get()
    {
        if (_stackPool.Count > 0)
        {
            GameObject obj = _stackPool.Pop();
            if (obj == null)
                return Get();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Object.Instantiate(_prefab);
            obj.AddComponent<PoolObject>().SetPool(this);
            return obj;
        }
    }

    public void Return(GameObject obj)
    {
        obj.transform.SetParent(_parent, false);
        obj.SetActive(false);
        _stackPool.Push(obj);
    }
}