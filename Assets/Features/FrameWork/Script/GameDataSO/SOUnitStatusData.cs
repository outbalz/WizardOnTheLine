using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusDataSO_", menuName = "Scriptable Objects/GameData/UnitStatusDataSO")]
public class SOUnitStatusData : SOGameData
{
	#region inspector
	[SerializeField] float _movementSpeed;
    #endregion

    #region property
    public float MovementSpeed => _movementSpeed;
    #endregion


}
