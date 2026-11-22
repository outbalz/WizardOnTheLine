using System.Collections.Generic;
using UnityEngine;

public class CGameProgressManager : CSingletonDontDstroyAtLoad<CGameProgressManager>
{
    [SerializeField] private List<SOSquadDataPreset> _unlockSquadList = new List<SOSquadDataPreset>();
    private Dictionary<SOSquadDataPreset, List<CSquadData>> _squadMap = new Dictionary<SOSquadDataPreset, List<CSquadData>>();
}
