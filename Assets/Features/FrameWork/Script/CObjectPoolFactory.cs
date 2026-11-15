using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObjectPoolFactory<TFactoy, TPool, TReuslt> : CFactory<TFactoy, TReuslt> 
    where TFactoy : MonoBehaviour where TReuslt : MonoBehaviour where TPool : MonoBehaviour
{
    #region inspector
    [SerializeField] protected CObjectPool<TPool, TReuslt> _pool;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        PoolCheck();
    }

    protected virtual void PoolCheck() 
    {
        if(_pool == null)
        {
            Debug.LogWarning("Object pool is not assigned in the inspector.");
            enabled = false;
            return;
        }

        else
        {
            _pool.Prewarm();
        }
    }


    public override TReuslt Creat()
    {
        return _pool.GetFromPool();
    }

    public override TReuslt Creat(Transform tr)
    {
        TReuslt obj = _pool.GetFromPool();

        if (obj == null)
        {
            return null;
        }

        obj.transform.SetParent(tr);

        return obj;
    }
}
