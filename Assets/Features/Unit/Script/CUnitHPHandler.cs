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
        _hp -= damage;
        Debug.Log($"{_hp} - {damage}");

        if (_hp <= 0)
        {
            _unitController.KillUnit();
        }
    }
}
