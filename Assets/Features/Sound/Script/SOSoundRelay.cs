using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CAudioSetting
{
    public AudioClip Clip;
    public float Delay;
    public float Volume;
    public float PitchMin;
    public float PitchMax;
}

[CreateAssetMenu(fileName = "SoundRelaySO_", menuName = "Scriptable Objects/GameData/SoundRelaySO_")]
public class SOSoundRelay : SOGameData
{
    [SerializeField] private List<CAudioSetting> _audio;
    
    public IReadOnlyList<CAudioSetting> Audio => _audio;
}
