using System.Collections.Generic;
using UnityEngine;

public class CMultiObjectPool<TPool, TReuslt> : CSingleton<TPool> where TPool : MonoBehaviour where TReuslt : MonoBehaviour
{
    #region inspector
    [SerializeField] protected TReuslt[] _prefab;
    [SerializeField] protected int _prewarmCount;
    [SerializeField] protected Transform _poolRoot;
    #endregion

    #region private var
    protected readonly Dictionary<TReuslt, Queue<TReuslt>> _pool = new Dictionary<TReuslt, Queue<TReuslt>>();
    protected readonly Dictionary<TReuslt, List<TReuslt>> _liveList = new Dictionary<TReuslt, List<TReuslt>>();
    #endregion

    protected override void Awake()
    {
        base.Awake();

        PrefabCheck();
    }

    protected virtual void PrefabCheck()
    {
        if (_prefab.Length == 0)
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

        for (int i = 0; i < _prefab.Length; i++)
        {
            _pool.Add(_prefab[i], new Queue<TReuslt>());
            _liveList.Add(_prefab[i], new List<TReuslt>());

            for (int j = 0; j < _prewarmCount; j++)
            {
                TReuslt obj = Instantiate(_prefab[i], _poolRoot);
                obj.gameObject.SetActive(false);
                _pool[_prefab[i]].Enqueue(obj);
            }
        }
    }

    public TReuslt GetFromPool(TReuslt prefab)
    {
        TReuslt obj;

        if (_pool.Count > 0)
        {
            obj = _pool[prefab].Dequeue();
        }

        else
        {
            obj = Instantiate(prefab);
        }

        _liveList[prefab].Add(obj);
        //obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(TReuslt obj, TReuslt prefab)
    {
        if (obj == null)
        {
            return;
        }

        if (_liveList[prefab].Contains(obj))
        {
            _liveList.Remove(obj);
        }

        obj.transform.SetParent(_poolRoot);
        obj.gameObject.SetActive(false);
        _pool[prefab].Enqueue(obj);
    }

    public List<TReuslt> GetAliveList(TReuslt prefab)
    {
        return _liveList[prefab];
    }
}
