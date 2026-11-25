using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CHQHPHandler : MonoBehaviour, ITargetable
{
	#region inspector
	[SerializeField] private float _maxHP;
    [SerializeField] private Slider _hpSlider;

    [SerializeField] private UnityEvent _destroyEvent;
	#endregion

	#region private var
	private float _hp = 1;
    #endregion

    private void Awake()
    {
        _hp = _maxHP;
    }

    public void TakeDamage(float damage)
    {
        if(_hp <= 0)
        {
            return;
        }

        _hp -= damage;

        _hpSlider.maxValue = _maxHP;


        if (_hp <= 0)
        {
            _hp = 0;
            
            _destroyEvent.Invoke();
            enabled = false;
        }

        _hpSlider.SetValueWithoutNotify(_hp);
    }

}
