using NiqonNO.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NiqonNO.UGUI
{
	public class NOTernaryController : MonoBehaviour
	{
		[SerializeField] [TextArea] private string LabelText;

		[FormerlySerializedAs("SliderVariable")] [SerializeField]
		private NOVector3Variable TernaryVariable;

		[SerializeField] [Range(0, 1)] private float StepSize = 0.1f;

		[Header("References")] [SerializeField] [Required]
		private NOTernarySlider Ternary;

		[SerializeField] private TMP_Text TitleLabel;

		[SerializeField] private TMP_Text ValueLabel;

		[SerializeField] private Button ButtonIncreaseX;

		[SerializeField] private Button ButtonIncreaseY;

		[SerializeField] private Button ButtonIncreaseZ;

		private void OnEnable()
		{
			Ternary.OnValueChanged.AddListener(UpdateValue);
			ButtonIncreaseX.onClick.AddListener(IncreaseX);
			ButtonIncreaseY.onClick.AddListener(DecreaseY);
			ButtonIncreaseZ.onClick.AddListener(DecreaseZ);
		}

		private void OnDisable()
		{
			Ternary.OnValueChanged.RemoveListener(UpdateValue);
			ButtonIncreaseX.onClick.RemoveListener(IncreaseX);
			ButtonIncreaseY.onClick.RemoveListener(DecreaseY);
			ButtonIncreaseZ.onClick.RemoveListener(DecreaseZ);
		}

		private void OnValidate()
		{
			if (Ternary)
			{
				Ternary.SetValueWithoutNotify(TernaryVariable.Value);
				UpdateValue(Ternary.Value);
			}

			if (TitleLabel) TitleLabel.text = LabelText;
		}

		private void UpdateValue(Vector3 newValue)
		{
			TernaryVariable.Value = newValue;
			if (ValueLabel) ValueLabel.text = newValue.ToString();
		}

		private void IncreaseX()
		{
			ValueStep(new Vector3(StepSize, 0, 0));
		}

		private void DecreaseY()
		{
			ValueStep(new Vector3(0, StepSize, 0));
		}

		private void DecreaseZ()
		{
			ValueStep(new Vector3(0, 0, StepSize));
		}

		private void ValueStep(Vector3 stepSize)
		{
			var relativeValue = stepSize * (Ternary.MaxValue - Ternary.MinValue);
			Ternary.Value += relativeValue;
		}
	}
}