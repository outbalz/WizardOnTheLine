using System.Collections.Generic;
using UnityEngine;

public class CGameProgressManager : CSingletonDontDstroyAtLoad<CGameProgressManager>
{
    #region inspector
    [SerializeField] private List<SOSquadDataPreset> _unlockSquadList;
    [SerializeField] private List<SOUpgradeData> _upgradeList;
    #endregion

    #region private var
    private Dictionary<SOSquadDataPreset, List<CSquadData>> _squadMap = new Dictionary<SOSquadDataPreset, List<CSquadData>>();
    private SNodeIndex _mapNodeIndex = new SNodeIndex();
    private List<List<CNodeData>> _mapNodeData;
    #endregion

    #region property
    public IReadOnlyList<SOSquadDataPreset> UnlockSquadList => _unlockSquadList;
    public IReadOnlyDictionary<SOSquadDataPreset, List<CSquadData>> SquadMap => _squadMap;
    public SNodeIndex MapNodeIndex { get { return _mapNodeIndex; } set { _mapNodeIndex = value; } }
    public List<List<CNodeData>> MapNodeData { get { return _mapNodeData; } set { _mapNodeData = value; } }
    #endregion

    private void Start()
    {
        _mapNodeIndex.Stage = 0;
        _mapNodeIndex.Layer = 0;
        _mapNodeIndex.Node = 0;
    }

    public void SetPreset()
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
    }
}
