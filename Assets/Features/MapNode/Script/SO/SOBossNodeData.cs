using UnityEngine;

[CreateAssetMenu(fileName = "BossNodeDataSO_", menuName = "Scriptable Objects/GameData/MapNodeData/BossNodeDataSO")]
public class SOBossNodeData : SONodeTypeData
{
    [SerializeField] private SOPlayerData _enemyPlayer;
    [SerializeField] private float _difficulty;

    public float Difficulty => _difficulty;
    public SOPlayerData EnemyPlayer => _enemyPlayer;

    public override void OnNodeClick()
    {
        //TODO Load battle scene
    }
}