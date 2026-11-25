using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDataSO_", menuName = "Scriptable Objects/GameData/UpgradeDataSO")]
public class SOUpgradeData : SOGameData, IRewardable
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

    [Space]
    [Space]
    [Header("Rwardable")]
    [SerializeField] private Sprite _upgradeRewardSprite;
    [SerializeField] private string _unitName;
    [SerializeField] private string _unitSubName;
    [TextArea]
    [SerializeField] private string _description;

    [SerializeReference, SubclassSelector] private CCondition[] _unlockCondition;
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

    public string TargetName => _unitName;
    public string SubName => _unitSubName;
    public string Description => _description;
    public Sprite RewardSprite => _upgradeRewardSprite;
    public CCondition[] UnlockCondition => _unlockCondition;

    public override string Name
    {
        get
        {
            IReadOnlyList<SOUpgradeData> upgradeList = CGameProgressManager.Instance.UpgradeList;

            int stack = 0;

            for (int i = 0; i < upgradeList.Count; i++)
            {
                if (upgradeList[i] == this)
                {
                    stack++;

                }
            }

            return stack == 0 ? _name : $"{_name} ({stack+1})";
        }
    }
    #endregion
}

[System.Serializable]
public struct SMemberReplacement
{
    public int IndexNum;
    public SOUnitData RepalceUnitData;
}