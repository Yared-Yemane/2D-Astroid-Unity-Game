using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundEffectHandler : MonoBehaviour
{

    Coroutine m_fadeCoroutine;
    [SerializeField] private List<AudioClip> m_generators;
    [SerializeField] private Vector2 m_minMaxPitch;
    [SerializeField] private List<AudioSource> m_audioSources;
    [SerializeField] private bool m_playOnAwake;

    private void OnEnable(){
        GameEvents.Instance.onGameOver += OnGameOver;
    }

    private void OnDisable(){
        GameEvents.Instance.onGameOver -= OnGameOver;
    }

    private void OnGameOver(){

        if(!m_playOnAwake){
            return;
        }

        StopAllCoroutines();
        foreach(AudioSource source in m_audioSources){
            source.volume = 0f;
        }
        // FadeOut();
    }

    private void Awake()
    {
        if(m_playOnAwake){
            Play();
        }
    }

    public void Play()
    {

        AudioSource source = m_audioSources[0];

        if(source.isPlaying && m_audioSources.Count > 1){
            source = m_audioSources[1];
        }

        int index = Random.Range(0, m_generators.Count);
        AudioClip clip = m_generators[index];
        source.generator = clip;
        source.pitch = Random.Range(m_minMaxPitch.x, m_minMaxPitch.y);
        source.Play();
    }

    public void FadeIn(){
        AudioSource source  = m_audioSources[0];
        if(m_fadeCoroutine != null){
            StopCoroutine(m_fadeCoroutine);
        }
        m_fadeCoroutine = StartCoroutine(FadeAudioSource(source, 1.0f));
    }

    public void FadeOut(){
        AudioSource source  = m_audioSources[0];
        if(m_fadeCoroutine != null){
            StopCoroutine(m_fadeCoroutine);
        }
        m_fadeCoroutine = StartCoroutine(FadeAudioSource(source, 0.0f));
    }

    private IEnumerator FadeAudioSource(AudioSource source, float target){
        while(source.volume != target){
            float currentVolume = source.volume;
            yield return new WaitForEndOfFrame();
            source.volume = Mathf.Lerp(currentVolume, target, Time.deltaTime);
        }
    }
}
