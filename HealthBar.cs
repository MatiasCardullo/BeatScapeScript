
using UnityEngine;
using UnityEngine.UI;

public class HealthBar
{
    public Slider slider;
    public Gradient gradient;
    public Image image;

    public void SetMaxHealth(ushort max)
    {
        slider.maxValue = max;
        image.color = gradient.Evaluate(1f);
    }
    public void SetHealth(ushort life)
    {
        slider.value = life;
        image.color = gradient.Evaluate(slider.normalizedValue);
    }
}
