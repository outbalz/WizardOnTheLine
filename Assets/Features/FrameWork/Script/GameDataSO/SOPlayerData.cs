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
	public int UnitLayer => Mathf.RoundToInt(Mathf.Log(_unitLayer.value,2));
	public LayerMask TargetLayer => _targetLayer;
	#endregion
}
