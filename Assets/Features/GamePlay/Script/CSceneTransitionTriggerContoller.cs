using UnityEngine;
using UnityEngine.Events;

public class CSceneTransitionTriggerContoller : CSingleton<CSceneTransitionTriggerContoller>
{
    [SerializeField] private SOSceneData _sceneData;
    [SerializeField] private UnityEvent _sceneCloseEvent;

    public void SetTargetScene(SOSceneData sceneData)
    {
        _sceneData = sceneData;
    }

    public void TriggerSceneTransiton(float fadeDuration = -1)
    {
        if(_sceneData == null)
        {
            return;
        }

        _sceneCloseEvent?.Invoke();

        CSeceneManager.Instance.LoadScene(_sceneData, fadeDuration);
    }
}
