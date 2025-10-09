using System.Collections.Generic;

namespace NiqonNO.UGUI
{
    public interface INODataCollection<T> : INODataCollection where T : INODataProvider
    {
        List<T> ItemData { get; }
        T GetDataAt(int index);
        INODataProvider INODataCollection.GetGenericDataAt(int index) => GetDataAt(index);
    }
}