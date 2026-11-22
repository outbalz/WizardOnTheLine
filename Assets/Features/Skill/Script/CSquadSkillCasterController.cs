using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Threading;
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
    public CSquadContorller Squad => _squadContorller;
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

    public void InitializeSquadSkill(ISquadReferance data)
    {
        ClearSkill();
        SetUnitSkillCasters();

        if(data.SquadSkill.Count == 0 && data.UnitSkill.Count == 0)
        {
            return;
        }

        for (int i = 0; i < data.SquadSkill.Count; i++)
        {
            AddSkill(data.SquadSkill[i]);
        }

        for (int i = 0; i < _unitSkillCaster.Count; i++)
        {
            for (int j = 0; j < data.UnitSkill.Count; j++)
            {
                _unitSkillCaster[i].AddSkill(data.UnitSkill[j]);
            }
        }

    }
    private void FindTarget(SOSkillData skillData)
    {
        Vector2 targetPos = Vector2.positiveInfinity;
        bool hasTarget = false;

        if(_squadContorller.SquadMembers.Count <= 0)
        {
            return;
        }

        Vector2 squadPos = _squadContorller.SquadMembers[0].transform.position;

        Vector2 pointA = new Vector2 (squadPos.x - skillData.Range, CUtil.MAP_HIGHT_MAX);
        Vector2 pointB = new Vector2 (squadPos.x + skillData.Range, CUtil.MAP_HIGHT_MIN);

        Collider2D[] targets = Physics2D.OverlapAreaAll(pointA, pointB, _squadContorller.PlayerData.TargetLayer);

        if (targets.Length > 0)
        {
            float closistDistance = float.MaxValue;

            for (int i = 0; i < targets.Length; i++)
            {
                float distance = Mathf.Abs(targets[i].transform.position.x - squadPos.x);
                
                if(distance > skillData.Range)
                {
                     //Debug.Log("??");
                     continue;
                }

                if (distance < closistDistance)
                {
                     closistDistance = distance;
                     targetPos = targets[i].transform.position;
                     hasTarget = true;
                }

                if(targetPos.y > CUtil.MAP_HIGHT_MAX)
                {
                    targetPos.y = CUtil.MAP_HIGHT_MAX -1;
                }

            }
            /*
            Debug.DrawLine(squadPos, targetPos, Color.yellow, 5f);
            Debug.Log($"[FindTarget] frame={Time.frameCount} skill={skillData.name} " +
                     $"origin={squadPos:F2} aim={targetPos:F2} " +
                     $"dx={Mathf.Abs(targetPos.x - squadPos.x):F2} range={skillData.Range}");
            */
            
            if (hasTarget)
            {
                CastSkill(skillData, targetPos);
                return;
            }
        }
            
    }
    
    /*
    private async UniTask SkillUpdate(CancellationToken ct, SOSkillData skillData)
    {
        while (!ct.IsCancellationRequested)
        {
            while (SkiilDataMap[skillData] > 0)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                await UniTask.Yield();
            }

            if (ct.IsCancellationRequested)
            {
                return;
            }

            await FindTarget(ct, skillData);
        }

    }
    */

    private void FixedUpdate()
    {
        for (int i = 0; i < _skillDataList.Count; i++)
        {
            if (SkiilDataMap[_skillDataList[i]] <= 0)
            {
                FindTarget(_skillDataList[i]);
            }
        }
    }

    protected override void OnDisable()
    {
        _unitSkillCaster.Clear();
        base.OnDisable();
    }
}
