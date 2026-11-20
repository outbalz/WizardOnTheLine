using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO_", menuName = "Scriptable Objects/GameData/SkillDataSO_")]
public class SOSkillData : SOGameData
{
	#region inspector
	[SerializeField] private float _cost;
	[SerializeField] private float _coolDown;
	[SerializeField] private float _range;
	[SerializeReference, SubclassSelector] private CSkillEffect _effect;
	#endregion

	#region property
	public float Cost => _cost;
	public float CoolDown => _coolDown;
	public float Range => _range;
	public CSkillEffect Effect => _effect;
	#endregion
}

[System.Serializable]
public abstract class CSkillEffect
{
	public abstract void CastSkill(CSkillCasterController caster, Vector2 targtPos);
}

[System.Serializable]
public class CEtitySummonSkillEffect : CSkillEffect
{
	[SerializeField] private SOEntityData _entityData;

	[SerializeField] private float _randonRangeX;
	[SerializeField] private float _randonRangeY;

	[SerializeField] private int _summonAmount;
	[SerializeField] private float _summonInterval;

	[SerializeField] private string _modifierName;

    public override void CastSkill(CSkillCasterController caster, Vector2 targetPos)
    {
		SummonEntity(caster, targetPos).Forget();
    }

	public async UniTask SummonEntity(CSkillCasterController caster, Vector2 targetPos)
	{
		float ranX = caster.ModifierHandler.GetModifiedValue(_modifierName + "RanX", _randonRangeX);
		float ranY = caster.ModifierHandler.GetModifiedValue(_modifierName + "RanY", _randonRangeY);

		int amount = Mathf.RoundToInt(caster.ModifierHandler.GetModifiedValue(_modifierName + "Amount", _summonAmount));

		for (int i = 0; i < amount; i++)
		{
			Vector2 ranPos = targetPos;
			ranPos.x += Random.Range(-ranX, ranX);
			ranPos.y += Random.Range(-ranY, ranY);

			CSkillEntityController entity = CEntityPool.Instance.GetFromPool(_entityData);
			entity.transform.position = ranPos;
			entity.Initialized(caster);
			entity.gameObject.SetActive(true);

			await UniTask.WaitForSeconds(_summonInterval);
		}
	}
}