using UnityEngine;

public class CUnitController : MonoBehaviour
{
	#region inspector
	[SerializeField] private CUnitStatusHandler _unitStatusHandler;
	[SerializeField] private CUnitMovementController _unitMovementController;
	[SerializeField] private CModifierHandler _modifierHandler;
    #endregion

    #region property
    public CUnitStatusHandler Status => _unitStatusHandler;
	public CUnitMovementController Movement => _unitMovementController;
	public CModifierHandler Modifier => _modifierHandler;
    #endregion
}
