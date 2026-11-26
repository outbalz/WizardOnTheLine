using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CCondition
{
    public abstract bool CheckCondition();
}


[Serializable]
public class CNotUseableCondition : CCondition
{
    public override bool CheckCondition()
    {
        return false;
    }
}

[Serializable]
public class CUpgradeMaxStackCondition : CCondition
{
    [SerializeField] private SOUpgradeData _upgrade;
    [SerializeField] private int _maxStack;

    public override bool CheckCondition()
    {
        IReadOnlyList<SOUpgradeData> upgradeList = CGameProgressManager.Instance.UpgradeList;

        int stack = 0;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            if (upgradeList[i] == _upgrade)
            {
                stack++;

                if(stack >= _maxStack)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

[Serializable]
public class CUpgradeReqirementCondition : CCondition
{
    [SerializeField] private SOUpgradeData _upgrade;
    [SerializeField] private int _reqireStack;

    public override bool CheckCondition()
    {
        IReadOnlyList<SOUpgradeData> upgradeList = CGameProgressManager.Instance.UpgradeList;

        int stack = 0;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            if (upgradeList[i] == _upgrade)
            {
                stack++;

                if (stack >= _reqireStack)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[Serializable]
public class CSkillUpgradeMaxStackCondition : CCondition
{
    [SerializeField] private SOSkillUpgradeData _upgrade;
    [SerializeField] private int _maxStack;

    public override bool CheckCondition()
    {
        IReadOnlyList<SOSkillUpgradeData> upgradeList = CGameProgressManager.Instance.SkillUpgradeList;

        int stack = 0;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            if (upgradeList[i] == _upgrade)
            {
                stack++;

                if(stack >= _maxStack)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

[Serializable]
public class CSkillUpgradeReqirementCondition : CCondition
{
    [SerializeField] private SOSkillUpgradeData _upgrade;
    [SerializeField] private int _reqireStack;

    public override bool CheckCondition()
    {
        IReadOnlyList<SOSkillUpgradeData> upgradeList = CGameProgressManager.Instance.SkillUpgradeList;

        int stack = 0;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            if (upgradeList[i] == _upgrade)
            {
                stack++;

                if (stack >= _reqireStack)
                {
                    return true;
                }
            }
        }

        return false;
    }
}