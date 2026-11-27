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
    [SerializeField, Min(0f)] private float _armorPiercing;
    [SerializeField] private SOSoundRelay _weaponSound;

    [Header("HP")]
    [SerializeField, Min(1f)] private float _maxHp;
    [SerializeField] private float _armor;

    [Header("Projectile")]
    [SerializeField] private SOProjectileData _projectile;
    #endregion

    #region property
    public float MovementSpeed => _movementSpeed;
    public float WeaponRange => _weaponRange;
    public float WeaponDamage => _weaponDamage;
    public float WeaponAimTimeMin => _weaponAimTimeMin;
    public float WeaponAimTimeMax => _weaponAimTimeMax;
    public float WeaponCoolDown => _weaponCoolDown;
    public float ArmorPiercing => _armorPiercing;
    public float MaxHp => _maxHp;
    public float Armor => _armor;
    public SOSoundRelay WeaponSound => _weaponSound;
    public SOProjectileData Projectile => _projectile;
    #endregion

    public float GetWeaponDamageByDistance(float distance)
    {
        if(_weaponDamage <= 0f)
        {
            return 0;
        }

        /*
        if(distance > _weaponRange)
        {
            return 0;
        }
        */
        float t = distance/_weaponRange;
        float damege = _weaponDamage * _weaponDamageFalloff.Evaluate(Mathf.Clamp01(t));

        return damege;
    }

}
