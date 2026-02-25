using NiqonNO.Core;
using UnityEngine;

namespace NiqonNO.UGUI
{
    public class NODefaultDataProvider : NOScriptableObject, INODataProvider
    {
        [SerializeField] 
        private NOValue<string> _ItemName;
        public string ItemName => _ItemName.Value;
        
        [SerializeField] 
        private Sprite _ItemIcon;
        public Sprite ItemIcon => _ItemIcon;
        
        [SerializeField] 
        private NOValue<Color> _ItemColor;
        public Color ItemColor => _ItemColor.Value;
    }
}