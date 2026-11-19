using System.Collections.Generic;
using UnityEngine;

public interface IPoolabeSO<T> where T : MonoBehaviour
{
    public T Prefab { get;}
}

public class CMultiObjectPool<TPool, TReuslt, TSO> : CSingleton<TPool> where TPool : MonoBehaviour where TReuslt : MonoBehaviour where TSO : IPoolabeSO<TReuslt>
{
    #region inspector
    [SerializeField] protected TSO[] _so;
    [SerializeField] protected int _prewarmCount;
    [SerializeField] protected Transform _poolRoot;
    #endregion

    #region private var
    protected readonly Dictionary<TSO, Queue<TReuslt>> _pool = new Dictionary<TSO, Queue<TReuslt>>();
    protected readonly Dictionary<TSO, List<TReuslt>> _liveList = new Dictionary<TSO, List<TReuslt>>();
    #endregion

    protected override void Awake()
    {
        base.Awake();

        PrefabCheck();
        Prewarm();
    }

    protected virtual void PrefabCheck()
    {
        if (_so.Length == 0)
        {
            Debug.LogWarning("Object prefab is not assigned in the inspector.");
            enabled = false;
            return;
        }
    }

    public void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("_Pool_Root_" + (typeof(TReuslt).ToString()));

        _poolRoot = root.transform;
    }

    public void Prewarm()
    {
        CreatePoolRoot();

        for (int i = 0; i < _so.Length; i++)
        {
            _pool.Add(_so[i], new Queue<TReuslt>());
            _liveList.Add(_so[i], new List<TReuslt>());

            for (int j = 0; j < _prewarmCount; j++)
            {
                TReuslt obj = Instantiate(_so[i].Prefab, _poolRoot);
                obj.gameObject.SetActive(false);
                _pool[_so[i]].Enqueue(obj);
            }
        }
    }

    public TReuslt GetFromPool(TSO so)
    {
        TReuslt obj;

        if (_pool[so].Count > 0)
        {
            obj = _pool[so].Dequeue();
        }

        else
        {
            obj = Instantiate(so.Prefab);
            obj.gameObject.SetActive(false);
        }

        _liveList[so].Add(obj);
        //obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(TReuslt obj, TSO so)
    {
        if (obj == null)
        {
            return;
        }

        if (_liveList[so].Contains(obj))
        {
            _liveList[so].Remove(obj);
        }

        obj.transform.SetParent(_poolRoot);
        obj.gameObject.SetActive(false);
        _pool[so].Enqueue(obj);
    }

    public List<TReuslt> GetAliveList(TSO so)
    {
        return _liveList[so];
    }
}
