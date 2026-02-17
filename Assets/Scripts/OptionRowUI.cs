using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Slider slider;

    public void SetupToggle(string text, bool value, UnityEngine.Events.UnityAction<bool> callback)
    {
        label.text = text;
        slider.gameObject.SetActive(false);
        toggle.gameObject.SetActive(true);
        toggle.isOn = value;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(callback);
    }

    public void SetupSlider(string text, float value, UnityEngine.Events.UnityAction<float> callback)
    {
        label.text = text;
        toggle.gameObject.SetActive(false);
        slider.gameObject.SetActive(true);
        slider.value = value;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(callback);
    }
}
