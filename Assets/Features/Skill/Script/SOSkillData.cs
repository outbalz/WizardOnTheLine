using Cysharp.Threading.Tasks;
using UnityEditor.Tilemaps;
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

[System.Serializable]
public class CProjectileSummonSkillEffect : CSkillEffect
{
	[SerializeField] private SOProjectileData _projectileData;

    [SerializeField] private float _randonRangeX;
    [SerializeField] private float _randonRangeY;

    [SerializeField] private int _summonAmount;
    [SerializeField] private float _summonInterval;

    [SerializeField] private string _modifierName;

    public override void CastSkill(CSkillCasterController caster, Vector2 targetPos)
    {
        SummonProjectile(caster, targetPos).Forget();
    }

    public async UniTask SummonProjectile(CSkillCasterController caster, Vector2 targetPos)
    {
        float ranX = caster.ModifierHandler.GetModifiedValue(_modifierName + "RanX", _randonRangeX);
        float ranY = caster.ModifierHandler.GetModifiedValue(_modifierName + "RanY", _randonRangeY);


        int amount = Mathf.RoundToInt(caster.ModifierHandler.GetModifiedValue(_modifierName + "Amount", _summonAmount));

        for (int i = 0; i < amount; i++)
        {
            Vector2 ranPos = targetPos;
            ranPos.x += Random.Range(-ranX, ranX);
            ranPos.y += Random.Range(-ranY, ranY);
			//Debug.Log(ranPos);

            CProjectileController projectile = CProjectilePool.Instance.GetFromPool(_projectileData);
			projectile.transform.position = caster.transform.position;
            projectile.Initialized(caster.transform.position, ranPos);
            projectile.gameObject.SetActive(true);

            await UniTask.WaitForSeconds(_summonInterval);
        }
    }
}

[System.Serializable]
public class CSquadSkillCastEffect : CSkillEffect
{
	[SerializeField] private SOSkillData _skillData;
	[SerializeField] private int _castAmount;
	[SerializeField] private float _sreachRange;
    [SerializeField] private string _modifierName;

    public override void CastSkill(CSkillCasterController caster, Vector2 targtPos)
    {
		if(caster is not CSquadSkillCasterController squadSkillCaster)
		{
			return;
		}

		int[] ranIndex = new int[squadSkillCaster.UnitSkillCaster.Count];

		for (int i = 0; i < ranIndex.Length; i++)
		{
			ranIndex[i] = i;
		}

		for (int i = ranIndex.Length -1; i > 0; i--)
		{
			int ran = Random.Range(0, i +1);
			int temp =  ranIndex[i];
			ranIndex[i] = ranIndex[ran];
			ranIndex[ran] = temp;

		}

		int castAmount = Mathf.RoundToInt(squadSkillCaster.ModifierHandler.GetModifiedValue(_modifierName, _castAmount));

		if (castAmount > ranIndex.Length)
		{
			castAmount = ranIndex.Length;
		}

		Vector2 pointA = new Vector2(targtPos.x - _sreachRange, CUtil.MAP_HIGHT_MAX);
		Vector2 pointB = new Vector2(targtPos.x + _sreachRange, CUtil.MAP_HIGHT_MIN);

		Collider2D[] targets = Physics2D.OverlapAreaAll(pointA, pointB, squadSkillCaster.Squad.PlayerData.TargetLayer);

		if (targets.Length <= 0)
		{
			return;
		}

		for (int i = 0; i < castAmount; i++)
		{
			float closistDitance = float.MaxValue;
			Vector2 closistPos = targtPos;

			for (int j = 0; j < targets.Length; j++)
			{
				if (targets[j].CompareTag("SquadMarker"))
				{
					continue;
				}

				if (Mathf.Abs(targets[j].transform.position.x - targtPos.x) > _sreachRange)
				{
					continue;
				}

				float distance = (targets[j].transform.position - squadSkillCaster.UnitSkillCaster[i].transform.position).sqrMagnitude;

				if (distance < closistDitance)
				{
					closistDitance = distance;
					closistPos = targets[j].transform.position;
				}
			}

			if(closistPos.y > CUtil.MAP_HIGHT_MAX)
			{
				closistPos.y = CUtil.MAP_HIGHT_MAX -1;
			}

			squadSkillCaster.UnitSkillCaster[ranIndex[i]].CastSkill(_skillData, closistPos);

		}
    }
}
