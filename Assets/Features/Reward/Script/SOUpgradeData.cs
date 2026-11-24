using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDataSO_", menuName = "Scriptable Objects/GameData/UpgradeDataSO")]
public class SOUpgradeData : SOGameData
{
    #region inspector
    [SerializeField] private string _upgradeTag;
    [SerializeField] private SOSquadDataPreset _preset;
    [SerializeReference, SubclassSelector] private List<CModifier> _squadModifier;
    [SerializeReference, SubclassSelector] private List<CModifier> _unitModifier;
    [SerializeField] private List<SOSkillData> _squadSkillData;
    [SerializeField] private List<SOSkillData> _unitSkillData;
    [SerializeField] private Sprite _upgradeMakerSprite;
    [SerializeField] private List<SMemberReplacement> _replacement;
    [SerializeField] private float _costChange = 0;

    [SerializeField] private Sprite _potraitSprite;
    #endregion

    #region property
    public string UpgradeTag => _upgradeTag;
    public SOSquadDataPreset Preset => _preset;
    public IReadOnlyList<CModifier> SquadModifier => _squadModifier;
    public IReadOnlyList<CModifier> UnitModifier => _unitModifier;
    public IReadOnlyList<SOSkillData> SquadSkillData => _squadSkillData;
    public IReadOnlyList<SOSkillData> UnitSkillData => _unitSkillData;
    public Sprite UpgradeMakerSprite => _upgradeMakerSprite;
    public IReadOnlyList<SMemberReplacement> Replacement => _replacement;
    public float CostChange => _costChange;

    public Sprite PotraitSprite => _potraitSprite;
    #endregion
}

[System.Serializable]
public struct SMemberReplacement
{
    public int IndexNum;
    public SOUnitData RepalceUnitData;
}