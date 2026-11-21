using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDataSO_", menuName = "Scriptable Objects/GameData/ProjectileDataSO")]
public class SOProjectileData : SOGameData, IPoolabeSO<CProjectileController>
{
    #region inspector
    [SerializeField] private CProjectileController _prefab;
    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private float _heightMax;
    [SerializeField] private float _travelTime;
    [SerializeField] private bool _isFaceingTarget;
    [SerializeReference, SubclassSelector] private CProjectileEffect _effect;
    #endregion

    #region property
    public CProjectileController Prefab => _prefab;
    public AnimationCurve PositionCurve => _positionCurve;
    public AnimationCurve HeightCurve => _heightCurve;
    public float HeightMax => _heightMax;
    public float TravelTime => _travelTime;
    public bool IsFaceingTarget => _isFaceingTarget;
    public CProjectileEffect Effect => _effect;
    #endregion

    public Vector2 GetTravelPosition(float time, Vector2 originPos , Vector2 targetPos)
    {
        float t = time/TravelTime;

        float posT = _positionCurve.Evaluate(t);
        float heightT = _heightCurve.Evaluate(t);

        Vector2 pos = Vector2.Lerp(originPos, targetPos, posT);
        pos.y += heightT * _heightMax;

        return pos;
    }
}

[System.Serializable]
public abstract class CProjectileEffect
{
    public abstract void ApplyEffect(Vector2 targetPos);
}


[System.Serializable]
public abstract class CProjectileAreaEffect : CProjectileEffect
{
    [SerializeField] protected float _width;
    [SerializeField] protected float _height;
    
    public Collider2D[] GetEffectedCollider(Vector2 targetPos)
    {

        Vector2 pointA = new Vector2(targetPos.x - (_width * 0.5f), targetPos.y - (_height * 0.5f));
        Vector2 pointB = new Vector2(targetPos.x + (_width * 0.5f), targetPos.y + (_height * 0.5f));


        return Physics2D.OverlapAreaAll(pointA, pointB);
    }
}

[System.Serializable]
public class CProjectileAreaDamageEffect : CProjectileAreaEffect
{
    [SerializeField] private float _damage;
    [SerializeField] private AnimationCurve _damageFallOff;

    public override void ApplyEffect(Vector2 targetPos)
    {
        Collider2D[] colliders = GetEffectedCollider(targetPos);

        if (colliders.Length <= 0)
        {
            return;
        }

        Vector2 pointA = new Vector3(targetPos.x - (_width * 0.5f), targetPos.y - (_width * 0.5f));
        float maxDistance = (targetPos - pointA).magnitude;


        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
            {
                return;
            }

            float damage = _damage;

            if (_damageFallOff != null)
            {
                float distanceFormCanter = (colliders[i].transform.position - new Vector3(targetPos.x, targetPos.y, colliders[i].transform.position.z)).magnitude;

                float t = distanceFormCanter / maxDistance;

                damage = _damage * _damageFallOff.Evaluate(t);
            }

            ITargetable target = colliders[i].GetComponent<ITargetable>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}

[System.Serializable]
public class CProjectileSummonEntityEffect : CProjectileEffect
{
    [SerializeField] private SOEntityData _entityData;

    public override void ApplyEffect(Vector2 targetPos)
    {
        CSkillEntityController entity = CEntityPool.Instance.GetFromPool(_entityData);
        entity.transform.position = targetPos;
        entity.Initialized(null);
        entity.gameObject.SetActive(true);
    }
}
