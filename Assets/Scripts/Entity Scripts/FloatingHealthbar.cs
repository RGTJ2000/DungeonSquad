using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{

    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateHealthbar(float currentValue, float maxValue)
    {
        _slider.value = Mathf.Clamp01(currentValue/maxValue);
    }
}
