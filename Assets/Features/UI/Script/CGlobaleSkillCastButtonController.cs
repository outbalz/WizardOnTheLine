using UnityEngine;

public class CGlobaleSkillCastButtonController : MonoBehaviour
{
    [SerializeField] private CSkillCasterController _skillCaster;
    [SerializeField] private SOSkillData _skillData;

    public void CastSkill()
    {
        if(_skillData == null)
        {
            Debug.LogWarning("_skillData is null.");
            return;
        }

        if(_skillCaster == null)
        {
            Debug.LogWarning("_skillCaster is null.");
            return;
        }

        CMouseCommandController.Instance.SetCommanState(CMouseCommandController.EMouseCommadState.Skill);
        CMouseCommandController.Instance.OnClickEvent += OnSkillCastCommand;
    }

    private void OnSkillCastCommand(Vector2 targetPos)
    {
        if(CMouseCommandController.Instance.CommadState != CMouseCommandController.EMouseCommadState.Skill)
        {
            CMouseCommandController.Instance.OnClickEvent -= OnSkillCastCommand;
            return;
        }

        _skillCaster.CastSkill(_skillData, targetPos);
        CMouseCommandController.Instance.OnClickEvent -= OnSkillCastCommand;
        CMouseCommandController.Instance.SetCommanState(CMouseCommandController.EMouseCommadState.Universal);
    }
}
