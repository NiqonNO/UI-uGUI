using System.Collections.Generic;
using NiqonNO.Core;
using UnityEngine;

namespace NiqonNO.UGUI
{
    public class NODataCollection<T> : NOEventAsset<T>, INODataCollection<T> where T : INODataProvider
    {
        [SerializeField] 
        private List<T> ItemData = default;
        List<T> INODataCollection<T>.ItemData => ItemData;

        public int Count => ItemData.Count;
        
        public T GetDataAt(int index) => ItemData[index];

        public void SelectDataItem(int index) => Raise(GetDataAt(index));
    }
}