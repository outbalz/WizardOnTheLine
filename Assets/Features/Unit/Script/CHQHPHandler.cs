using UnityEngine;
using UnityEngine.UI;

public class CHQHPHandler : MonoBehaviour, ITargetable
{
	#region inspector
	[SerializeField] private float _maxHP;
    [SerializeField] private Slider _hpSlider;
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
        _hp -= damage;

        _hpSlider.maxValue = _maxHP;


        if (_hp <= 0)
        {
            _hp = 0;
            //TODO game over
        }

        _hpSlider.SetValueWithoutNotify(_hp);
    }

}
