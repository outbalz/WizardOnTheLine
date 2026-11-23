using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO_", menuName = "Scriptable Objects/GameData/EntityDataSO")]
public class SOEntityData : SOGameData, IPoolabeSO<CSkillEntityController>
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
    [SerializeField] private SOSoundRelay _sound;

    [Space]
    [SerializeReference, SubclassSelector] private CEntityEffect _effect;
    #endregion

    #region property
    public float Height => _height;
    public float Width => _width;
    public float LifeTime => _lifeTime;
    public float EffectTime => _effectTime;
    public bool IsEffectContinuous => _isEffectContinuous;
    public SOSoundRelay Sound => _sound;
    public CEntityEffect Effect => _effect;
    public CSkillEntityController Prefab => _prefab;
    #endregion

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
    [SerializeField] private AnimationCurve _damageFallOff;
    [SerializeField] private float _cameraShakeStrengh;
    [SerializeField] private float _cameraShakeDuration;

    public override void ApplyEffect(CSkillEntityController entity)
    {
        Collider2D[] colliders = GetEffectedCollider(entity);

        if (colliders.Length <= 0)
        {
            return;
        }

        if (_cameraShakeDuration > 0)
        {
            CCameraContoller.ShakeCamera(_cameraShakeStrengh, _cameraShakeDuration);
        }

        Vector3 pointA = new Vector3(entity.transform.position.x - (entity.Width * 0.5f), entity.transform.position.y - (entity.Height * 0.5f), entity.transform.position.z);
        float maxDistance = (entity.transform.position - pointA).magnitude;


        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
            {
                return;
            }

            float damage = _damage;

            if (_damageFallOff != null)
            {
                float distanceFormCanter = (colliders[i].transform.position - entity.transform.position).magnitude;

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