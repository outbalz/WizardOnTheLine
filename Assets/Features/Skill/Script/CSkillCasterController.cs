using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class CSkillCasterController : MonoBehaviour
{

    #region inspector
    [SerializeField] private SOPlayerData _player;
	[SerializeField] private CModifierHandler _modifierHandler;
	[SerializeField] protected List<SOSkillData> _skillDataList;
	#endregion

	#region private var
	private Dictionary<SOSkillData, float> _skillDatasMap = new Dictionary<SOSkillData, float>();
	protected Dictionary<SOSkillData, CancellationTokenSource> _skillCTSMap = new Dictionary<SOSkillData, CancellationTokenSource>();
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
				_skillCTSMap.Add(_skillDataList[i], new CancellationTokenSource());
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

		if(targetPos.y > CUtil.MAP_HIGHT_MAX || targetPos.y < CUtil.MAP_HIGHT_MIN)
		{
			Debug.Log($" {targetPos} Skill Can't not casted outside of map range");
			return false;
		}

		/*
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
		*/

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

			Cancel(skillData);
			_skillCTSMap[skillData] = new CancellationTokenSource();
			CoolDownSkill(_skillCTSMap[skillData].Token, skillData).Forget();
		}

		return true;

	}

	protected async UniTask CoolDownSkill(CancellationToken ct, SOSkillData skillData)
	{
		if (ct.IsCancellationRequested)
		{
			return;
		}

		while (_skillDatasMap[skillData] > 0)
		{
			if (ct.IsCancellationRequested)
			{
				return;
			}

			if (gameObject == null)
			{
				return;
			}

			if(gameObject.activeSelf == false)
			{
				return;
			}

			_skillDatasMap[skillData] -= Time.deltaTime;
			await UniTask.Yield();

			if (ct.IsCancellationRequested)
			{
				return;
			}
		}
	}

	public void AddSkill(SOSkillData skillData)
	{
		_skillDataList.Add(skillData);
		_skillDatasMap.Add(skillData, 0);
		_skillCTSMap.Add(skillData, new CancellationTokenSource());
	}

	public void SetPlayer(SOPlayerData playerData)
	{
		_player = playerData;
	}
	
	public void ClearSkill()
	{
        for (int i = 0; i < _skillDataList.Count; i++)
        {
            Cancel(_skillDataList[i]);
        }

        _skillDataList.Clear();
		_skillDatasMap.Clear();
		_skillCTSMap.Clear();
	}

    protected virtual void OnDisable()
    {
		ClearSkill();
    }

    private void OnDestroy()
    {
        ClearSkill();
    }

	public void Cancel(SOSkillData key)
	{
		if (!_skillCTSMap.ContainsKey(key))
		{
			return;
		}

		if (_skillCTSMap[key] == null)
		{
			return;
		}

		_skillCTSMap[key].Cancel();
		_skillCTSMap[key].Dispose();
		_skillCTSMap[key] = null;
	}
}
