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
    private const float MAP_HIGHT_MAX = 0.35F;
    private const float MAP_HIGHT_MIN = -4.5F;
    #endregion

    #region inspector
    [SerializeField] private CSquadMarkerController _markerController;
	#endregion

	#region private var
	private List<CUnitController> _squadMembers = new List<CUnitController>();
    #endregion

    #region property
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

    public void InitializeSquad(SOSquadDataPreset squadDataPreset)
    {
        if (squadDataPreset == null)
        {
            Debug.LogWarning("squadDataPreset is null.");
            return;
        }

        _squadMembers.Clear();

        float spawnPointY = MAP_HIGHT_MAX - Random.Range(0f, 2f);

        for (int i = 0; i < squadDataPreset.SquadSize; i++)
        {
            CUnitController unit = CUnitFactory.Instance.Creat(this, squadDataPreset.UnitDatas[i]);

            unit.transform.position = new Vector3(unit.transform.position.x, spawnPointY, unit.transform.position.z);

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
                    _squadMembers[i].Movement.GiveMoveOder(context.targetPos.x);
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


}

