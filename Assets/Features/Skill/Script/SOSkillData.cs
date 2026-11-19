using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO_", menuName = "Scriptable Objects/GameData/SkillDataSO_")]
public class SOSkillData : SOGameData
{
	#region inspector
	[SerializeField] private float _cost;
	[SerializeReference, SubclassSelector] private CSkillEffect _effect;
	#endregion
}

[System.Serializable]
public abstract class CSkillEffect
{
	public abstract void CastSkill(CSkillCasterController caster, Vector2 targtPos);
}