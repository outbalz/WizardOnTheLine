using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO_", menuName = "Scriptable Objects/EntityDataSO")]
public class SOEntityData : ScriptableObject
{
    #region inspector
    [SerializeField] private CSkillEntityController _prefab;

    [Header("EffectAreaSize")]
    [SerializeField] private float _height;
    [SerializeField] private float _width;

    [Space]
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _effectTime;

    [SerializeField] private bool _isEffectContinuous;

    [Space]
    [SerializeReference, SubclassSelector] private CEntityEffect _effect;
    #endregion

    #region property
    public float Height => _height;
    public float Width => _width;
    public float LifeTime => _lifeTime;
    public float EffectTime => _effectTime;
    public bool IsEffectContinuous => _isEffectContinuous;
    public CEntityEffect Effect => _effect;
    #endregion

    public CSkillEntityController GetFromPool()
    {
        return CEntityPool.Instance.GetFromPool(_prefab);
    }

    public void ReturnToPool(CSkillEntityController entity)
    {
        CEntityPool.Instance.ReturnToPool(entity, _prefab);
    }
}

[System.Serializable]
public abstract class CEntityEffect
{
    public abstract void ApplyEffect(CSkillEntityController entity);

    public Collider2D[] GetEffectedCollider(CSkillEntityController entity)
    {

        Vector2 pointA = new Vector2(entity.transform.position.x - (entity.Width * 0.5f), entity.transform.position.y - (entity.Height * 0.5f));
        Vector2 pointB = new Vector2(entity.transform.position.x + (entity.Width * 0.5f), entity.transform.position.y + (entity.Height * 0.5f));


        return Physics2D.OverlapAreaAll(pointA, pointB);
    }
}

[System.Serializable]
public class CAreaDamageEntityEffect : CEntityEffect
{
    [SerializeField] private float _damage;

    public override void ApplyEffect(CSkillEntityController entity)
    {
        Collider2D[] colliders = GetEffectedCollider(entity);

        if (colliders.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
            {
                return;
            }

            ITargetable target = colliders[i].GetComponent<ITargetable>();

            if (target != null)
            {
                target.TakeDamage(_damage);
            }
        }
    }
}