using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBarUI : MonoBehaviour
{
    [Header("Refs")]
	[SerializeField] private Slider _slider;
	[SerializeField] private Gradient _gradient;	
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameObject _fillBar;
    [SerializeField] private TextMeshProUGUI _amountText;

    Image fill;

    void Awake()
    {
        fill = _fillBar.GetComponent<Image>();
        _amountText.text = _slider.value + "/" + _slider.maxValue;
    }

    //set max value of slider
    public void SetMaxValue(float amount)
	{
		_slider.maxValue = amount;

        //if cur value is greater than new max, set it to new max
        if(_slider.value > _slider.maxValue)
		    _slider.value = _slider.maxValue;

		fill.color = _gradient.Evaluate(1f);
        _amountText.text = _slider.value.ToString("#.00") + "/" + _slider.maxValue.ToString("#.00");
	}

    //set value of slider
    public void SetValue(float health)
	{
		_slider.value = health;
        fill.color = _gradient.Evaluate(_slider.normalizedValue);
        _amountText.text = _slider.value.ToString("#.00") + "/" + _slider.maxValue.ToString("#.00");
    }
}
