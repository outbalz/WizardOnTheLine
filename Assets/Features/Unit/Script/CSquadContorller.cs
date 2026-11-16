using System.Collections.Generic;
using UnityEngine;

public struct SOderContext
{
    public enum EOderType
    {
        Move,
        AttackMove,
        Stop,
        Retreat
    }

    public EOderType oderType;
    public Vector3 targetPos;
}
public class CSquadContorller : MonoBehaviour
{
    #region const
    private const float MAP_HIGHT_MAX = 0.6F;

    private const float MAP_HIGHT_MIN = -4.5F;
    #endregion

    #region inspector
    [SerializeField] private CSquadMarkerController _markerController;
    [SerializeField] private float _squadRadiusX;
	#endregion

	#region private var
    private SOPlayerData _playerData;
	private List<CUnitController> _squadMembers = new List<CUnitController>();
    #endregion

    #region property
    public SOPlayerData PlayerData => _playerData;
    public IReadOnlyList<CUnitController> SquadMembers => _squadMembers;
    #endregion

    private void Awake()
    {
        ExceptionCheck();
    }

    private void ExceptionCheck()
    {
        if( _markerController == null )
        {
            if(!TryGetComponent<CSquadMarkerController>(out _markerController))
            {
                Debug.LogWarning($"{this}/ _markerController is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
        }
    }

    public void InitializeSquad(SOSquadDataPreset squadDataPreset, SOPlayerData playerData)
    {
        if (squadDataPreset == null)
        {
            Debug.LogWarning("squadDataPreset is null.");
            return;
        }

        if (playerData == null)
        {
            Debug.LogWarning("playerData is null.");
            enabled = false;
            return;
        }

        _squadMembers.Clear();

        float spawnPointY = MAP_HIGHT_MAX - Random.Range(0f, 1.8f);

        for (int i = 0; i < squadDataPreset.SquadSize; i++)
        {
            CUnitController unit = CUnitFactory.Instance.Creat(this, squadDataPreset.UnitDatas[i]);

            float spawnPointX = unit.transform.position.x;
            spawnPointX += Random.Range(-_squadRadiusX, _squadRadiusX);

            unit.transform.position = new Vector3(spawnPointX, spawnPointY, unit.transform.position.z);

            _squadMembers.Add(unit);

            spawnPointY -= Random.Range(0.3f, 0.7f);
        }

        ExceptionCheck();
    }

    public bool GiveOder(SOderContext context)
    {

        switch (context.oderType)
        {
            case SOderContext.EOderType.Move:
                for (int i = 0; i < _squadMembers.Count; i++)
                {
                    float targtX = context.targetPos.x;
                    targtX += Random.Range(-_squadRadiusX, _squadRadiusX);

                    _squadMembers[i].Movement.GiveMoveOder(targtX);
                }

                return true;
            case SOderContext.EOderType.AttackMove:
                break;
            case SOderContext.EOderType.Stop:
                break;
            case SOderContext.EOderType.Retreat:
                break;
            default:
                break;
        }

        return false;
    }

    public void SelectSquad(bool isSelected)
    {
        for (int i = 0; i < _squadMembers.Count; i++)
        {
            _squadMembers[i].SelectUint(isSelected);
        }

        _markerController.SelectMarker(isSelected);
    }

}

