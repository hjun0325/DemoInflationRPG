using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioDatabase audioDB;
    [SerializeField] private AudioMixer mainMixer;

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    private const string BGM_VOLUME_PARAM = "BGM_Volume";
    private const string SFX_VOLUME_PARAM = "SFX_Volume";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM과 SFX 전용 AudioSource 생성.
            bgmPlayer = gameObject.AddComponent<AudioSource>();
            bgmPlayer.volume = 1.0f;
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;

            sfxPlayer = gameObject.AddComponent<AudioSource>();
            sfxPlayer.volume = 1.0f;
            sfxPlayer.playOnAwake = false;
            sfxPlayer.loop = false;

            if (mainMixer != null)
            {
                // AudioSource의 출력을 믹서 그룹으로 설정
                bgmPlayer.outputAudioMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
                sfxPlayer.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        // PlayerPrefs에서 저장된 값을 불러옴. 값이 없으면 설정값으로 사용.
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_PARAM, -10.0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_PARAM, 0.0f);

        // 믹서에 볼륨 값을 즉시 적용
        mainMixer.SetFloat(BGM_VOLUME_PARAM, bgmVolume);
        mainMixer.SetFloat(SFX_VOLUME_PARAM, sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        float dbVolume = (volume <= -30f) ? -80f : volume;

        // 믹서에 공개된 "BGM_Volume" 파라미터의 값을 변경
        mainMixer.SetFloat(BGM_VOLUME_PARAM, dbVolume);
        // 변경된 값을 기기에 저장
        PlayerPrefs.SetFloat(BGM_VOLUME_PARAM, dbVolume);

        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        float dbVolume = (volume <= -20f) ? -80f : volume;
        Debug.Log(dbVolume);
        mainMixer.SetFloat(SFX_VOLUME_PARAM, dbVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_PARAM, dbVolume);

        PlayerPrefs.Save();
    }

    public void PlayBGM(string bgmName)
    {
        AudioClip clip = audioDB.GetBGM(bgmName);
        if (clip != null && bgmPlayer.clip != clip)
        {
            bgmPlayer.clip = clip;
            bgmPlayer.Play();
        }
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip clip = audioDB.GetSFX(sfxName);
        if (clip != null)
        {
            sfxPlayer.PlayOneShot(clip);
        }
    }
}
