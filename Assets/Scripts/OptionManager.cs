using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionManager : MonoBehaviour
{
    [Header("밝기 (검은 오버레이 패널)")]
    public Image brightnessOverlay;   // 화면 전체 덮는 검은 Image
    public Slider brightnessSlider;

    [Header("사운드")]
    public AudioMixer audioMixer;     // 없으면 AudioListener로 대체 가능
    public Slider volumeSlider;

    // 저장 키
    const string BRIGHTNESS_KEY = "option_brightness";
    const string VOLUME_KEY = "option_volume";

    void Start()
    {
        // 저장된 값 불러오기 (없으면 기본값)
        float brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        // 슬라이더 초기화
        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 시작 시 적용
        SetBrightness(brightness);
        SetVolume(volume);
    }

    // ---------------- 밝기 ----------------
    public void SetBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            // value 1 = 밝음(투명), value 0 = 어두움(불투명)
            Color c = brightnessOverlay.color;
            c.a = (1f - value) * 0.8f;  // 최대 80%까지만 어둡게
            brightnessOverlay.color = c;
        }
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
        PlayerPrefs.Save();
    }

    // ---------------- 사운드 ----------------
    public void SetVolume(float value)
    {
        if (audioMixer != null)
        {
            // 슬라이더(0~1)를 데시벨로 변환
            float db = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("MasterVolume", db);
        }
        else
        {
            // 믹서 없으면 전체 볼륨 직접 조절
            AudioListener.volume = value;
        }
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    // ---------------- 나중에 기능 추가하는 자리 ----------------
    // 예: public void SetFullscreen(bool on) { ... }
    // 예: public void SetLanguage(int index) { ... }
}