using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGameManager : CSingleton<CGameManager>
{
	#region inspector
	[SerializeField] private float _CMPperSec;
	[SerializeField] private SOPlayerData _playerData;
	[SerializeField] private SOPlayerData _enemyPlayerData;
	[SerializeField] private CEnemyAIManager _enemyAIManager;
	[SerializeField] private CSquadSpawner _spawner;
	[SerializeField] private CSkillCasterController _skillCaster;

	[Space]
	[SerializeField] private float _mapDifficultyMultiplyer;
	[SerializeField] private float _gameDifficultyMultiplyer = 1;

	[Space]
	[SerializeField] private CBattleUIManager _battleUIManager;
	//[SerializeField] private Camera _NoPostCam;

	[SerializeField] private CRewardPanelConroller _rewardPanel;
	#endregion

	#region private var
	private float _playerCMP = 200;
	private float _enemyCMP = 200f;
	private float _numHolder = 0;
	private Dictionary<SOPlayerData, List<CSquadContorller>> _squadListMap = new Dictionary<SOPlayerData, List<CSquadContorller>>();
	private int _reward = 1;
	#endregion

	#region property
	public SOPlayerData PlayerData => _playerData;
	public SOPlayerData EnemyPlayerData => _enemyPlayerData;
	public Dictionary<SOPlayerData, List<CSquadContorller>> SquadListMap => _squadListMap;
	public float PlayerCMP { get { return _playerCMP; } set { _playerCMP = value; _battleUIManager.UpdateText(_playerCMP); } }
	public CSquadSpawner Spawner => _spawner;
	public CSkillCasterController SkillCaster => _skillCaster;
	#endregion

	protected override void Awake()
	{

		if (_playerData == null || _enemyPlayerData == null)
		{
			Debug.LogWarning("playerData is null");
			enabled = false;
			return;
		}

		_squadListMap.Add(_playerData, new List<CSquadContorller>());
		_squadListMap.Add(_enemyPlayerData, new List<CSquadContorller>());

		//Cursor.lockState = CursorLockMode.Confined;
		//Time.timeScale = 2;

		base.Awake();
	}

	private void Start()
	{
		CGameProgressManager.Instance.SetUpgrade();
		_battleUIManager.InitializeButtons();
	}

	public void SetBattleSetting(float difficulty, SOPlayerData enemyPlayerData, int reward = 1)
	{
		_mapDifficultyMultiplyer = difficulty;
		_gameDifficultyMultiplyer = CGameProgressManager.Instance.GameDifficulty;
		_enemyPlayerData = enemyPlayerData;
		_reward = reward;
	}

	private void Update()
	{
		_numHolder += _CMPperSec * Time.deltaTime;
		_enemyCMP += _CMPperSec * _mapDifficultyMultiplyer * _gameDifficultyMultiplyer * Time.deltaTime;

		if (_numHolder >= 1)
		{
			float num = Mathf.Floor(_numHolder);

			_numHolder -= num;
			_playerCMP += num;

			_battleUIManager.UpdateText(_playerCMP);
		}
	}

	public bool SpendPlayerCMP(float amount)
	{
		if (_playerCMP < amount)
		{
			return false;
		}

		_playerCMP -= amount;
		_battleUIManager.UpdateText(_playerCMP);

		return true;
	}

	public bool SpendEnemyCMP(float amount)
	{
		if (_enemyCMP < amount)
		{
			return false;
		}

		_enemyCMP -= amount;
		return true;
	}


	public void GameOver(bool isVictory)
	{
		SetTimeScale(0);
		_enemyAIManager.enabled = false;
		//_NoPostCam.gameObject.SetActive(false);

		_battleUIManager.GameOver(isVictory);
		_rewardPanel.CreatRewardButton(_reward);

		if (!isVictory)
		{
			CGameProgressManager.Instance.ResetSave();
		}
	}

	public void SetTimeScale(float timeScale)
	{
		Time.timeScale = timeScale;
	}
}
