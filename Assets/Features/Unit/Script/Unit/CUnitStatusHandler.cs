using System;
using UnityEngine;

public class CUnitStatusHandler : MonoBehaviour
{
	#region inspector
	[SerializeField] private SOUnitStatusData _unitStatusDataSO;
    [SerializeField] private CUnitController _unitController;
    #endregion

    #region property
    public event Action OnStatChange;
    public SOUnitStatusData UnitStatusDataSO => _unitStatusDataSO;

    public float MovementSpeed => GetStatusValue(_unitStatusDataSO.MovementSpeed, "MovementSpeed");
    public float WeaponRange => GetStatusValue(_unitStatusDataSO.WeaponRange, "WeaponRange");
    public float WeaponCoolDown => GetStatusValue(_unitStatusDataSO.WeaponCoolDown, "WeaponCoolDown");
    public float WeaponAimTime => GetWeaponAimTime();
    public SOSoundRelay WeaponSound => _unitStatusDataSO.WeaponSound;
    public float MaxHP => GetStatusValue(_unitStatusDataSO.MaxHp, "MaxHP");

    public SOProjectileData Projectile => _unitStatusDataSO.Projectile;
    #endregion

    private void Awake()
    {
        ExceptionCheck();
    }

    public void InitializeStat(SOUnitStatusData data)
    {
        if (data == null)
        {
            Debug.LogWarning("SOUnitStatusData is null.");
            return; 
        }

        _unitStatusDataSO = data;
        ExceptionCheck();
    }

    private void ExceptionCheck()
    {
        if (_unitController == null)
        {
            if (!TryGetComponent<CUnitController>(out _unitController))
            {
                Debug.LogWarning($"{this}/ _unitController is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
        }

        if (_unitStatusDataSO == null)
        {
            Debug.LogWarning($"{this}/ _unitStatusDataSO is not assigned.");
            enabled = false;
            return;
        }
    }

    protected virtual float GetStatusValue(float baseVal, string modifierName)
    {
        float result = baseVal;

        if (_unitController.Modifier == null)
        {
            Debug.LogWarning($"{this}/ _unitController.Modifier is not assigned.");
            return result;
        }

        result = _unitController.Modifier.GetModifiedValue(modifierName, baseVal);

        return result;
    }

    private float GetWeaponAimTime()
    {
        return UnityEngine.Random.Range
            (
                GetStatusValue(_unitStatusDataSO.WeaponAimTimeMin, "WeaponAimTime"),
                GetStatusValue(_unitStatusDataSO.WeaponAimTimeMax, "WeaponAimTime")
            );
    }

    public float GetWeaponDamage(float distance)
    {
        float baseVal = _unitStatusDataSO.GetWeaponDamageByDistance(distance);

        return GetStatusValue(baseVal, "WeaponDamage");
    }

    public void InvokeOnStatChange()
    {
        OnStatChange?.Invoke();
    }
}
