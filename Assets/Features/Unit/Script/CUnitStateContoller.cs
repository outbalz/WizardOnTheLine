using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.Animations;
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
	#endregion

	#region private var
    private EUnitState _state = EUnitState.Idle;
    private SOderContext _currentOder;
	private bool _isInCover = false;
    //private bool _isWaitingAnimation = false;
    //private CancellationTokenSource _cts;

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
        ChangeState(EUnitState.Idle);
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
        //CancelOder();

        switch (context.oderType)
        {
            case SOderContext.EOderType.None:
                return false;
            case SOderContext.EOderType.Move:

                //float targtX = context.targetPos.x;
                //targtX += Random.Range(-_unitController.Squad.SquadRadiusX, _unitController.Squad.SquadRadiusX);

                context.targetPos.x += Random.Range(-_unitController.Squad.SquadRadiusX, _unitController.Squad.SquadRadiusX);

                _currentOder = context;

                if (!_isInCover)
                {
                    CarryOutOder();
                }
                ChangeState(EUnitState.Move);

                return true;
            case SOderContext.EOderType.AttackMove:
                break;
            case SOderContext.EOderType.Stop:
                break;
            case SOderContext.EOderType.Retreat:
                break;
            default:
                break;
        }

        return false;
    }

    /*
    public void CancelOder()
    {
        if(_cts ==  null)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
    */

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
                _isInCover = true;

                FlipX(_unitController.Squad.PlayerData.IsFaceingLeft);
                break;
            case EUnitState.Move:
                _animator.SetBool(_isInCoverHash, false);
                _animator.SetBool(_isMoveingHash, true);
                _isInCover = false;
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
                break;
            case EUnitState.Aim:
                if(state != EUnitState.Engage)
                {
                    _animator.SetBool(_hasTagetHash, false);
                }
                break;
            case EUnitState.Engage:
                if(state != EUnitState.Aim)
                {
                    _animator.SetBool(_hasTagetHash, false);
                }
                break;
            default:
                break;
        }
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
                break;
            case SOderContext.EOderType.Stop:
                break;
            case SOderContext.EOderType.Retreat:
                break;
            default:
                break;
        }
    }

    public void OderComplet()
    {
        _currentOder.oderType = SOderContext.EOderType.None;
        ChangeState(EUnitState.Idle);
    }

    public void FlipX(bool flipX)
    {
        _spriteRenderer.flipX = flipX;
    }
}
