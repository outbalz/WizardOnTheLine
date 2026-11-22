using System.Collections.Generic;
using UnityEngine;

public class CSquadData
{
    #region inspector
    [SerializeField] private string _name;
    [SerializeField] private SOSquadDataPreset _preset;
	[SerializeField] private List<SOUnitStatusData> _unitData;
    [SerializeField] private Sprite _markerSprite;
    [SerializeField] private List<CModifier> _squadModifier;
    [SerializeField] private List<CModifier> _unitModifier;
    [SerializeField] private float _cost;
    #endregion

    #region property
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitStatusData> UnitDatas => _unitData;
	#endregion
}
