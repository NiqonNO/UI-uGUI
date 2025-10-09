using NiqonNO.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NiqonNO.UGUI
{
	public class NOSliderController : MonoBehaviour
	{
		[SerializeField] [TextArea] private string LabelText;

		[SerializeField] private NOFloatVariable SliderVariable;

		[SerializeField] [Range(0, 1)] private float StepSize = 0.1f;

		[Header("References")] [SerializeField] [Required]
		private NOSlider Slider;

		[SerializeField] private TMP_Text TitleLabel;

		[SerializeField] private TMP_Text ValueLabel;

		[SerializeField] private Button ButtonIncrease;

		[SerializeField] private Button ButtonDecrease;

		private void OnEnable()
		{
			Slider.OnValueChanged.AddListener(UpdateValue);
			ButtonIncrease.onClick.AddListener(IncreaseValue);
			ButtonDecrease.onClick.AddListener(DecreaseValue);
		}

		private void OnDisable()
		{
			Slider.OnValueChanged.RemoveListener(UpdateValue);
			ButtonIncrease.onClick.RemoveListener(IncreaseValue);
			ButtonDecrease.onClick.RemoveListener(DecreaseValue);
		}

		private void OnValidate()
		{
			if (Slider)
			{
				Slider.SetValueWithoutNotify(SliderVariable.Value);
				UpdateValue(Slider.Value);
			}

			if (TitleLabel) TitleLabel.text = LabelText;
		}

		private void UpdateValue(float newValue)
		{
			SliderVariable.Value = newValue;
			if (ValueLabel) ValueLabel.text = newValue.ToString();
		}

		private void IncreaseValue()
		{
			ValueStep(StepSize);
		}

		private void DecreaseValue()
		{
			ValueStep(-StepSize);
		}

		private void ValueStep(float stepSize)
		{
			var relativeValue = (Slider.MaxValue - Slider.MinValue) * stepSize;
			Slider.Value += relativeValue;
		}
	}
}