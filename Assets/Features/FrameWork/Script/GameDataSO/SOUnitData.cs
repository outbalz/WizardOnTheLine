using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO_", menuName = "Scriptable Objects/GameData/UnitDataSO")]
public class SOUnitData : ScriptableObject
{
	#region inspector
	[SerializeField] private SOUnitStatusData _statusData;

	[Space]
	[SerializeField] private Sprite _selectedCircle;
	[SerializeField] private Sprite _deselectedCircle;
	#endregion

	#region property
	public SOUnitStatusData StatusData => _statusData;

	public Sprite SelectedCircle => _selectedCircle;
	public Sprite DeselectedCircle => _deselectedCircle;
	#endregion
}
