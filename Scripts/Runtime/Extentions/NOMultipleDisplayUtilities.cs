using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public static class NOMultipleDisplayUtilities
	{
		public static bool GetRelativeMousePositionForDrag(PointerEventData eventData, ref Vector2 position)
		{
			return MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position);
		}

		internal static Vector3 GetRelativeMousePositionForRaycast(PointerEventData eventData)
		{
			return MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
		}

		public static Vector3 RelativeMouseAtScaled(Vector2 position, int displayIndex)
		{
			return MultipleDisplayUtilities.RelativeMouseAtScaled(position, displayIndex);
		}
	}
}