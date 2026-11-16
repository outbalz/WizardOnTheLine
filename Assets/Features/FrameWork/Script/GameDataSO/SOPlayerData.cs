using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerData", menuName = "Scriptable Objects/GameData/SOPlayerData")]
public class SOPlayerData : SOGameData
{
	#region inspector
	[SerializeField] private bool _isFaceingLeft = false;
	[SerializeField] private Color32 _playerColor;
	[SerializeField] private LayerMask _unitLayer;
	[SerializeField] private LayerMask _targetLayer;
	#endregion

	#region property
	public bool IsFaceingLeft => _isFaceingLeft;
	public Color32 PlayerColor => _playerColor;
	public LayerMask UnitLayer => _unitLayer;
	public LayerMask TargetLayer => _targetLayer;
	#endregion
}
