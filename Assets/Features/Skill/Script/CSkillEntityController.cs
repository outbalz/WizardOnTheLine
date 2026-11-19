using UnityEngine;

public class CSkillEntityController : MonoBehaviour
{
	#region inspector
	[SerializeField] SOEntityData _data;
	#endregion

	#region private var
	private bool _isInitialized;
	private bool _isEffectApplyed;
	private CSkillCasterController _caster;
	private float _timer;
	#endregion

	#region property
	public float Height => _data.Height;
	public float Width => _data.Width;
	public float LifeTime => _data.LifeTime;
	public float EffectTime => _data.EffectTime;
	public CSkillCasterController Caster => _caster;
	public bool IsEffectContinuous => _data.IsEffectContinuous;
	public CEntityEffect Effect => _data.Effect;
    #endregion

    public void Initialized(CSkillCasterController caster)
    {
		_timer = 0;
		_caster = caster;
		_isEffectApplyed = false;

		_isInitialized = true;
    }

    private void Update()
    {
		if (!_isInitialized)
		{
			return;
		}

        if(_timer >= LifeTime)
		{
			KillEntity();
			return;
		}

		if (IsEffectContinuous)
		{
			Effect.ApplyEffect(this);
		}

		else if (!_isEffectApplyed && _timer >= EffectTime)
		{
			Effect.ApplyEffect(this);
			_isEffectApplyed = true;
		}

		_timer += Time.deltaTime;
    }

	private void KillEntity()
	{
		_caster = null;
		_isInitialized = false;

		CEntityPool.Instance.ReturnToPool(this, _data);
	}
}
