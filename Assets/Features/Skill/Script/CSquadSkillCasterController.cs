using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CSquadSkillCasterController : CSkillCasterController
{
    #region inspector
    [SerializeField] private CSquadContorller _squadContorller;
    #endregion

    #region private var
    private List<CSkillCasterController> _unitSkillCaster = new List<CSkillCasterController>();
    #endregion

    #region property
    public IReadOnlyList<CSkillCasterController> UnitSkillCaster => _unitSkillCaster;
    #endregion

    public void SetUnitSkillCasters()
    {
        _unitSkillCaster.Clear();

        for (int i = 0; i < _squadContorller.SquadMembers.Count; i++)
        {
            _unitSkillCaster.Add(_squadContorller.SquadMembers[i].SkillCaster);
        }
    }

    public void InitializeSquadSkill(SOSquadDataPreset dataPreset)
    {
        ClearSkill();
        SetUnitSkillCasters();

        if(dataPreset.SquadSkill.Count == 0 && dataPreset.UnitSkill.Count == 0)
        {
            return;
        }

        for (int i = 0; i < dataPreset.SquadSkill.Count; i++)
        {
            AddSkill(dataPreset.SquadSkill[i]);
        }

        for (int i = 0; i < _unitSkillCaster.Count; i++)
        {
            for (int j = 0; j < dataPreset.UnitSkill.Count; j++)
            {
                _unitSkillCaster[i].AddSkill(dataPreset.UnitSkill[j]);
            }
        }
    }
}
