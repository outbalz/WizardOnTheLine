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
    }

    private void Update()
    {
		if (_isInitialized)
		{
			return;
		}

		if (_timer <= _data.TravelTime)
		{
			_data.Effect.ApplyEffect(_targetPos);
			KillProjectile();
			return;
		}
    }

	public void KillProjectile()
	{
		_isInitialized = false;
		CProjectilePool.Instance.ReturnToPool(this, _data);
	}
}
