using System;
using UnityEngine;
using UnityEngine.UI;

namespace NiqonNO.UGUI
{
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public abstract class NOMask : Mask
	{
		[NonSerialized] private Material BaseMaterial;

		[NonSerialized] private Material InstancedMaterial;

		[NonSerialized] protected Material MaskMaterial;

		protected override void OnRectTransformDimensionsChange()
		{
			UpdateMaterialProperties();
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			if (BaseMaterial != baseMaterial)
			{
				BaseMaterial = baseMaterial;
#if UNITY_EDITOR
				DestroyImmediate(InstancedMaterial);
#else
                Destroy(InstancedMaterial);
#endif
				InstancedMaterial = Instantiate(BaseMaterial);
			}

			MaskMaterial = base.GetModifiedMaterial(InstancedMaterial);
			UpdateMaterialProperties();
			return MaskMaterial;
		}

		public abstract void UpdateMaterialProperties();
	}
}