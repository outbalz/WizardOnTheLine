using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class CInputActionData
{
    [SerializeField] private InputActionReference _reference;
    [SerializeField] private string _actionName;
    
    public InputActionReference Reference => _reference;
    public string ActionName => _actionName;
    public event Action<InputAction.CallbackContext> OnPerformedAction;
    public event Action<InputAction.CallbackContext> OnStartedAction;
    public event Action<InputAction.CallbackContext> OnCanceledAction;

    public virtual void OnStarted(InputAction.CallbackContext context)
    {
        OnStartedAction?.Invoke(context);
    }

    public virtual void OnPerformed(InputAction.CallbackContext context)
    {
        OnPerformedAction?.Invoke(context);
    }

    public virtual void OnCanceled(InputAction.CallbackContext context)
    {
        OnCanceledAction?.Invoke(context);
    }
}

[Serializable]
public class CInputVectorActionData : CInputActionData
{
    public Vector2 Value => Reference.action.ReadValue<Vector2>();

}

[Serializable]
public class CInputFloatActionData : CInputActionData
{
    public float Value => Reference.action.ReadValue<float>();
}



[DefaultExecutionOrder(-10)]
public class CInputDispatcher : CSingleton<CInputDispatcher>
{
    #region inspector
    [SerializeReference, SubclassSelector] private List<CInputActionData> _inputActions;
    #endregion

    #region private var
    private bool _isReady = false;
    #endregion

    #region poperty
    public readonly Dictionary<string, CInputActionData> InputActionMap = new Dictionary<string, CInputActionData>();
    #endregion

    protected override void Awake()
    {
        base.Awake();

        TryBindAll();
    }

    private void OnEnable()
    {
        TryBindAll();
        EnableActions(true);
        BakeInputMap();
    }

    private void OnDisable()
    {
        EnableActions(false);
        InputActionMap.Clear();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputActionMap.Clear();

        UnBindAll();
    }

    private void TryBind(CInputActionData data)
    {
        if(data.Reference == null)
        {
            Debug.LogWarning("액션 레퍼런스 누락");
            return;
        }

        data.Reference.action.started += data.OnStarted;
        data.Reference.action.performed += data.OnPerformed;
        data.Reference.action.canceled += data.OnCanceled;
    }



    private void UnBind(CInputActionData data)
    {
        if(data.Reference == null)
        {
            Debug.LogWarning("액션 레퍼런스 누락");
            return;
        }

        data.Reference.action.started -= data.OnStarted;
        data.Reference.action.performed -= data.OnPerformed;
        data.Reference.action.canceled -= data.OnCanceled;
    }

    private void TryBindAll()
    {
        if(_isReady)
        {
            return;
        }

        for (int i = 0; i < _inputActions.Count; i++)
        {
            if(_inputActions[i] == null)
            {
                continue;
            }

            TryBind(_inputActions[i]);
        }

        _isReady = true;
    }

    private void UnBindAll()
    {
        if(!_isReady)
        {
            return;
        }

        for (int i = 0; i < _inputActions.Count; i++)
        {
            if(_inputActions[i] == null)
            {
                continue;
            }

            UnBind(_inputActions[i]);
        }

        _isReady = false;
    }

    private void EnableActions(bool enable)
    {

        if (!_isReady)
        {
            return;
        }

        if (enable)
        {
            for (int i = 0; i < _inputActions.Count; i++)
            {
                if (_inputActions[i] == null)
                {
                    continue;
                }

                _inputActions[i].Reference.action.Enable();
            }
        }

        else
        {

            for (int i = 0; i < _inputActions.Count; i++)
            {
                if (_inputActions[i] == null)
                {
                    continue;
                }

                _inputActions[i].Reference.action.Disable();
            }
        }
    }

    private void BakeInputMap()
    {

        for (int i = 0; i < _inputActions.Count; i++)
        {

            if (_inputActions[i] == null)
            {
                continue;
            }

            CInputActionData action = _inputActions[i];

            if (InputActionMap.ContainsKey(action.ActionName))
            {
                continue;
            }

            InputActionMap.Add(action.ActionName, action);
        }
    }

    public void ToggleAction(string actionName, bool enable)
    {
        CInputActionData inputAction = InputActionMap[actionName];

        if (inputAction == null)
        {
            return;
        }

        if (enable)
        {
            inputAction.Reference.action.Enable();
        }

        else
        {
            inputAction.Reference.action.Disable();
        }
    }
}
