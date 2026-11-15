using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusDataSO_", menuName = "Scriptable Objects/GameData/UnitStatusDataSO")]
public class SOUnitStatusData : SOGameData
{
	#region inspector
	[SerializeField, Min(0f)] float _movementSpeed = 2.5f;
    #endregion

    #region property
    public float MovementSpeed => _movementSpeed;
    #endregion

}
