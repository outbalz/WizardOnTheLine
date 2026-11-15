using System.Collections.Generic;
using UnityEngine;

public class CSquadData
{
	#region inspector
	[SerializeField] private List<SOUnitStatusData> _unitData;
    #endregion

    #region property
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitStatusData> UnitDatas => _unitData;
	#endregion
}
