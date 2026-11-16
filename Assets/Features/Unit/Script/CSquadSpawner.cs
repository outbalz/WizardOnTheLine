using UnityEngine;

public class CSquadSpawner : MonoBehaviour
{
    #region inspector
    [SerializeField] private SOPlayerData _playerData;
    #endregion


    public void SpawnSquad(SOSquadDataPreset data, SOPlayerData playerData)
    {
        CSquadFactory.Instance.Creat(transform, data, playerData);
    }

    //for test====================
    #region debug
    [SerializeField] private SOSquadDataPreset _debugPrest;
    #endregion

    [ContextMenu("SpawnSquad")]
    private void SpawnTest()
    {
        SpawnSquad(_debugPrest, _playerData);
    }
    //=============================
}
