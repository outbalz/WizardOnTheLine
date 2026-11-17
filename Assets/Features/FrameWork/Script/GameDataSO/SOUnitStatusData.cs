using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusDataSO_", menuName = "Scriptable Objects/GameData/UnitStatusDataSO")]
public class SOUnitStatusData : SOGameData
{
    #region inspector
    [Header("Movement")]
	[SerializeField, Min(0f)] private float _movementSpeed = 2.5f;

    [Header("Weapon")]
    [SerializeField, Min(0f)] private float _weaponRange;
    [SerializeField, Min(0f)] private float _weaponDamage;
    [SerializeField, Min(0f)] private float _weaponAimTimeMin;
    [SerializeField, Min(0f)] private float _weaponAimTimeMax;
    [SerializeField, Min(0f)] private float _weaponCoolDown;
    [SerializeField] private AnimationCurve _weaponDamageFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("HP")]
    [SerializeField, Min(1f)] private float _maxHp;
    #endregion

    #region property
    public float MovementSpeed => _movementSpeed;
    public float WeaponRange => _weaponRange;
    public float WeaponDamage => _weaponDamage;
    public float WeaponAimTimeMin => _weaponAimTimeMin;
    public float WeaponAimTimeMax => _weaponAimTimeMax;
    public float WeaponCoolDown => _weaponCoolDown;
    public float MaxHp => _maxHp;
    #endregion

    public float GetWeaponDamageByDistance(float distance)
    {
        if(_weaponDamage <= 0f)
        {
            return 0;
        }

        if(distance > _weaponRange)
        {
            return 0;
        }

        float t = distance/_weaponRange;
        float damege = _weaponDamage * _weaponDamageFalloff.Evaluate(t);

        return damege;
    }

}
