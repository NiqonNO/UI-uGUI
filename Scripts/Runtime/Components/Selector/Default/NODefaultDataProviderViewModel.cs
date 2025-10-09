using System;
using NiqonNO.UGUI.MVVM;
using UnityEngine;

namespace NiqonNO.UGUI
{
    public class NODefaultDataProviderViewModel : NOMVVBaseViewModel<NODefaultDataProvider>
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;

        public void SetData(INODataProvider itemData)
        {
            if (itemData is NODefaultDataProvider provider)
                base.SetData(provider);
        }
    }
}
