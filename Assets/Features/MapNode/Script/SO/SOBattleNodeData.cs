using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "BattleNodeDataSO_", menuName = "Scriptable Objects/GameData/MapNodeData/BattleNodeDataSO")]
public class SOBattleNodeData : SONodeTypeData
{
    [SerializeField] private SOPlayerData _enemyPlayer;
    [SerializeField] private float _difficulty;
    [SerializeField] private int _rewardamount = 1;
    [SerializeField] private SOSceneData _sceneData;

    public float Difficulty => _difficulty;
    public SOPlayerData EnemyPlayer => _enemyPlayer;

    public override void OnNodeClick()
    {
        if(_sceneData == null)
        {
            Debug.LogWarning("_sceneData is not assigned in the inspector.");
            return;
        }

        CSeceneManager.OnLoadComplete += () =>
        {
            CGameManager.Instance.SetBattleSetting(_difficulty, _enemyPlayer, _rewardamount);
        };

        CSceneTransitionTriggerContoller.Instance.SetTargetScene(_sceneData);
        CSceneTransitionTriggerContoller.Instance.TriggerSceneTransiton();
    }
}