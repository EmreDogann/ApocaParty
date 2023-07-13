namespace UI.Views
{
    public interface IViewable
    {
        public void Initialize();

        public void Open();

        public void Close();

        public bool IsActive();
    }
}