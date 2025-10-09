namespace NiqonNO.UGUI.MVVM
{
    public interface INOMVVMViewModel<TData> : INOMVVMViewModel
    {
        TData ItemData { get; set; }
        void SetData(TData itemData);
    }
}