using UnityEngine;

public class CUnitHPHandler : MonoBehaviour, ITargetable
{
	#region inspector
	[SerializeField] private CUnitController _unitController;
	#endregion

	#region private var
	private float _hp;
	#endregion

	#region property
	public float MaxHp => _unitController.Status.MaxHP;
	public float Armor => _unitController.Status.Armor;
    public float HP => _hp;
	#endregion

	private bool ExceptionCheck()
	{
        if (_unitController == null)
        {
            if (!TryGetComponent<CUnitController>(out _unitController))
            {
                Debug.LogWarning($"{this}/ _unitController is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return false;
            }
        }

        if(_unitController.Status == null)
        {
            Debug.LogWarning("_unitController.Status is null.");
            return false;
        }

        return true;
    }

    public void InitializeHP()
    {
        if (!ExceptionCheck())
        {
            return;
        }

        _hp = MaxHp;
    }

    public void TakeDamage(float damage)
    {
        if(_hp <= 0)
        {
            return;
        }

        damage = damage - Armor > 0 ? damage - Armor : 0.1f;

        _hp -= damage;
        //Debug.Log($"{_hp} - {damage}");

        _unitController.Squad.SquadHpUpdate();

        if (_hp <= 0)
        {
            _hp = 0;
            _unitController.KillUnit();
        }
    }

    public void TakeDamage(float damage, float armorPiercing)
    {
        if (_hp <= 0)
        {
            return;
        }

        if (armorPiercing > Armor)
        {
            armorPiercing = Armor;
        }

        float apDamage = damage - (Armor - armorPiercing) > 0 ? damage - (Armor - armorPiercing) : 0.1f;

        if(apDamage < armorPiercing)
        {
            apDamage = armorPiercing < damage ? armorPiercing : damage;
        }

        _hp -= apDamage;
        //Debug.Log($"{_hp} - {damage}");

        _unitController.Squad.SquadHpUpdate();

        if (_hp <= 0)
        {
            _hp = 0;
            _unitController.KillUnit();
        }
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }
}
