using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CGameStartManager : MonoBehaviour
{
    [System.Serializable]
    private struct SStartOption
    {
        [TextArea]
        public string StartDiecText;
        public SOUpgradeData[] StartUpgrade;
    }

    [SerializeField] private TMP_Dropdown _startSelectionDropdown;
    [SerializeField] private TMP_Dropdown _diffcultySelectionDropdown;
    [SerializeField] private TMP_Text _diecText;
    [SerializeField] private SStartOption[] _startOptions;
    [SerializeField] private float[] _difficultyModifiers;

    private int _selectedIndex;

    public void SelecteIndexIncrease()
    {
        _selectedIndex++;

        if (_selectedIndex >= _startOptions.Length)
        {
            _selectedIndex = 0;
        }

        _diecText.text = _startOptions[_selectedIndex].StartDiecText;

        _startSelectionDropdown.SetValueWithoutNotify(_selectedIndex);
    }

    public void SelecteIndexDecrease()
    {
        _selectedIndex--;

        if (_selectedIndex < 0)
        {
            _selectedIndex = _startOptions.Length-1;
        }

        _diecText.text = _startOptions[_selectedIndex].StartDiecText;

        _startSelectionDropdown.SetValueWithoutNotify(_selectedIndex);
    }

    public void OpenOptionMenu()
    {
        COptionMenuManager.Instance.OpenOptitonMenu();
    }

    public void GameStart()
    {
        CGameProgressManager.Instance.ResetSave();

        for (int i = 0; i < _startOptions[_selectedIndex].StartUpgrade.Length; i++)
        {
            CGameProgressManager.Instance.GetReward(_startOptions[_selectedIndex].StartUpgrade[i]);
        }

        if (_diffcultySelectionDropdown.value >= _difficultyModifiers.Length)
        {
            _diffcultySelectionDropdown.SetValueWithoutNotify(_difficultyModifiers.Length - 1);
        }

        CGameProgressManager.Instance.GameDifficulty = _difficultyModifiers[_diffcultySelectionDropdown.value];

        CSceneTransitionTriggerContoller.Instance.TriggerSceneTransiton();
    }
}
