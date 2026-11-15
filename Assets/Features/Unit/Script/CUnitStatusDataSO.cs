using UnityEngine;

[CreateAssetMenu(fileName = "CUnitStatusDataSO", menuName = "Scriptable Objects/CUnitStatusDataSO")]
public class CUnitStatusDataSO : CGameDataSO
{
	#region inspector
	[SerializeField] float _movementSpeed;
    #endregion

    #region property
    public float MovementSpeed => _movementSpeed;
    #endregion


}
