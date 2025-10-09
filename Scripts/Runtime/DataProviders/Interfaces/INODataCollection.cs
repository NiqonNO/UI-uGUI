using System.Collections.Generic;

namespace NiqonNO.UGUI
{
    public interface INODataCollection
    {
        int Count { get; }
        INODataProvider GetGenericDataAt(int index);
        void SelectDataItem(int index);
    }
}