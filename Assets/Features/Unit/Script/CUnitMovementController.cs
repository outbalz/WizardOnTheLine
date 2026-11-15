using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CUnitMovementController : MonoBehaviour
{
	#region inspector
	[SerializeField] private CUnitController _unitController;
	#endregion

	#region private var
	private CUnitStatusHandler _stat;
    private CancellationTokenSource _cts;
    #endregion

    private void Awake()
    {
        if(_unitController == null)
        {
            if(!TryGetComponent(out _unitController))
            {
                Debug.LogError($"{this}/ UnitController is not assigned and not found on the GameObject {gameObject.name}");
                enabled = false;
                return;
            }
        }

        _stat = _unitController.Status;
        
        if(_stat == null)
        {
            Debug.LogError($"{this}/ StatusHandler is not assigned on the UnitController {gameObject.name}");
            enabled = false;
            return;
        }
    }

    private void OnDisable()
    {
        CancelMoveOder();
    }

    private void OnDestroy()
    {
        CancelMoveOder();
    }

    private void MoveTo(Vector3 targetPosition)
    {
        float speed = _stat.MovementSpeed;

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= (speed * speed)* Time.deltaTime)
        {
            transform.position = targetPosition;
            return;
        }

        transform.position = transform.position + direction.normalized * speed * Time.deltaTime;
    }

    private async UniTask MoveToAsync(Vector3 targetPosition, CancellationToken ct)
    {
        while ((transform.position - targetPosition).sqrMagnitude > 0.1f)
        {
            if(ct.IsCancellationRequested)
            {
                break;
            }

            MoveTo(targetPosition);
            await UniTask.Yield();
        }
    }

    private void MoveTo(float targtX)
    {
        float speed = _stat.MovementSpeed;

        float distance = targtX - transform.position.x;
        float direction = (distance > 0) ? 1 : -1;

        if (Mathf.Abs(distance) <= speed * Time.deltaTime)
        {
            transform.position = new Vector3(targtX, transform.position.y, transform.position.z);
            return;
        }

        transform.position = new Vector3(transform.position.x + direction * speed * Time.deltaTime, transform.position.y, transform.position.z);

    }

    private async UniTask MoveToAsync(float targtX, CancellationToken ct)
    {
        float distance = targtX - transform.position.x;
        while (Mathf.Abs(distance) > 0.01f)
        {
            if(ct.IsCancellationRequested)
            {
                break;
            }

            MoveTo(targtX);
            await UniTask.Yield();
        }
    }

    public void GiveMoveOder(Vector3 target)
    {
        CancelMoveOder();
        _cts = new CancellationTokenSource();

        MoveToAsync(target, _cts.Token).Forget();
    }

    public void GiveMoveOder(float targetX)
    {
        CancelMoveOder();
        _cts = new CancellationTokenSource();

        MoveToAsync(targetX, _cts.Token).Forget();
    }

    public void CancelMoveOder()
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
