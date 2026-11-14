using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSingletonDontDstroyAtLoad<T> : CSingleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }
}
