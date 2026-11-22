using System.Collections.Generic;
using UnityEngine;

public class CSquadData : ISquadReferance
{
    #region inspector
    [SerializeField] private string _upgradeTag;
    [SerializeField] private SOSquadDataPreset _preset;
	[SerializeField] private List<SOUnitData> _unitData;
    [SerializeField] private Sprite _markerSprite;
    [SerializeField] private List<CModifier> _squadModifier;
    [SerializeField] private List<CModifier> _unitModifier;
    [SerializeField] private List<SOSkillData> _squadSkillData;
    [SerializeField] private List<SOSkillData> _unitSkillData;
    [SerializeField] private float _cost;
    #endregion

    #region property
    public string UpgradeTag => _upgradeTag;
    public SOSquadDataPreset Preset => _preset;
    public int SquadSize => _unitData.Count;
    public IReadOnlyList<SOUnitData> UnitDatas => _unitData;
    public Sprite MarkerSprite => _markerSprite;
    public IReadOnlyList<CModifier> SquadModifier => _squadModifier;
    public IReadOnlyList<CModifier> UnitModifier => _unitModifier;
    public IReadOnlyList<SOSkillData> SquadSkill => _squadSkillData;
    public IReadOnlyList<SOSkillData> UnitSkill => _unitSkillData;
    public float Cost => _cost;
    #endregion

    public CSquadData(string tag, SOSquadDataPreset preset)
    {
        _upgradeTag = tag;
        _preset = preset;
        _markerSprite = preset.MarkerSprite;
        _squadModifier = new List<CModifier>();
        _unitModifier = new List<CModifier>();
        
        _squadSkillData = new List<SOSkillData>(preset.SquadSkill);
        _unitSkillData = new List<SOSkillData>(preset.UnitSkill);
        _cost = preset.Cost;
    }

    public bool ApplyUpgrade(SOUpgradeData upgrade)
    {
        if (upgrade == null)
        {
            return false;
        }

        if (upgrade.UpgradeTag != "All" && upgrade.UpgradeTag != _upgradeTag)
        {
            return false;
        }

        if(upgrade.UpgradeMakerSprite != null)
        {
            _markerSprite = upgrade.UpgradeMakerSprite;
        }

        if(upgrade.SquadModifier.Count > 0)
        {
            _squadModifier.AddRange(upgrade.SquadModifier);
        }

        if (upgrade.UnitModifier.Count > 0)
        {
            _unitModifier.AddRange(upgrade.UnitModifier);
        }

        if(upgrade.Replacement.Count > 0)
        {
            for (int i = 0; i < upgrade.Replacement.Count; i++)
            {
                SMemberReplacement replacement = upgrade.Replacement[i];

                if(replacement.IndexNum > _unitData.Count -1)
                {
                    continue;
                }

                _unitData[replacement.IndexNum] = replacement.RepalceUnitData;
            }
        }

        if(upgrade.SquadSkillData.Count > 0)
        {
            _squadSkillData.AddRange(upgrade.SquadSkillData);
        }

        if(upgrade.UnitSkillData.Count > 0)
        {
            _unitSkillData.AddRange(upgrade.UnitSkillData);
        }

        _cost += upgrade.CostChange;

        return true;
    }
}
