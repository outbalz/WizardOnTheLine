using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadDataPresetSO_", menuName = "Scriptable Objects/GameData/SquadDataPreseSO")]
public class SOSquadDataPreset : SOGameData
{
    #region inspector
    [SerializeField] private List<SOUnitStatusData> _unitData;
    #endregion

    #region property
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitStatusData> UnitDatas => _unitData;
    #endregion
}
