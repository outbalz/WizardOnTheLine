using System;
using System.Collections.Generic;
using UnityEngine;

public class CGameProgressManager : CSingletonDontDstroyAtLoad<CGameProgressManager>
{
    #region inspector
    [SerializeField] private List<SOSquadDataPreset> _unlockSquadList;
    [SerializeField] private List<SOUpgradeData> _upgradeList;
    [SerializeField] private List<SOSkillUpgradeData> _skillUpgradeList;
    #endregion

    #region private var
    private Dictionary<SOSquadDataPreset, List<CSquadData>> _squadMap = new Dictionary<SOSquadDataPreset, List<CSquadData>>();
    private SNodeIndex _mapNodeIndex = new SNodeIndex();
    private List<List<CNodeData>> _mapNodeData;
    private float _gameDifficulty = 1;
    #endregion

    #region property
    public IReadOnlyList<SOSquadDataPreset> UnlockSquadList => _unlockSquadList;
    public List<SOUpgradeData> UpgradeList => _upgradeList;
    public List<SOSkillUpgradeData> SkillUpgradeList => _skillUpgradeList;
    public IReadOnlyDictionary<SOSquadDataPreset, List<CSquadData>> SquadMap => _squadMap;
    public SNodeIndex MapNodeIndex { get { return _mapNodeIndex; } set { _mapNodeIndex = value; } }
    public List<List<CNodeData>> MapNodeData { get { return _mapNodeData; } set { _mapNodeData = value; } }
    public float GameDifficulty { get { return _gameDifficulty; } set { _gameDifficulty = value; } }
    #endregion

    private void Start()
    {
        _mapNodeIndex.Stage = 0;
        _mapNodeIndex.Layer = 0;
        _mapNodeIndex.Node = 0;
    }

    public void SetUpgrade()
    {
        _squadMap.Clear();

        for (int i = 0; i < _unlockSquadList.Count; i++)
        {
            CSquadData squadData = new CSquadData("All", _unlockSquadList[i]);
            _squadMap.Add(_unlockSquadList[i], new List<CSquadData> { squadData });
        }

        for (int i = 0; i < _upgradeList.Count; i++)
        {
            if (!_squadMap.ContainsKey(_upgradeList[i].Preset))
            {
                continue;
            }

            bool hasUpgrade = false;

            for (int j = 0; j < _squadMap[_upgradeList[i].Preset].Count; j++)
            {
                if (_squadMap[_upgradeList[i].Preset][j].ApplyUpgrade(_upgradeList[i]))
                {
                    hasUpgrade = true;
                }
            }

            if (!hasUpgrade)
            {
                CSquadData squadData = new CSquadData(_upgradeList[i].UpgradeTag, _upgradeList[i].Preset);
                squadData.ApplyUpgrade(_upgradeList[i]);
                _squadMap[_upgradeList[i].Preset].Add(squadData);
            }
        }

        CModifierHandler skillModifierHandler = CGameManager.Instance.SkillCaster.ModifierHandler;

        for (int i = 0; i < _skillUpgradeList.Count; i++)
        {
            for (int j = 0; j < _skillUpgradeList[i].Modifiers.Length; j++)
            {
                skillModifierHandler.ApplyModifierWithCheck(_skillUpgradeList[i].Modifiers[j]);
            }
        }
    }

    public void ResetSave()
    {
        //_unlockSquadList.Clear();
        _upgradeList.Clear();
        _skillUpgradeList.Clear();
        _squadMap.Clear();

        _mapNodeData = null;
        _mapNodeIndex.Stage = 0;
        _mapNodeIndex.Layer = 0;
        _mapNodeIndex.Node = 0;
    }

    public void GetReward(IRewardable reward)
    {

        if (reward is SOUpgradeData upgradeData)
        {
            _upgradeList.Add(upgradeData);

            if(upgradeData.UpgradeTag != "All")
            {
                for (int i = _upgradeList.Count -1; i >= 0 ; i--)
                {
                    SOUpgradeData upgrade = _upgradeList[i];

                    if(upgrade.UpgradeTag == "All")
                    {
                        _upgradeList.RemoveAt(i);
                        _upgradeList.Add(upgrade);
                    }
                }
            }
        }

        else if (reward is SOSkillUpgradeData skillUpgradeData)
        {
            _skillUpgradeList.Add(skillUpgradeData);
        }
    }
}
