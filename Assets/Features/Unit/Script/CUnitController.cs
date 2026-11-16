using UnityEngine;


public class CUnitController : MonoBehaviour, ITargetable
{
	#region inspector
	[SerializeField] private CUnitStatusHandler _unitStatusHandler;
	[SerializeField] private CUnitMovementController _unitMovementController;
	[SerializeField] private CModifierHandler _modifierHandler;
	[SerializeField] private CUnitStateContoller _stateContoller;
	[SerializeField] private CUnitTargetFinder _targetFinder;

	[Space]
	[Header("UnitSelection")]
	[SerializeField] private SpriteRenderer _selectionCircle;
    #endregion

    #region private var
    private CSquadContorller _squad;
	#endregion

	#region property
	public CSquadContorller Squad => _squad;
	public CUnitStateContoller StateContoller => _stateContoller;
	public CUnitStatusHandler Status => _unitStatusHandler;
	public CUnitMovementController Movement => _unitMovementController;
	public CModifierHandler Modifier => _modifierHandler;
	public CUnitTargetFinder TargetFinder => _targetFinder;
    #endregion

    public void InitializeUnit(CSquadContorller squad, SOUnitData data)
    {
		if (squad == null)
		{
			Debug.LogWarning("CSquadContorller is null.");
			return;
		}

        _squad = squad;

		ExceptionCheck();

		_unitStatusHandler.InitializeStat(data.StatusData);
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

		if(_stateContoller == null)
		{
			if(!TryGetComponent<CUnitStateContoller>(out _stateContoller))
			{
				Debug.LogWarning($"{this}/ _stateContoller is not assigned and not found on the GameObject {gameObject.name}.");
				enabled = false;
				return;
			}
		}

		if(_targetFinder == null)
		{
			if(!TryGetComponent <CUnitTargetFinder>(out _targetFinder))
			{
				Debug.LogWarning($"{this}/ _targetFinder is not assigned and not found on the GameObject {gameObject.name}.");
				enabled = false;
				return;
			}
		}
	}

	public void SelectUint(bool isSelected)
	{
		Color32 color = _selectionCircle.color;

		if(isSelected)
		{
			color.a = 255;
			_selectionCircle.color = color;
		}

		else
		{
			color.a = 50;
			_selectionCircle.color = color;
		}
	}
}
