using UnityEngine;

public class CTargetDummy : MonoBehaviour, ITargetable
{
    public void TakeDamage(float damage)
    {
        Debug.Log(damage);
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }
}
