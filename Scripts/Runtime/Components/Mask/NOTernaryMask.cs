using UnityEngine;

namespace NiqonNO.UGUI
{
	[AddComponentMenu("NiqonNO/UI/NOTernaryMask")]
	public class NOTernaryMask : NOMask
	{
		private static readonly int SliderTopCorner = Shader.PropertyToID("_SliderTopCorner");
		private static readonly int SliderBottomCorners = Shader.PropertyToID("_SliderBottomCorners");
		private static readonly int HandlePositions = Shader.PropertyToID("_HandlePositions");

		[SerializeField] private RectTransform ForegroundContainerRect;

		[SerializeField] private RectTransform HandleRect;

		public override void UpdateMaterialProperties()
		{
			if (MaskMaterial == null)
				return;

			var localSize = rectTransform.rect.size;
			var topVert = Vector2.zero;
			var leftVert = Vector2.zero;
			var rightVert = Vector2.zero;
			var handlePos = Vector2.zero;

			if (ForegroundContainerRect != null)
			{
				var rectSize = ForegroundContainerRect.rect.size;
				var rectCenter = ForegroundContainerRect.rect.center;

				var width = rectSize.x;
				var height = width * Mathf.Sqrt(3) / 2;
				if (height > rectSize.y)
				{
					height = rectSize.y;
					width = 2 * height / Mathf.Sqrt(3);
				}

				var halfHeight = height / 2;
				var halfWidth = width / 2;

				leftVert = transform.InverseTransformPoint(
					ForegroundContainerRect.TransformPoint(rectCenter + new Vector2(-halfWidth, -halfHeight)));
				topVert = transform.InverseTransformPoint(
					ForegroundContainerRect.TransformPoint(rectCenter + new Vector2(0, halfHeight)));
				rightVert = transform.InverseTransformPoint(
					ForegroundContainerRect.TransformPoint(rectCenter + new Vector2(halfWidth, -halfHeight)));
			}

			if (HandleRect != null) handlePos = transform.InverseTransformPoint(HandleRect.position);

			MaskMaterial.SetVector(SliderTopCorner,
				new Vector4(topVert.x, topVert.y, 0, 0));
			MaskMaterial.SetVector(SliderBottomCorners,
				new Vector4(leftVert.x, leftVert.y, rightVert.x, rightVert.y));
			MaskMaterial.SetVector(HandlePositions,
				new Vector4(handlePos.x, handlePos.y, localSize.x, localSize.y));
		}
	}
}