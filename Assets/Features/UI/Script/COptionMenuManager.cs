using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class COptionMenuManager : CSingletonDontDstroyAtLoad<COptionMenuManager>
{
    #region inspector
    [SerializeField] private SOSceneData _tilteScene;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private TMP_Dropdown _resolutionDropDown;
    #endregion

    #region private var
    private bool _isOptitonMenuOpen = false;
    private float _previousTimescale = 1;
    private readonly string _ESCActionName = "ESC";
    private CInputActionData _ESCActionData;
    private bool _isBinded = false;
    #endregion

    #region property
    public float TimeScale { set { _previousTimescale = value; } }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (_optionMenu == null)
        {
            Debug.LogWarning("Missing Option Menu element");
        }

        if (_resolutionDropDown == null)
        {
            Debug.LogWarning("Missing DropDown element");
        }

        _previousTimescale = Time.timeScale;

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        Resolution[] resolutionOptions = Screen.resolutions;
        List<string> resolutionOptionStrings = new List<string>();

        int currentIndex = 0;
        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            resolutionOptionStrings.Add(resolutionOptions[i].width + "x" + resolutionOptions[i].height + "@" + resolutionOptions[i].refreshRateRatio.value);
            if (
                resolutionOptions[i].width == Screen.currentResolution.width &&
                resolutionOptions[i].height == Screen.currentResolution.height &&
                resolutionOptions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value
                )
            {
                currentIndex = i;
            }
        }

        _resolutionDropDown.ClearOptions();
        _resolutionDropDown.AddOptions(resolutionOptionStrings);
        _resolutionDropDown.SetValueWithoutNotify(currentIndex);
        _resolutionDropDown.RefreshShownValue();

        if (CInputDispatcher.Instance.InputActionMap.TryGetValue(_ESCActionName, out _ESCActionData))
        {
            BindAction();
        }

        else
        {
            Debug.LogError($"Mouse click action '{_ESCActionData}' not found in InputActionMap.");
        }

    }
    private void OnEnable()
    {
        BindAction();
    }

    private void OnDisable()
    {
        UnbindAction();
    }

    protected override void OnDestroy()
    {
        UnbindAction();
        base.OnDestroy();
    }

    private void BindAction()
    {
        if (_isBinded)
        {
            return;
        }

        if (_ESCActionData == null)
        {
            return;
        }

        _ESCActionData.OnStartedAction += ToggleOptitonMenu;

        _isBinded = true;
    }

    private void UnbindAction()
    {
        if (!_isBinded)
        {
            return;
        }

        if (_ESCActionData == null)
        {
            Debug.LogError("ESC action is not assigned.");
            return;
        }

        _ESCActionData.OnStartedAction -= ToggleOptitonMenu;

        _isBinded = false;
    }

    public void SetResolution(int index)
    {
        Resolution[] resolutionOptions = Screen.resolutions;
        Resolution targetResolution = resolutionOptions[index];

        Screen.SetResolution(targetResolution.width, targetResolution.height, Screen.fullScreenMode, targetResolution.refreshRateRatio);
    }

    public void SetFullscreenMode(int index)
    {

        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }

    }

    private void ToggleOptitonMenu()
    {
        _isOptitonMenuOpen = !_isOptitonMenuOpen;

        if (_isOptitonMenuOpen)
        {
            OpenOptitonMenu();
        }

        else
        {
            CloseOptionMenu();
        }

    }

    public void ExitGame()
    {
        //CSaveAndLoadManager.SaveGame();
        Application.Quit();
    }

    public void CloseOptionMenu()
    {
        _isOptitonMenuOpen = false;

        Time.timeScale = _previousTimescale;
        _optionMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OpenOptitonMenu()
    {
        _previousTimescale = Time.timeScale;
        Time.timeScale = 0;
        _optionMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadTileSecene()
    {
        CSeceneManager.Instance.LoadScene(_tilteScene);
    }

    public void CheatCode(string code)
    {
        switch (code)
        {
            case "show me the money":
                if (CGameManager.HasInstance)
                {
                    CGameManager.Instance.PlayerCMP += 10000;
                }
                break;
            case "power overwhelming":
                if (CGameManager.HasInstance)
                {
                    CGameManager.Instance.GameOver(true);
                }
                break;
            case "gatta go fast":
                _previousTimescale = _previousTimescale != 0 ? _previousTimescale *2 : 2;
                break;
            default:
                break;
        }

    }


    /*
    public void SetMasterVolume(float level)
    {
        level = Mathf.Log10(level) * 20f;
        CSoundManager.Instance.SetMasterVolume(level);
        CSoundManager.Instance.PlayCursorSound();
    }


    public void SetSFXVolume(float level)
    {
        level = Mathf.Log10(level) * 20f;
        CSoundManager.Instance.SetSFXVolume(level);
        CSoundManager.Instance.PlayCursorSound();
    }

    public void SetBGMVolume(float level)
    {
        level = Mathf.Log10(level) * 20f;
        CSoundManager.Instance.SetBGMVolume(level);
        CSoundManager.Instance.PlayCursorSound();
    }

    public void SetAmbienceVolume(float level)
    {
        level = Mathf.Log10(level) * 20f;
        CSoundManager.Instance.SetAmbienceVolume(level);
        CSoundManager.Instance.PlayCursorSound();
    }
    */

}