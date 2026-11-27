using UnityEngine;

[CreateAssetMenu(fileName = "BossNodeDataSO_", menuName = "Scriptable Objects/GameData/MapNodeData/BossNodeDataSO")]
public class SOBossNodeData : SONodeTypeData
{
    [SerializeField] private SOPlayerData _enemyPlayer;
    [SerializeField] private float _difficulty;
    [SerializeField] private int _rewardamount = 3;
    [SerializeField] private SOSceneData _sceneData;
    [SerializeField] private SOSceneData _endingSceneData;
    [SerializeField] private SOSquadDataPreset _bossSquadData;
    [SerializeField] private CSquadContorller _bossePrefab;

    public float Difficulty => _difficulty;
    public SOPlayerData EnemyPlayer => _enemyPlayer;

    public override void OnNodeClick()
    {
        if (_sceneData == null)
        {
            Debug.LogWarning("_sceneData is not assigned in the inspector.");
            return;
        }

        CSeceneManager.OnLoadComplete += () =>
        {
            CGameManager.Instance.SetBattleSetting(_difficulty, _enemyPlayer, _rewardamount);
            CGameManager.Instance.EnemyAIManager.Spawner.SpawnSquad(_bossSquadData, _enemyPlayer, _bossePrefab);
            CSceneTransitionTriggerContoller.Instance.SetTargetScene(_endingSceneData);
        };

        CSceneTransitionTriggerContoller.Instance.SetTargetScene(_sceneData);
        CSceneTransitionTriggerContoller.Instance.TriggerSceneTransiton();
    }
}