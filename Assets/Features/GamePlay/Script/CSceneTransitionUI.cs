using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;

public class CSceneTransitionUI : MonoBehaviour
{

    #region Inspector
    [SerializeField] private CanvasGroup _transitionUI;
    [SerializeField] private float _defultFadeDuration = 0.25f;
    #endregion


    public void Initialize()
    {
        if (_transitionUI == null)
        {
            Debug.LogWarning("Missing _transitionUI");

            return;
        }

        _transitionUI.alpha = 0.0f;
        _transitionUI.blocksRaycasts = false;
        _transitionUI.interactable = false;

    }


    public async UniTask FadeTo(CancellationToken ct, float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (_transitionUI == null)
        {
            Debug.LogWarning("Missing _transitionUI");
            return;
        }

        if (duration < 0f)
        {
            duration = _defultFadeDuration;
        }

        if(ct.IsCancellationRequested)
        {
            return;
        }

        await FadeInternal(ct, targetAlpha, duration, blockRaycastWhileFading);
    }

    private async UniTask FadeInternal(CancellationToken ct, float targetAlpha, float duration, bool blockRaycastWhileFading)
    {
        float statAlpha = _transitionUI.alpha;

        _transitionUI.blocksRaycasts = blockRaycastWhileFading;

        _transitionUI.interactable = false;

        if (duration <= 0f)
        {
            _transitionUI.alpha = targetAlpha;

            _transitionUI.blocksRaycasts = (targetAlpha >= 0.99f);

            return;
        }

        float t = 0f;

        while (t < duration)
        {

            float dt = Time.unscaledDeltaTime;

            t += dt;

            float lerp = Mathf.Clamp01(t / duration);

            _transitionUI.alpha = Mathf.Lerp(statAlpha, targetAlpha, lerp);

            await UniTask.Yield();

        }

        _transitionUI.alpha = targetAlpha;

        _transitionUI.blocksRaycasts = (targetAlpha >= 0.99f);
    }

}