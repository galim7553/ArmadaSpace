using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFactory<T> where T : MonoBehaviour
{
    PoolManager _poolManager;
    string _prefabName;
    public ViewFactory(PoolManager poolManager, string prefabName)
    {
        _poolManager = poolManager;
        _prefabName = prefabName;
    }

    public T GetView()
    {
        T view = _poolManager.GetFromPool(_prefabName).GetOrAddComponent<T>();
        return view;
    }
}
