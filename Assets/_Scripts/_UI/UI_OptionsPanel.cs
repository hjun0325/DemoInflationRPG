using UnityEngine;
using UnityEngine.UI;

public class UI_OptionsPanel : UI_Popup
{    
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button backButton;
    

    private void Start()
    {
        // PlayerPrefs에 저장된 볼륨 값을 불러와 슬라이더에 반영
        bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", -10f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 0f);

        // 슬라이더의 값이 변경될 때마다 SoundManager의 함수를 호출
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }

    public void OnClick_Close()
    {
        // PlayerPrefs에 최종 저장 (필수는 아님. Set...Volume에서 이미 저장됨)
        PlayerPrefs.Save();
        Hide();
    }
}
