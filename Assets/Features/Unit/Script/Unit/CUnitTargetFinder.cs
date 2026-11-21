using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public interface ITargetable
{
    public void TakeDamage(float damage);
    public Transform transform { get;}
}

public class CUnitTargetFinder : MonoBehaviour
{
    #region inspector
    [SerializeField] private CUnitController _unitController;
    #endregion

    #region private var
    private CancellationTokenSource _cts;
    #endregion

    #region property
    private float WeaponRange => _unitController.Status.WeaponRange;
    #endregion

    private void Awake()
    {
        ExceptionCheck();
    }

    private void OnEnable()
    {
        ExceptionCheck();
    }

    private void OnDisable()
    {
        CancelSreach();
    }

    private void OnDestroy()
    {
        CancelSreach();
    }

    private void ExceptionCheck()
	{
        if (_unitController == null)
        {
            if (!TryGetComponent(out _unitController))
            {
                Debug.LogError($"{this}/ UnitController is not assigned and not found on the GameObject {gameObject.name}");
                enabled = false;
                return;
            }
        }
    }

    public ITargetable FindTarget()
    {
        Vector3 pointA = new Vector3(transform.position.x - WeaponRange, CUtil.MAP_HIGHT_MIN, transform.position.z);
        Vector3 pointB = new Vector3(transform.position.x + WeaponRange, CUtil.MAP_HIGHT_MAX, transform.position.z);

        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, _unitController.Squad.PlayerData.TargetLayer);

        if (colliders.Length <= 0)
        {
            return null;
        }

        float closestDistance = float.MaxValue;
        ITargetable closestTarget = null;

        for (int i = 0; colliders.Length > i; i++)
        {

            if (Mathf.Abs(colliders[i].transform.position.x - transform.position.x) > WeaponRange)
            {
                continue;
            }

            float distance = (colliders[i].transform.position - transform.position).sqrMagnitude;

            if (distance>= closestDistance)
            {
                continue;
            }

            ITargetable target = colliders[i].GetComponent<ITargetable>();

            if(target == null)
            {
                continue;
            }

            closestDistance = distance;
            closestTarget = target;
        }

        return closestTarget;
    }

    public void GiveTagetSreachOder(Action<ITargetable> targetHadleAction)
    {
        CancelSreach();

        _cts = new CancellationTokenSource();
        SreachTarget(_cts.Token, targetHadleAction).Forget();
    }

    private async UniTask SreachTarget(CancellationToken ct, Action<ITargetable> targetHadleAction)
    {
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

        while (!ct.IsCancellationRequested)
        {
            ITargetable target = FindTarget();

            if(target != null)
            {
                targetHadleAction?.Invoke(target);
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
    }

    public void CancelSreach()
    {
        if(_cts == null)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
