using UnityEngine;

namespace NiqonNO.UGUI
{
	internal static class NOBarycentricHelper
	{
		internal static readonly Vector2 LeftCorner = new(0.0f, 0.0f);
		internal static readonly Vector2 TopCorner = new(0.5f, 1.0f);
		internal static readonly Vector2 RightCorner = new(1.0f, 0.0f);


		internal static Vector3 BarycentricFromPosition(Vector2 position)
		{
			var v0 = LeftCorner - TopCorner;
			var v1 = RightCorner - TopCorner;
			var v2 = position - TopCorner;

			var d00 = Vector2.Dot(v0, v0);
			var d01 = Vector2.Dot(v0, v1);
			var d11 = Vector2.Dot(v1, v1);
			var d20 = Vector2.Dot(v2, v0);
			var d21 = Vector2.Dot(v2, v1);

			var denom = d00 * d11 - d01 * d01;
			var x = (d11 * d20 - d01 * d21) / denom;
			var z = (d00 * d21 - d01 * d20) / denom;
			var y = 1.0f - x - z;

			if (x < 0)
			{
				var t = Vector2.Dot(position - TopCorner, RightCorner - TopCorner) /
				        Vector2.Dot(RightCorner - TopCorner, RightCorner - TopCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(0.0f, 1.0f - t, t);
			}

			if (y < 0)
			{
				var t = Vector2.Dot(position - RightCorner, LeftCorner - RightCorner) /
				        Vector2.Dot(LeftCorner - RightCorner, LeftCorner - RightCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(t, 0.0f, 1.0f - t);
			}

			if (z < 0)
			{
				var t = Vector2.Dot(position - LeftCorner, TopCorner - LeftCorner) /
				        Vector2.Dot(TopCorner - LeftCorner, TopCorner - LeftCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(1.0f - t, t, 0.0f);
			}

			return new Vector3(x, y, z);
		}

		internal static Vector2 PositionFromBarycentric(Vector3 barycentric)
		{
			return barycentric.x * new Vector2(0.0f, 0.0f) +
			       barycentric.y * new Vector2(0.5f, 1.0f) +
			       barycentric.z * new Vector2(1.0f, 0.0f);
		}

		internal static Vector3 NormalizeBarycentric(Vector3 barycentric, float minValue, float maxValue)
		{
			return (barycentric - Vector3.one * minValue) / (maxValue - minValue);
		}

		internal static Vector3 DenormalizeBarycentric(Vector3 barycentric, float minValue, float maxValue)
		{
			return Vector3.one * minValue + barycentric * (maxValue - minValue);
		}

		internal static Vector3 ClampBarycentric(Vector3 barycentric, float minValue, float maxValue, BarycentricConstraint constraint)
		{
			return DenormalizeBarycentric(
				ClampBarycentric01(
					NormalizeBarycentric(barycentric, minValue, maxValue), 
					constraint), 
				minValue, maxValue);
		}

		internal static Vector3 ClampBarycentric01(Vector3 barycentric, BarycentricConstraint constraint)
		{

			barycentric.x = Mathf.Clamp01(barycentric.x);
			barycentric.y = Mathf.Clamp01(barycentric.y);
			barycentric.z = Mathf.Clamp01(barycentric.z);

			var currentSum = barycentric.x + barycentric.y + barycentric.z;
			return constraint switch
			{
				BarycentricConstraint.X => ClampConstrained(0),
				BarycentricConstraint.Y => ClampConstrained(1),
				BarycentricConstraint.Z => ClampConstrained(2),
				_ => currentSum == 0 ? Vector3.one / 3.0f : barycentric / currentSum
			};

			Vector3 ClampConstrained(int axis)
			{
				var axisNext = (int)Mathf.Repeat(axis + 1, 3);
				var axisPrev = (int)Mathf.Repeat(axis + 2, 3);

				if (currentSum == 0)
				{
					var halfDelta = (1 - barycentric[axis]) / 2.0f;
					barycentric[axisNext] = halfDelta;
					barycentric[axisPrev] = halfDelta;
					return barycentric;
				}

				var sumDelta = 1 - currentSum;
				var sum = barycentric[axisNext] + barycentric[axisPrev];
				var delta = sumDelta * (sum > 0 ? barycentric[axisNext] / sum : 0.5f);

				barycentric[axisNext] = Mathf.Clamp(barycentric[axisNext] + delta, 0, 1 - barycentric[axis]);
				barycentric[axisPrev] = 1 - barycentric[axis] - barycentric[axisNext];
				return barycentric;
			}
		}

		internal static Vector3 RoundBarycentric(Vector3 barycentric)
		{
			var floored = new Vector3Int(
				Mathf.FloorToInt(barycentric.x),
				Mathf.FloorToInt(barycentric.y),
				Mathf.FloorToInt(barycentric.z)
			);

			var targetSum = Mathf.RoundToInt(barycentric.x + barycentric.y + barycentric.z);
			var currentSum = floored.x + floored.y + floored.z;
			var delta = targetSum - currentSum;

			if (delta == 0)
				return floored;

			var fracX = barycentric.x - floored.x;
			var fracY = barycentric.y - floored.y;
			var fracZ = barycentric.z - floored.z;

			var minF = Mathf.Min(fracX, Mathf.Min(fracY, fracZ));
			var maxF = Mathf.Max(fracX, Mathf.Max(fracY, fracZ));
			var midF = fracX + fracY + fracZ - minF - maxF;

			var threshold = delta == 1 ? maxF : midF;
			if (fracX >= threshold)
			{
				floored.x += 1;
				delta--;
			}

			if (fracY >= threshold && delta > 0)
			{
				floored.y += 1;
				delta--;
			}

			if (fracZ >= threshold && delta > 0) floored.z += 1;
			return floored;
		}

		internal enum BarycentricConstraint
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 3
		}
	}
}