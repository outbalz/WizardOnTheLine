using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOGameData : ScriptableObject
{
    #region inspector
    [SerializeField] protected string _id = "id";
	[SerializeField] protected string _name = "name";
    #endregion

    #region property
    public string ID => _id;
	public virtual string Name => _name;
    #endregion

    protected virtual void OnValidate()
    {
		if (string.IsNullOrWhiteSpace(_id))
		{
			Debug.LogWarning($"{this}/ {_id} :invalid _id");
		}

		if (string.IsNullOrWhiteSpace(_name))
		{
			Debug.LogWarning($"{this}/ {_name} :invalid _name");
		}
    }
}
