using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CSoundManager : CSingletonDontDstroyAtLoad<CSoundManager>
{
    private Dictionary<AudioSource, CancellationTokenSource> _audioSourcesCTSMap = new Dictionary<AudioSource, CancellationTokenSource>();
    private List<CancellationTokenSource> _soundRelrayCTS = new List<CancellationTokenSource>();

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
        CancellationTokenSource cts = new CancellationTokenSource();
        Instance._soundRelrayCTS.Add(cts);
        Instance.SoundRelayUpdate(cts.Token, soundRelay, pos).Forget();
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
        await UniTask.WaitForSeconds(time, true, cancellationToken:ct);

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
        for (int i = 0; i < _soundRelrayCTS.Count; i++)
        {
            _soundRelrayCTS[i].Cancel();
            _soundRelrayCTS[i].Dispose();
            _soundRelrayCTS[i] = null;
        }

        foreach(CancellationTokenSource cts in _audioSourcesCTSMap.Values)
        {
            cts.Cancel();
        }

        base.OnDestroy();
    }
}

