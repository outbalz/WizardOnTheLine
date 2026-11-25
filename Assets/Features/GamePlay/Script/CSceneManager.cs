using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CSeceneManager : CSingletonDontDstroyAtLoad<CSeceneManager>
{
    #region inspector
    [SerializeField] private CSceneTransitionUI _transitionUI;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private SOSceneData _currentScene;
    #endregion

    #region private var
    private bool _isLoading = false;
    private CancellationTokenSource _cts;
    #endregion

    #region property
    public static SOSceneData CurrentScene => Instance._currentScene;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        if (_loadingBar == null)
        {
            Debug.LogWarning("Missing _loadingbar");
        }
        /*
        */
        _isLoading = false;
    }

    private void Start()
    {
        if (_transitionUI != null)
        {
            _transitionUI.Initialize();
        }

        else
        {
            Debug.LogWarning("Transition UI is not assigned");
        }
    }

    public void LoadScene(SOSceneData sceneData, float fadeDuration = -1)
    {
        if (sceneData == null)
        {
            Debug.LogWarning("Scene data is null");
            return;
        }

        string sceneName = sceneData.Name;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is null or empty");
            return;
        }

        _currentScene = sceneData;

        if (_isLoading)
        {
            Debug.LogWarning("Scene is already loading");
            return;
        }

        if(_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _cts = new CancellationTokenSource();
        LoadSceneWithTransition(_cts.Token, sceneName, fadeDuration).Forget();

    }
    private async UniTask LoadSceneWithTransition(CancellationToken ct, string sceneName, float fadeDuration)
    {

        if (_isLoading)
        {
            Debug.LogWarning("Scene is already loading");
            return;
        }

        _isLoading = true;

        Debug.Log(sceneName);

        if (_transitionUI != null)
        {
            await _transitionUI.FadeTo(ct, 1f, fadeDuration);
        }

        _loadingBar.SetValueWithoutNotify(0);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            _loadingBar.SetValueWithoutNotify(op.progress);
            await UniTask.Yield();
        }

        _loadingBar.SetValueWithoutNotify(1);
        op.allowSceneActivation = true;

        await UniTask.Yield();

        if (_transitionUI != null)
        {
            await _transitionUI.FadeTo(ct, 0f, fadeDuration);
        }


        Debug.Log($"{sceneName}");

        _isLoading = false;


    }

}