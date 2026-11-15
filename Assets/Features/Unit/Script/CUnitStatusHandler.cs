using System;
using UnityEngine;

public class CUnitStatusHandler : MonoBehaviour
{
	#region inspector
	[SerializeField] CUnitStatusDataSO _unitStatusDataSO;
    [SerializeField] CUnitController _unitController;
    #endregion

    #region property
    public event Action OnStatChange;
    public CUnitStatusDataSO UnitStatusDataSO => _unitStatusDataSO;

    public float MovementSpeed => GetStatusValue(_unitStatusDataSO.MovementSpeed, "MovementSpeed");
    #endregion

    private void Awake()
    {
        if (_unitController == null)
        {
            if(!TryGetComponent<CUnitController>(out _unitController))
            {
                Debug.LogWarning($"{this}/ _unitController is null");
                enabled = false;
                return;
            }
        }

        if (_unitStatusDataSO == null)
        {
            Debug.LogWarning($"{this}/ _unitStatusDataSO is null");
            enabled = false;
            return;
        }
    }

    protected virtual float GetStatusValue(float baseVal, string modifierName)
    {
        float result = baseVal;

        if (_unitController.Modifier == null)
        {
            Debug.LogWarning($"{this}/ _unitController.Modifier is null");
            return result;
        }

        result = _unitController.Modifier.GetModifiedValue(modifierName, baseVal);

        return result;
    }

    public void InvokeOnStatChange()
    {
        OnStatChange?.Invoke();
    }
}
