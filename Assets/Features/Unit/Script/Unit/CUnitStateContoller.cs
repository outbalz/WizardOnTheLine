using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CUnitStateContoller : MonoBehaviour
{
	public enum EUnitState
	{
		Idle,
		Move,
		Aim, 
		Engage
    }

	#region inspector
	[SerializeField] private CUnitController _unitController;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private Animator _animator;
    [SerializeField] private Vector2 _projectileOffset;
	#endregion

	#region private var
    private EUnitState _state = EUnitState.Idle;
    private SOderContext _currentOder;
	//private bool _isInCover = false;
    //private bool _isWaitingAnimation = false;
    
    //private ITargetable _target = null;

    private CancellationTokenSource _cts;

    private float _weaponCooldown;

	private readonly int _isMoveingHash = Animator.StringToHash("bIsMoveing");
	private readonly int _isInCoverHash = Animator.StringToHash("bIsInCover");
	private readonly int _hasTagetHash = Animator.StringToHash("bHasTaget");
	private readonly int _fireWeaponhash = Animator.StringToHash("tFireWeapon");
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
        CancelTask();
    }

    private void OnDestroy()
    {
        CancelTask();
    }

    public void InitializeState(SOUnitData data)
    {
        ExceptionCheck();

        if (data.AnimatorOverrideController != null)
        {
            _animator.runtimeAnimatorController = data.AnimatorOverrideController;
        }

        else
        {

        }

        if (_state != EUnitState.Idle)
        {
            ChangeState(EUnitState.Idle);
        }

        else
        {
            OnStateEnter(EUnitState.Idle);
        }
    }

    private void ExceptionCheck()
    {
        if(_unitController == null)
        {
            Debug.LogWarning("_unitController is not assigned.");
            enabled = false;
            return;
        }

        if (_spriteRenderer == null)
        {
            Debug.LogWarning("_spriteRenderer is not assigned.");
            enabled = false; 
            return;
        }

        if (_animator == null)
        {
            Debug.LogWarning("_animator is not assigned.");
            enabled = false;
            return;
        }
    }

    public bool GiveOder(SOderContext context)
    {

        switch (context.oderType)
        {
            case SOderContext.EOderType.None:
                return false;
            case SOderContext.EOderType.Move:

                //float targtX = context.targetPos.x;
                //targtX += Random.Range(-_unitController.Squad.SquadRadiusX, _unitController.Squad.SquadRadiusX);

                context.targetPos.x += Random.Range(-_unitController.Squad.SquadRadiusX, _unitController.Squad.SquadRadiusX);

                _currentOder = context;

                ChangeState(EUnitState.Move);

                //if (!_isInCover)
                //{
                    CarryOutOder();
                //}

                return true;
            case SOderContext.EOderType.AttackMove:

                context.targetPos.x += Random.Range(-_unitController.Squad.SquadRadiusX, _unitController.Squad.SquadRadiusX);

                _currentOder = context;

                ChangeState(EUnitState.Move);
                
                CarryOutOder();
                break;
            case SOderContext.EOderType.Stop:
                _currentOder = context;
                CarryOutOder();
                break;
            case SOderContext.EOderType.Retreat:
                context.targetPos.x = CUtil.MAP_WITH_MIN;
                _currentOder = context;

                ChangeState(EUnitState.Move);
                CarryOutOder();
                break;
            default:
                break;
        }

        return false;
    }

    public void CancelTask()
    {
        if(_cts ==  null)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    private void ChangeState(EUnitState state)
    {
        if(_state == state)
        { 
            return; 
        }

        OnStateExit(state);
        _state = state;
        OnStateEnter(state);
    }

    private void OnStateEnter(EUnitState state)
    {
        switch (state)
        {
            case EUnitState.Idle:
                _animator.SetBool(_isInCoverHash, true);
                //_isInCover = true;

                FlipX(_unitController.Squad.PlayerData.IsFaceingLeft);

                if (_weaponCooldown <= 0)
                {
                    _unitController.TargetFinder.GiveTagetSreachOder(EngageTarget);
                }

                else
                {
                    CancelTask();
                    _cts = new CancellationTokenSource();
                    CoolDownWeapon(_cts.Token).Forget();
                }

                break;
            case EUnitState.Move:
                _animator.SetBool(_isInCoverHash, false);
                _animator.SetBool(_isMoveingHash, true);
                //_isInCover = false;

                if (_currentOder.oderType != SOderContext.EOderType.AttackMove)
                {
                    _unitController.TargetFinder.CancelSreach();
                }

                CarryOutOder();

                break;
            case EUnitState.Aim:
                _animator.SetBool(_hasTagetHash, true);
                break;
            case EUnitState.Engage:
                _animator.SetTrigger(_fireWeaponhash);
                break;
            default:
                break;
        }
    }

    private void OnStateExit(EUnitState state)
    {
        switch (_state)
        {
            case EUnitState.Idle:
                break;
            case EUnitState.Move:
                _animator.SetBool(_isMoveingHash, false);
                _animator.SetBool(_isInCoverHash, true);
                break;
            case EUnitState.Aim:
                if(state != EUnitState.Engage)
                {
                    _animator.SetBool(_hasTagetHash, false);
                    CancelTask();
                }
                break;
            case EUnitState.Engage:
                if(state != EUnitState.Aim)
                {
                    _animator.SetBool(_hasTagetHash, false);
                    CancelTask();
                }
                break;
            default:
                break;
        }
    }

    private void EngageTarget(ITargetable target)
    {
        if(target == null)
        {
            return;
        }

        if(_state == EUnitState.Move && _currentOder.oderType != SOderContext.EOderType.AttackMove)
        {
            return;
        }

        if(_currentOder.oderType == SOderContext.EOderType.AttackMove)
        {
            CancelTask();
            _cts = new CancellationTokenSource();
            TakeAStepColserToTarget(target.transform, _cts.Token).Forget();

            return;
        }

        //_target = target;
        ChangeState(EUnitState.Aim);
        CancelTask();
        _cts = new CancellationTokenSource();
        AimTarget(_cts.Token,target).Forget();
    }

    private async UniTask TakeAStepColserToTarget(Transform targetTr, CancellationToken ct)
    {
        await UniTask.Yield();

        if (ct.IsCancellationRequested)
        {
            CarryOutOder();
            return;
        }

        while (Mathf.Abs(targetTr.position.x - transform.position.x) > _unitController.Status.WeaponRange - 1)
        {
            if (ct.IsCancellationRequested)
            {
                CarryOutOder();
                return;
            }

            if(Mathf.Abs(targetTr.position.x - transform.position.x) > _unitController.Status.WeaponRange)
            {
                CarryOutOder();
                return;
            }

            await UniTask.Yield();
        }

        _unitController.Movement.CancelMoveOder();

        ChangeState(EUnitState.Aim);
        
        CancelTask();
        _cts = new CancellationTokenSource();
        ITargetable target = _unitController.TargetFinder.FindTarget();
        AimTarget(_cts.Token,target).Forget();

    }

    private async UniTask CoolDownWeapon(CancellationToken ct)
    {
        await UniTask.Yield();

        while (_weaponCooldown > 0)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            _weaponCooldown -= Time.deltaTime;
            await UniTask.Yield();
        }

        if(_state == EUnitState.Idle)
        {
            _unitController.TargetFinder.GiveTagetSreachOder(EngageTarget);
        }
    }

    private async UniTask AimTarget(CancellationToken ct, ITargetable target)
    {
        await UniTask.Yield();

        if (ct.IsCancellationRequested)
        {
            return;
        }

        if (target == null)
        {
            if (_currentOder.oderType != SOderContext.EOderType.Move && _currentOder.oderType != SOderContext.EOderType.AttackMove)
            {
                ChangeState(EUnitState.Idle);
            }

            else
            {
                ChangeState(EUnitState.Move);
            }

            return;
        }

        /*
        if (_weaponCooldown > 0)
        {
            await CoolDownWeapon(ct);
        }
        */

        float aimTime = _unitController.Status.WeaponAimTime;

        while (aimTime > 0)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }


            target = _unitController.TargetFinder.FindTarget();

            if (target == null)
            {
                if (_currentOder.oderType != SOderContext.EOderType.Move && _currentOder.oderType != SOderContext.EOderType.AttackMove)
                {
                    ChangeState(EUnitState.Idle);
                }

                else
                {
                    ChangeState(EUnitState.Move);
                }

                return;
            }

            Vector2 targetPos = target.transform.position;

            if(targetPos.x < transform.position.x)
            {
                FlipX(true);
            }

            else
            {
                FlipX(false);
            }

            if(_weaponCooldown > 0)
            {
                _weaponCooldown -= Time.deltaTime;
                await UniTask.Yield();
                continue;
            }

            aimTime -= Time.deltaTime;
            await UniTask.Yield();
        }

        EngageWeapon();
    }

    private void EngageWeapon()
    {
        ITargetable target = _unitController.TargetFinder.FindTarget();

        if(target == null)
        {
            if (_currentOder.oderType != SOderContext.EOderType.Move && _currentOder.oderType != SOderContext.EOderType.AttackMove)
            {
                ChangeState(EUnitState.Idle);
            }

            else
            {
                ChangeState(EUnitState.Move);
            }
            return;
        }

        Vector2 targetPos = target.transform.position;
        Vector2 originPos = transform.position;

        if (targetPos.x < transform.position.x)
        {
            FlipX(true);
            originPos.x -= _projectileOffset.x;
            originPos.y += _projectileOffset.y;
        }

        else
        {
            FlipX(false);
            originPos.x += _projectileOffset.x;
            originPos.y += _projectileOffset.y;
        }

        if(_unitController.Status.WeaponSound != null)
        {
            CSoundManager.PlaySoundRelay(_unitController.Status.WeaponSound, originPos);
        }

        float distance = Mathf.Abs(transform.position.x - targetPos.x);
        target.TakeDamage(_unitController.Status.GetWeaponDamage(distance));

        if(_unitController.Status.Projectile != null)
        {
            CProjectileController projectile = CProjectilePool.Instance.GetFromPool(_unitController.Status.Projectile);
            projectile.transform.position = transform.position;
            projectile.Initialized(originPos, targetPos);
            projectile.gameObject.SetActive(true);
        }
        
        ChangeState(EUnitState.Engage);
    }

    private void FireWeapon()
    {
        _weaponCooldown = _unitController.Status.WeaponCoolDown;

        ITargetable target = _unitController.TargetFinder.FindTarget();

        if(target != null && _currentOder.oderType != SOderContext.EOderType.Move)
        {
            ChangeState(EUnitState.Aim);
            CancelTask();
            _cts = new CancellationTokenSource();

            AimTarget(_cts.Token, target).Forget();
            return;
        }

        if (_currentOder.oderType != SOderContext.EOderType.Move && _currentOder.oderType != SOderContext.EOderType.AttackMove)
        {
            ChangeState(EUnitState.Idle);
        }

        else
        {
            ChangeState(EUnitState.Move);
        }

        CancelTask();
        _cts = new CancellationTokenSource();

        CoolDownWeapon(_cts.Token).Forget();
    }

    private void CarryOutOder()
    {

        switch (_currentOder.oderType)
        {
            case SOderContext.EOderType.None:
                return;
            case SOderContext.EOderType.Move:
                _unitController.Movement.GiveMoveOder(_currentOder.targetPos.x);
                break;
            case SOderContext.EOderType.AttackMove:
                _unitController.Movement.GiveMoveOder(_currentOder.targetPos.x);

                _unitController.TargetFinder.GiveTagetSreachOder(EngageTarget);
                break;
            case SOderContext.EOderType.Stop:
                _unitController.Movement.CancelMoveOder();
                OderComplet();
                break;
            case SOderContext.EOderType.Retreat:
                _unitController.Movement.GiveMoveOder(_currentOder.targetPos.x);
                break;
            default:
                break;
        }
    }

    public void OderComplet()
    {
        if(_currentOder.oderType == SOderContext.EOderType.Retreat)
        {
            float hpMultplier = _unitController.HP.HP/_unitController.Status.MaxHP;


            float refundCost = (_unitController.Squad.SquadReferance.Cost / _unitController.Squad.SquadReferance.SquadSize) * hpMultplier;

            CGameManager.Instance.PlayerCMP += refundCost;

            //Debug.Log($"{_unitController.Squad.SquadReferance.Cost} / {_unitController.Squad.SquadReferance.SquadSize} * {hpMultplier} = {refundCost}");
            //Debug.Log(_unitController.Squad.SquadReferance.SquadSize);

            _unitController.KillUnit();
            return;
        }

        _currentOder.oderType = SOderContext.EOderType.None;
        ChangeState(EUnitState.Idle);
    }

    public void FlipX(bool flipX)
    {
        _spriteRenderer.flipX = flipX;
    }
}
