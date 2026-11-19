using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CSkillCasterController : MonoBehaviour
{
    #region const
    private const float MAP_HIGHT_MAX = 0.6F;
    private const float MAP_HIGHT_MIN = -4.5F;
    #endregion

    #region inspector
    [SerializeField] private SOPlayerData _player;
	[SerializeField] private CModifierHandler _modifierHandler;
	[SerializeField] private List<SOSkillData> _skillDataList;
	#endregion

	#region private var
	private Dictionary<SOSkillData, float> _skillDatasMap = new Dictionary<SOSkillData, float>();
	#endregion

	#region property
	public CModifierHandler ModifierHandler => _modifierHandler;
	public Dictionary<SOSkillData, float> SkiilDataMap => _skillDatasMap;
    #endregion

    private void Awake()
    {
		for (int i = 0; i < _skillDataList.Count; i++)
		{
			if (!_skillDatasMap.ContainsKey(_skillDataList[i]))
			{
				_skillDatasMap.Add(_skillDataList[i],0);
			}
		}
    }

	public bool CastSkill(SOSkillData skillData, Vector2 targetPos)
	{
		if(skillData == null)
		{
			Debug.LogWarning("SkillData is null");
			return false;
		}

		if (!_skillDatasMap.ContainsKey(skillData))
		{
			Debug.LogWarning("This Skill Caster Can't Cast that Skill.");
			return false;
		}

		if (_skillDatasMap[skillData] > 0)
		{
			Debug.Log("Skill is on CoolDown");
			return false;
		}

		if(targetPos.y > MAP_HIGHT_MAX || targetPos.y < MAP_HIGHT_MIN)
		{
			Debug.Log("Skill Can't not casted outside of map range");
			return false;
		}

		if (skillData.Range != -1)
		{
			float range = _modifierHandler.GetModifiedValue(skillData.ID + "Range" ,skillData.Range);

			float distance = Mathf.Abs(transform.position.x - targetPos.x);

			if (distance > skillData.Range)
			{
				Debug.Log("target is out of range");
				return false;
			}
		}

		if (skillData.Cost > 0)
		{
			float cost = _modifierHandler.GetModifiedValue(skillData.ID + "Cost", skillData.Cost);

			if (_player == CGameManager.Instance.PlayerData)
			{
				if (!CGameManager.Instance.SpendPlayerCMP(cost))
				{
					Debug.Log("not enugh CMP");
					return false;
				}
			}

			else if(_player == CGameManager.Instance.EnemyPlayerData)
			{
				if (!CGameManager.Instance.SpendEnemyCMP(cost))
				{
					Debug.Log("not enugh CMP");
					return false;
				}
			}
		}

		skillData.Effect.CastSkill(this, targetPos);

		if (skillData.CoolDown != -1)
		{
			float coolDown = _modifierHandler.GetModifiedValue(skillData.ID + "CoolDown", skillData.CoolDown);

			_skillDatasMap[skillData] = coolDown;
			CoolDownSkill(destroyCancellationToken, skillData).Forget();
		}

		return true;

	}

	private async UniTask CoolDownSkill(CancellationToken ct, SOSkillData skillData)
	{
		while (_skillDatasMap[skillData] > 0)
		{
			if (ct.IsCancellationRequested)
			{
				return;
			}

			if(gameObject.activeSelf == false)
			{
				return;
			}

			_skillDatasMap[skillData] -= Time.deltaTime;
			await UniTask.Yield();
		}
	}

	public void AddSkill(SOSkillData skillData)
	{
		_skillDataList.Add(skillData);
		_skillDatasMap.Add(skillData, 0);
	}

    private void OnDisable()
    {
		for (int i = 0; i < _skillDataList.Count; i++)
		{
			_skillDatasMap[_skillDataList[i]] = 0;
		}
    }
}
