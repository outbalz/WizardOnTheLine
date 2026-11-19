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

