using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO_", menuName = "Scriptable Objects/GameData/UnitDataSO")]
public class SOUnitData : ScriptableObject
{
	#region inspector
	[SerializeField] private SOUnitStatusData _statusData;
	[SerializeField] private AnimatorOverrideController _animatorOverrideController;
	#endregion

	#region property
	public SOUnitStatusData StatusData => _statusData;
	public AnimatorOverrideController AnimatorOverrideController => _animatorOverrideController;
	#endregion
}
