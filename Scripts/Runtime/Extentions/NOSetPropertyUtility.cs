namespace UnityEngine.UI
{
	public static class NOSetPropertyUtility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			return SetPropertyUtility.SetColor(ref currentValue, newValue);
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			return SetPropertyUtility.SetStruct(ref currentValue, newValue);
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			return SetPropertyUtility.SetClass(ref currentValue, newValue);
		}
	}
}