using UnityEngine;

namespace NiqonNO.UGUI
{
	[AddComponentMenu("NiqonNO/UI/NOSliderMask")]
	public class NOSliderMask : NOMask
	{
		private static readonly int SliderCorners = Shader.PropertyToID("_SliderCorners");
		private static readonly int HandlePositions = Shader.PropertyToID("_HandlePositions");

		[SerializeField] private RectTransform ForegroundContainerRect;

		[SerializeField] private RectTransform HandleRect;

		[SerializeField] private Axis SlideAxis;

		public override void UpdateMaterialProperties()
		{
			if (MaskMaterial == null)
				return;

			var localSize = rectTransform.rect.size;
			var minEdge = Vector2.zero;
			var maxEdge = Vector2.zero;
			var handlePos = Vector2.zero;

			minEdge[(int)SlideAxis] = 1;
			maxEdge[(int)SlideAxis] = 1;

			if (ForegroundContainerRect != null)
			{
				minEdge *= transform.InverseTransformPoint(
					ForegroundContainerRect.TransformPoint(ForegroundContainerRect.rect.min));
				maxEdge *= transform.InverseTransformPoint(
					ForegroundContainerRect.TransformPoint(ForegroundContainerRect.rect.max));
			}

			if (HandleRect != null) handlePos = transform.InverseTransformPoint(HandleRect.position);

			MaskMaterial.SetVector(SliderCorners,
				new Vector4(minEdge.x, minEdge.y, maxEdge.x, maxEdge.y));
			MaskMaterial.SetVector(HandlePositions,
				new Vector4(handlePos.x, handlePos.y, localSize.x, localSize.y));
		}

		private enum Axis
		{
			Horizontal = 0,
			Vertical = 1
		}
	}
}