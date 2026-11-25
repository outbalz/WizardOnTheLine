using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CSoundManager : CSingletonDontDstroyAtLoad<CSoundManager>
{
    private Dictionary<AudioSource, CancellationTokenSource> _audioSourcesCTSMap = new Dictionary<AudioSource, CancellationTokenSource>();
    private Queue<CancellationTokenSource> _soundRelrayCTS = new Queue<CancellationTokenSource>();
    private Dictionary<SOSoundRelay,float> _soundPlayTimeMap = new Dictionary<SOSoundRelay,float>();

    public static void PlaySound(AudioClip clip, Vector3 pos)
    {
        AudioSource audioSource = CAudioSourcePool.Instance.GetFromPool();

        audioSource.clip = clip;
        audioSource.transform.position = pos;

        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        CancellationTokenSource cts = new CancellationTokenSource();
        Instance._audioSourcesCTSMap.Add(audioSource, cts);
        Instance.ReturnAudioSourceToPool(cts.Token, audioSource.clip.length, audioSource).Forget();
    }

    public static void PlaySound(AudioClip clip, Vector3 pos, Transform tr)
    {
        AudioSource audioSource = CAudioSourcePool.Instance.GetFromPool();

        audioSource.clip = clip;
        audioSource.transform.position = pos;
        audioSource.transform.SetParent(tr);

        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        CancellationTokenSource cts = new CancellationTokenSource();
        Instance._audioSourcesCTSMap.Add(audioSource, cts);
        Instance.ReturnAudioSourceToPool(cts.Token, audioSource.clip.length, audioSource).Forget();
    }

    public static void PlaySoundRelay(SOSoundRelay soundRelay, Vector3 pos)
    {
        if (Instance._soundPlayTimeMap.ContainsKey(soundRelay))
        {
            if(Time.time - Instance._soundPlayTimeMap[soundRelay] < 0.05f)
            {
                return;
            }
        }

        if(Instance._soundRelrayCTS.Count >= 50)
        {
            CancellationTokenSource cancelCts = Instance._soundRelrayCTS.Dequeue();
            cancelCts.Cancel();
            cancelCts.Dispose();
            cancelCts = null;
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        Instance._soundRelrayCTS.Enqueue(cts);
        Instance.SoundRelayUpdate(cts.Token, soundRelay, pos).Forget();

        if (Instance._soundPlayTimeMap.ContainsKey(soundRelay))
        {
            Instance._soundPlayTimeMap[soundRelay] = Time.time;
        }

        else
        {
            Instance._soundPlayTimeMap.Add(soundRelay, Time.time);
        }
    }

    private async UniTask SoundRelayUpdate(CancellationToken ct, SOSoundRelay soundRelay, Vector3 pos)
    {
        for (int i = 0; i < soundRelay.Audio.Count; i++)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            await UniTask.WaitForSeconds(soundRelay.Audio[i].Delay, cancellationToken: ct);

            AudioSource audioSource = CAudioSourcePool.Instance.GetFromPool();

            audioSource.clip = soundRelay.Audio[i].Clip;
            audioSource.transform.position = pos;
            audioSource.volume = soundRelay.Audio[i].Volume;
            audioSource.pitch = Random.Range(soundRelay.Audio[i].PitchMin, soundRelay.Audio[i].PitchMax);

            audioSource.gameObject.SetActive(true);
            audioSource.Play();

            CancellationTokenSource cts = new CancellationTokenSource();
            _audioSourcesCTSMap.Add(audioSource, cts);
            ReturnAudioSourceToPool(cts.Token, audioSource.clip.length, audioSource).Forget();
        }
    }

    private async UniTask ReturnAudioSourceToPool(CancellationToken ct, float time, AudioSource audioSource)
    {
        await UniTask.WaitForSeconds(time, false, cancellationToken:ct);

        if (ct.IsCancellationRequested || audioSource == null)
        {
            return;
        }

        audioSource.Stop();
        CAudioSourcePool.Instance.ReturnToPool(audioSource);

        _audioSourcesCTSMap[audioSource].Cancel();
        _audioSourcesCTSMap[audioSource].Dispose();
        _audioSourcesCTSMap[audioSource] = null;

        _audioSourcesCTSMap.Remove(audioSource);
    }

    public static void StopSound(AudioSource audioSource)
    {
        Instance._audioSourcesCTSMap[audioSource].Cancel();
    }

    protected override void OnDestroy()
    {

        CancelAll();
        base.OnDestroy();
    }

    public void CancelAll()
    {
        for (int i = 0; i < _soundRelrayCTS.Count; i++)
        {
            CancellationTokenSource cts = _soundRelrayCTS.Dequeue();

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        foreach(CancellationTokenSource cts in _audioSourcesCTSMap.Values)
        {
            cts.Cancel();
        }
    }
}

