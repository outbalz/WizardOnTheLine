using System.Collections.Generic;
using UnityEngine;

public interface IRewardable
{
    public string Name { get; }
    public string TargetName { get; }
    public string SubName { get; }
    public string Description { get; }
    public Sprite RewardSprite { get; }
    public CCondition[] UnlockCondition { get; }
}


[CreateAssetMenu(fileName = "SORewardPoolData", menuName = "Scriptable Objects/SORewardPoolData")]
public class SORewardPoolData : ScriptableObject
{
    [SerializeField] private List<CWeightedRandom<SOUpgradeData>.SRandomWeight> _upgradeRewardPool;

    public CWeightedRandom<IRewardable> GetRewardPool()
    {
        List<CWeightedRandom<IRewardable>.SRandomWeight> rewardPool = new List<CWeightedRandom<IRewardable>.SRandomWeight>();


        for (int i = 0; i < _upgradeRewardPool.Count; i++)
        {
            IRewardable reward = _upgradeRewardPool[i].Item;
            bool condition = true;

            for (int j = 0; j < reward.UnlockCondition.Length; j++)
            {
                if (!reward.UnlockCondition[j].CheckCondition())
                {
                    condition = false;
                    break;
                }
            }

            if (condition)
            {
                CWeightedRandom<IRewardable>.SRandomWeight weightedReward = new CWeightedRandom<IRewardable>.SRandomWeight();
                weightedReward.Item = reward;
                weightedReward.Weight = _upgradeRewardPool[i].Weight;

                rewardPool.Add(weightedReward);
            }
        }

        if (rewardPool.Count <= 0)
        {
            return null;
        }

        CWeightedRandom<IRewardable> checkedPool = new CWeightedRandom<IRewardable>();

        checkedPool.RandomWeightList = rewardPool;

        return checkedPool;
    }
}
