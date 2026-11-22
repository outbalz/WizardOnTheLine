using System.Collections.Generic;
using UnityEngine;

public interface ISquadReferance
{
    public int SquadSize { get; }
    public IReadOnlyList<SOUnitData> UnitDatas { get; }
    public IReadOnlyList<SOSkillData> SquadSkill { get; }
    public IReadOnlyList<SOSkillData> UnitSkill { get; }
    public IReadOnlyList<CModifier> SquadModifier { get; }
    public IReadOnlyList<CModifier> UnitModifier { get; }
    public Sprite MarkerSprite { get; }
    public float Cost { get; }
}

[CreateAssetMenu(fileName = "SquadDataPresetSO_", menuName = "Scriptable Objects/GameData/SquadDataPreseSO")]
public class SOSquadDataPreset : SOGameData, ISquadReferance
{
    #region inspector
    [SerializeField] private List<SOUnitData> _unitData;
    [SerializeField] private List<SOSkillData> _squadSkill;
    [SerializeField] private List<SOSkillData> _unitSkill;
    [SerializeField] private Sprite _markerSprite;
    [SerializeField] private float _cost;

    [SerializeField] private Sprite _potraitSprite;
    #endregion

    #region property
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitData> UnitDatas => _unitData;
    public IReadOnlyList<SOSkillData> SquadSkill => _squadSkill;
    public IReadOnlyList<SOSkillData> UnitSkill => _unitSkill;
    public IReadOnlyList<CModifier> SquadModifier => null;
    public IReadOnlyList<CModifier> UnitModifier => null;
    public Sprite MarkerSprite => _markerSprite;
    public float Cost => _cost;

    public Sprite PotraitSprite => _potraitSprite;
    #endregion
}
