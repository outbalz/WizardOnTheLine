using UnityEngine;


public class CUnitController : MonoBehaviour
{
	#region inspector
	[SerializeField] private CUnitStatusHandler _unitStatusHandler;
	[SerializeField] private CUnitMovementController _unitMovementController;
	[SerializeField] private CModifierHandler _modifierHandler;
	#endregion

	#region private var
	private CSquadContorller _squad;
	#endregion

	#region property
	public CSquadContorller Squad => _squad;
	public CUnitStatusHandler Status => _unitStatusHandler;
	public CUnitMovementController Movement => _unitMovementController;
	public CModifierHandler Modifier => _modifierHandler;
    #endregion

    public void InitializeUnit(CSquadContorller squad, SOUnitStatusData data)
    {
		if (squad == null)
		{
			Debug.LogWarning("CSquadContorller is null.");
			return;
		}

        _squad = squad;

		ExceptionCheck();

		_unitStatusHandler.InitializeStat(data);
    }

	private void ExceptionCheck()
	{
		if (_unitStatusHandler == null)
		{
			if (!TryGetComponent<CUnitStatusHandler>(out _unitStatusHandler))
			{
				Debug.LogWarning($"{this}/ _unitStatusHandler is not assigned and not found on the GameObject {gameObject.name}.");
				enabled = false;
				return;
			}
		}

		if (_modifierHandler == null)
		{
			if (!TryGetComponent<CModifierHandler>(out _modifierHandler))
			{
				Debug.LogWarning($"{this}/ _modifierHandler is not assigned and not found on the GameObject {gameObject.name}."); 
				return;
			}
		}

		if (_unitMovementController == null)
		{
			if(!TryGetComponent<CUnitMovementController>(out _unitMovementController))
			{
				Debug.LogWarning($"{this}/ _unitMovementController is not assigned and not found on the GameObject {gameObject.name}.");
				enabled = false;
				return;
			}
		}
	}
}
