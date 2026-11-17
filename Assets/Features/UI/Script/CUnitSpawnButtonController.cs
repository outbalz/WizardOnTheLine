using UnityEngine;

public class CUnitSpawnButtonController : MonoBehaviour
{
	#region inspector
	[SerializeField] private CSquadSpawner _spawner;
	[SerializeField] private SOPlayerData _playerData;
	[SerializeField] private SOSquadDataPreset _preset;
	#endregion

	public void OnButtonClicked()
	{
		if (!CGameManager.Instance.SpendPlayerCMP(_preset.Cost))
		{
			return;
		}

		_spawner.SpawnSquad(_preset, _playerData);
	}
}
