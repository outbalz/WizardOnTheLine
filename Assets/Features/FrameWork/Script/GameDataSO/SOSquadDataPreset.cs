using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadDataPresetSO_", menuName = "Scriptable Objects/GameData/SquadDataPreseSO")]
public class SOSquadDataPreset : SOGameData
{
    #region inspector
    [SerializeField] private List<SOUnitData> _unitData;
    [SerializeField] private float _cost;
    #endregion

    #region property
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitData> UnitDatas => _unitData;
    public float Cost => _cost;
    #endregion
}
