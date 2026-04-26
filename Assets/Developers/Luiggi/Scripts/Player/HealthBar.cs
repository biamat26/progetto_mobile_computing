using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(int max)
    {
        slider.maxValue = max;
        slider.value = max;
        Canvas.ForceUpdateCanvases();
    }

    public void SetHealth(int hp)
    {
        slider.value = Mathf.Clamp(hp, 0, (int)slider.maxValue);
        Canvas.ForceUpdateCanvases();
    }
}