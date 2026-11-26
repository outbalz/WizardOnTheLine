using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeDataSO_", menuName = "Scriptable Objects/GameData/SkillUpgradeDataSO")]
public class SOSkillUpgradeData : SOGameData, IRewardable
{
    #region inspector
    [SerializeField] private SOSkillData _skill;
    [SerializeReference, SubclassSelector] private CModifier[] _modifiers;
    //[SerializeField] private float _costChange = 0;

    //[SerializeField] private Sprite _upgradeMakerSprite;
    //[SerializeField] private Sprite _potraitSprite;

    [Space]
    [Space]
    [Header("Rwardable")]
    [SerializeField] private Sprite _upgradeRewardSprite;
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillSubName;
    [TextArea]
    [SerializeField] private string _description;

    [SerializeReference, SubclassSelector] private CCondition[] _unlockCondition;
    #endregion

    #region property
    public CModifier[] Modifiers => _modifiers;
    public string TargetName => _skillName;
    public string SubName => _skillSubName;
    public string Description => _description;
    public Sprite RewardSprite => _upgradeRewardSprite;
    public CCondition[] UnlockCondition => _unlockCondition;
    public override string Name
    {
        get
        {
            IReadOnlyList<SOSkillUpgradeData> upgradeList = CGameProgressManager.Instance.SkillUpgradeList;

            int stack = 0;

            for (int i = 0; i < upgradeList.Count; i++)
            {
                if (upgradeList[i] == this)
                {
                    stack++;
                }
            }

            return stack == 0 ? _name : $"{_name} ({stack + 1})";
        }
    }
    #endregion


}
