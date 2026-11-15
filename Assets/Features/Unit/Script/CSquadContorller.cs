using System.Collections.Generic;
using UnityEngine;

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

        float spawnPointY = MAP_HIGHT_MAX - Random.Range(0f, 1f);

        for (int i = 0; i < squadDataPreset.SquadSize; i++)
        {
            CUnitController unit = CUnitFactory.Instance.Creat(this, squadDataPreset.UnitDatas[i]);

            unit.transform.position = new Vector3(unit.transform.position.x, spawnPointY, unit.transform.position.z);

            _squadMembers.Add(unit);

            spawnPointY -= Random.Range(0.3f, 1f);
        }

        ExceptionCheck();
    }


}
