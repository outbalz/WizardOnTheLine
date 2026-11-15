using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CFactory<TFactoy, TReuslt> : CSingleton<TFactoy> where TFactoy : MonoBehaviour where TReuslt : MonoBehaviour
{
	#region inspector
	[SerializeField] protected TReuslt _prefab;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		
		PrefabCheck();
	}

	protected virtual void PrefabCheck()
	{
        if (_prefab == null)
        {
            Debug.LogWarning("factory prefab is not assigned in the inspector.");
            enabled = false;
            return;
        }
    }

	public virtual TReuslt Creat()
	{
		return Instantiate(_prefab);
	}

	public virtual TReuslt Creat(Transform tr)
	{
        return Instantiate(_prefab, tr);
    }
}
