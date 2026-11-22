using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CModifierHandler : MonoBehaviour
{
    #region private var
    private Dictionary<string, List<CModifier>> _modifierMap = new Dictionary<string, List<CModifier>>();
	private Dictionary<string, CAddtiveMultiflyModifier> _aditiveModifierMap = new Dictionary<string, CAddtiveMultiflyModifier>();
	#endregion

	public void ApplyModifierWithCheck(CModifier modifier)
	{
		if(modifier == null)
		{
			Debug.LogWarning("");
			return;
		}

		if (string.IsNullOrWhiteSpace(modifier.ModifierName))
		{
			return;
		}

        if (modifier.ModifierName.Contains("_Additive"))
		{
			ApplyAdditiveModifier(modifier);
			return;
		}

		ApplyModifier(modifier);
    }

	public void ApplyModifier(CModifier modifier)
	{
		if (_modifierMap.ContainsKey(modifier.ModifierName))
		{
			_modifierMap[modifier.ModifierName].Add(modifier);
		}

		else
		{
			_modifierMap.Add(modifier.ModifierName, new List<CModifier> {modifier});
		}

		modifier.ModifierHandler = this;

	}

	public void RemoveModifier(CModifier modifier)
	{
		if (!_modifierMap.ContainsKey(modifier.ModifierName))
		{
			return;
		}

		if (_modifierMap[modifier.ModifierName].Contains(modifier))
		{
			modifier.ModifierHandler = null;
			_modifierMap[modifier.ModifierName].Remove(modifier);
		}

		if(_modifierMap[modifier.ModifierName].Count <= 0)
		{
			_modifierMap[modifier.ModifierName] = null;
			_modifierMap.Remove(modifier.ModifierName);
		}
	}

	public void RemoveModifier(string modifierName)
	{
		if (!_modifierMap.ContainsKey(modifierName))
		{
			return;
		}
		
		_modifierMap[modifierName] = null;
		_modifierMap.Remove(modifierName);
	}

	public float GetModifiedValue(string modifierName, float baseValue)
	{
		if (!_modifierMap.ContainsKey(modifierName))
		{
			return baseValue;
		}

		List<CModifier> modifiers = _modifierMap[modifierName];
		float resultValue = baseValue;

		for (int i = 0; i < modifiers.Count; i++)
		{
			resultValue = modifiers[i].GetModifiedValue(resultValue);
		}

		return resultValue;
	}

	public void ApplyAdditiveModifier(CModifier modifier, string additiveModifierName)
	{
		if (_aditiveModifierMap.ContainsKey(additiveModifierName))
		{
			ApplyModifier(modifier);
		}

		else
		{
			CAddtiveMultiflyModifier additiveModifier = new CAddtiveMultiflyModifier(additiveModifierName, modifier.ModifierName, 1, this);
			_aditiveModifierMap.Add(additiveModifierName, additiveModifier);

			ApplyModifier(additiveModifier);
			ApplyModifier(modifier);
		}
	}

    public void ApplyAdditiveModifier(CModifier modifier)
    {
		if(!modifier.ModifierName.Contains("_Additive"))
		{
			Debug.LogWarning("Modifier name Err: Not Contain '_Additive' in nane");
			return;
		}

		string additiveModifierName = modifier.ModifierName.Replace("_Additive","");

        if (_aditiveModifierMap.ContainsKey(additiveModifierName))
        {
            ApplyModifier(modifier);
        }

        else
        {
            CAddtiveMultiflyModifier additiveModifier = new CAddtiveMultiflyModifier(additiveModifierName, modifier.ModifierName, 1, this);
            _aditiveModifierMap.Add(additiveModifierName, additiveModifier);

            ApplyModifier(additiveModifier);
            ApplyModifier(modifier);
        }
    }

	public void ClearModifier()
	{
		_modifierMap.Clear();
		_aditiveModifierMap.Clear();
	}

    private void OnDisable()
    {
		ClearModifier();
    }

    private void OnDestroy()
    {
        ClearModifier();
    }
}
