using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenu : BaseMenu
{
    public AudioMixer audioMixer;

    public Button backButton;


    public TMP_Text masterVolText;
    public Slider masterVolSlider;


    public TMP_Text musicVolText;
    public Slider musicVolSlider;


    public TMP_Text sfxVolText;
    public Slider sfxVolSlider;

    public override void Init(MenuController currentContext)
    {
        base.Init(currentContext);
        state = MenuStates.Audio;

        if (backButton) backButton.onClick.AddListener(() => JumpBack());

        if (masterVolSlider)
        {
            masterVolSlider.value = 0.5f;
            SetupSliderInformation(masterVolSlider, masterVolText, "MasterVol");
            OnSliderValueChanged(masterVolSlider.value, masterVolSlider, masterVolText, "MasterVol");
        }

        if (musicVolSlider)
        {
            musicVolSlider.value = 0.2f;
            SetupSliderInformation(musicVolSlider, musicVolText, "MusicVol");
            OnSliderValueChanged(musicVolSlider.value, musicVolSlider, musicVolText, "MusicVol");
        }

        if (sfxVolSlider)
        {
            sfxVolSlider.value = 1f;
            SetupSliderInformation(sfxVolSlider, sfxVolText, "SFXVol");
            OnSliderValueChanged(sfxVolSlider.value, sfxVolSlider, sfxVolText, "SFXVol");
        }


    }

    private void SetupSliderInformation(Slider slider, TMP_Text text, string parameterName)
    {
        slider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, slider, text, parameterName));
    }

    private void OnSliderValueChanged(float value, Slider slider, TMP_Text text, string parameterName)
    {
        if (value == 0)
        {
            value = -80;
            text.text = $"0%";
        }
        else
        {
            value = Mathf.Log10(value) * 20;
            text.text = $"{Mathf.RoundToInt(slider.value * 100)}%";
        }



        audioMixer.SetFloat(parameterName, value);

    }
}


