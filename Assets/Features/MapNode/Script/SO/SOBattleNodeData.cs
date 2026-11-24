using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "BattleNodeDataSO_", menuName = "Scriptable Objects/GameData/MapNodeData/BattleNodeDataSO")]
public class SOBattleNodeData : SONodeTypeData
{
    [SerializeField] private SOPlayerData _enemyPlayer;
    [SerializeField] private float _difficulty;

    public float Difficulty => _difficulty;
    public SOPlayerData EnemyPlayer => _enemyPlayer;

    public override void OnNodeClick()
    {
        //TODO Load battle scene

        //For test
        SceneManager.LoadScene(0);
    }
}