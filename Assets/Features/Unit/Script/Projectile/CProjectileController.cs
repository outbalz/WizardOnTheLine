using UnityEngine;

public class CProjectileController : MonoBehaviour
{
	#region inspector
	[SerializeField] private SOProjectileData _data;
	#endregion

	#region private var
	private Vector2 _originPos;
	private Vector2 _targetPos;
	private float _timer;
    private bool _isInitialized;
    #endregion

    public void Initialized(Vector2 originPos, Vector2 targetPos)
    {
		_originPos = originPos;
		_targetPos = targetPos;
		_timer = 0;

		_isInitialized = true;

		if (_data.IsFaceingTarget)
		{
			Vector2 direction = targetPos - originPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void Update()
    {
		if (!_isInitialized)
		{
			return;
		}

		if (_timer >= _data.TravelTime)
		{
			_data.Effect.ApplyEffect(_targetPos);
			KillProjectile();
			return;
		}

		transform.position = _data.GetTravelPosition(_timer, _originPos, _targetPos);

		_timer += Time.deltaTime;
    }

	public void KillProjectile()
	{
		_isInitialized = false;
		CProjectilePool.Instance.ReturnToPool(this, _data);
	}
}
