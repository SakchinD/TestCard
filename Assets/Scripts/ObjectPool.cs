using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public struct KeyValue
    {
        public string ObjectName;
        public CardView Object;
    }
    public List<KeyValue> objects;
    Dictionary<string, List<CardView>> poolDict = new Dictionary<string, List<CardView>>();

    DiContainer _diContainer;

    [Inject]
    void Construct(DiContainer container)
    {
        _diContainer = container;
    }

    void CreateObject(string ObjectName)
    {
        if (!poolDict.ContainsKey(ObjectName))
        {
            poolDict.Add(ObjectName, new List<CardView>());
        }
        var obj = objects.Find(item => item.ObjectName == ObjectName);
        var item = _diContainer.InstantiatePrefabForComponent<CardView>(obj.Object);
        item.transform.SetParent(transform);
        item.gameObject.SetActive(false);
        poolDict[ObjectName].Add(item);
    }
    public CardView GetPooledObject(string ObjectName)
    {
        if (poolDict.ContainsKey(ObjectName))
        {
            for (int i = 0; i < poolDict[ObjectName].Count; i++)
            {
                if (!poolDict[ObjectName][i].gameObject.activeInHierarchy)
                {
                    return poolDict[ObjectName][i];
                }
            }
        }
        CreateObject(ObjectName);
        return poolDict[ObjectName][poolDict[ObjectName].Count - 1];
    }
}
