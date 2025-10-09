using System;
using NiqonNO.Core.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NiqonNO.UGUI
{
	[AddComponentMenu("UI/NOTernarySlider")]
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class NOTernarySlider : NOSelectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		[SerializeField] private RectTransform _HandleRect;

		[SerializeField] private float _MinValue;

		[SerializeField] private float _MaxValue;

		[SerializeField] private bool _WholeNumbers;

		[SerializeField] private Vector3 _Value;

		[Space] [SerializeField] private TernaryEvent _OnValueChanged = new();

		private bool DelayedUpdateVisuals;
		private RectTransform HandleContainerRect;

		private Transform HandleTransform;

		private Vector2 Offset = Vector2.zero;

#pragma warning disable 649
		private DrivenRectTransformTracker Tracker;
#pragma warning restore 649
		public RectTransform HandleRect
		{
			get => _HandleRect;
			set
			{
				if (!NOSetPropertyUtility.SetClass(ref _HandleRect, value)) return;
				UpdateCachedReferences();
				UpdateVisuals();
			}
		}

		public float MinValue
		{
			get => _MinValue;
			set
			{
				if (!NOSetPropertyUtility.SetStruct(ref _MinValue, value)) return;
				Set(_Value);
				UpdateVisuals();
			}
		}

		public float MaxValue
		{
			get => _MaxValue;
			set
			{
				if (!NOSetPropertyUtility.SetStruct(ref _MinValue, value)) return;
				Set(_Value);
				UpdateVisuals();
			}
		}

		public bool WholeNumbers
		{
			get => _WholeNumbers;
			set
			{
				if (!NOSetPropertyUtility.SetStruct(ref _WholeNumbers, value)) return;
				Set(_Value);
				UpdateVisuals();
			}
		}

		public virtual Vector3 Value
		{
			get => WholeNumbers ? NOBarycentricHelper.RoundBarycentric(_Value) : _Value;
			set => Set(value);
		}

		public virtual Vector3 NormalizedValue
		{
			get => Mathf.Approximately(MinValue, MaxValue)
				? Vector3.one / 3f
				: NOBarycentricHelper.NormalizeBarycentric(Value, MinValue, MaxValue);
			set => Value = NOBarycentricHelper.DenormalizeBarycentric(value, MinValue, MaxValue);
		}

		public TernaryEvent OnValueChanged
		{
			get => _OnValueChanged;
			set => _OnValueChanged = value;
		}

		private float StepSize => WholeNumbers ? 1 : (MaxValue - MinValue) * 0.1f;

		protected virtual void Update()
		{
			if (DelayedUpdateVisuals)
			{
				DelayedUpdateVisuals = false;
				Set(_Value, false);
				UpdateVisuals();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateCachedReferences();
			Set(_Value, false);
			UpdateVisuals();
		}

		protected override void OnDisable()
		{
			Tracker.Clear();
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			_Value = ClampValue(_Value);
			var oldNormalizedValue = NormalizedValue;
			if (HandleContainerRect != null)
				oldNormalizedValue = NOBarycentricHelper.BarycentricFromPosition(HandleRect.anchoredPosition);

			UpdateVisuals();

			if (oldNormalizedValue != NormalizedValue)
			{
				UISystemProfilerApi.AddMarker("Ternary.value", this);
				OnValueChanged.Invoke(_Value);
			}

			base.OnDidApplyAnimationProperties();
		}

		private void OnDrawGizmosSelected()
		{
			var rectSize = HandleContainerRect.rect.size;

			Vector3 v0, v1, v2, v01, v12, v20;

			v0 = HandleContainerRect.TransformPoint(NOBarycentricHelper.LeftCorner * rectSize);
			v1 = HandleContainerRect.TransformPoint(NOBarycentricHelper.TopCorner * rectSize);
			v2 = HandleContainerRect.TransformPoint(NOBarycentricHelper.RightCorner * rectSize);

			v01 = (v0 + v1) / 2;
			v12 = (v1 + v2) / 2;
			v20 = (v2 + v0) / 2;

			Gizmos.color = Color.red;
			Gizmos.DrawLine(v1, v2);
			Gizmos.DrawLine(v0, v12);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(v2, v0);
			Gizmos.DrawLine(v1, v20);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(v0, v1);
			Gizmos.DrawLine(v2, v01);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			if (!IsActive())
				return;

			UpdateVisuals();
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (WholeNumbers)
			{
				_MinValue = Mathf.Round(_MinValue);
				_MaxValue = Mathf.Round(_MaxValue);
			}

			if (IsActive())
			{
				UpdateCachedReferences();
				DelayedUpdateVisuals = true;
			}

			if (!PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}
#endif

		public virtual void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout)
				OnValueChanged.Invoke(Value);
#endif
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;
			UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		public virtual void SetValueWithoutNotify(Vector3 input)
		{
			Set(input, false);
		}

		protected virtual void UpdateCachedReferences()
		{
			if (_HandleRect && _HandleRect != (RectTransform)transform)
			{
				HandleTransform = _HandleRect.transform;
				if (HandleTransform.parent != null)
					HandleContainerRect = HandleTransform.parent.GetComponent<RectTransform>();
			}
			else
			{
				_HandleRect = null;
				HandleContainerRect = null;
			}
		}

		private Vector3 ClampValue(Vector3 input)
		{
			var diff = (_Value - input).Abs();
			var constraint =
				Mathf.Approximately(diff.y + diff.z, Mathf.Epsilon) ? NOBarycentricHelper.BarycentricConstraint.X :
				Mathf.Approximately(diff.x + diff.z, Mathf.Epsilon) ? NOBarycentricHelper.BarycentricConstraint.Y :
				Mathf.Approximately(diff.x + diff.y, Mathf.Epsilon) ? NOBarycentricHelper.BarycentricConstraint.Z :
				NOBarycentricHelper.BarycentricConstraint.None;
			var newValue = NOBarycentricHelper.ClampBarycentric(input, MinValue, MaxValue, constraint);
			if (WholeNumbers)
				newValue = NOBarycentricHelper.RoundBarycentric(newValue);
			return newValue;
		}

		protected virtual void Set(Vector3 input, bool sendCallback = true)
		{
			var newValue = ClampValue(input);

			if (_Value == newValue)
				return;

			_Value = newValue;
			UpdateVisuals();
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("Ternary.value", this);
				_OnValueChanged.Invoke(newValue);
			}
		}

		protected void UpdateVisuals()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				UpdateCachedReferences();
#endif

			Tracker.Clear();

			UpdateHandle();
		}

		protected virtual void UpdateHandle()
		{
			if (HandleContainerRect == null) return;

			SetHandleAnchorAndPosition(_HandleRect, NormalizedValue);
		}

		protected virtual void SetHandleAnchorAndPosition(RectTransform rectTransform, Vector3 normalizedValue)
		{
			Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors);

			var anchorMin = Vector2.zero;
			var anchorMax = Vector2.one;
			anchorMin =
				anchorMax = NOBarycentricHelper.PositionFromBarycentric(normalizedValue);

			rectTransform.anchorMax = anchorMax;
			rectTransform.anchorMin = anchorMin;
		}

		protected virtual void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			var clickRect = HandleContainerRect;
			if (clickRect != null && clickRect.rect.size.magnitude > 0)
			{
				var position = Vector2.zero;
				if (!NOMultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
					return;

				Vector2 localCursor;
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
					return;
				localCursor -= clickRect.rect.position;

				var val = NOBarycentricHelper.BarycentricFromPosition((localCursor - Offset) / clickRect.rect.size);
				NormalizedValue = val;
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			base.OnPointerDown(eventData);

			Offset = Vector2.zero;
			if (HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(_HandleRect,
				    eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
			{
				Vector2 localMousePos;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_HandleRect,
					    eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
					Offset = localMousePos;
			}
			else
			{
				UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				base.OnMove(eventData);
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left when FindSelectableOnLeft() == null:
					Set(Value + Vector3.right * StepSize);
					break;
				case MoveDirection.Right when FindSelectableOnRight() == null:
					Set(Value + Vector3.forward * StepSize);
					break;
				case MoveDirection.Up when FindSelectableOnUp() == null:
					Set(Value + Vector3.up * StepSize);
					break;
				case MoveDirection.Down when FindSelectableOnDown() == null:
					Set(Value - Vector3.down * StepSize);
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			return base.FindSelectableOnLeft();
		}

		public override Selectable FindSelectableOnRight()
		{
			return base.FindSelectableOnRight();
		}

		public override Selectable FindSelectableOnUp()
		{
			return base.FindSelectableOnUp();
		}

		public override Selectable FindSelectableOnDown()
		{
			return base.FindSelectableOnDown();
		}

		[Serializable]
		public class TernaryEvent : UnityEvent<Vector3> { }
	}
}