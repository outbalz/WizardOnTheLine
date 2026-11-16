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
	private bool _isInCover;

	private readonly int _isMoveingHash = Animator.StringToHash("bIsMoveing");
	private readonly int _isInCoverHash = Animator.StringToHash("bIsInCover");
	private readonly int _hasTagetHash = Animator.StringToHash("bHasTaget");
	private readonly int _fireWeaponhash = Animator.StringToHash("tFireWeapon");
    #endregion
}
