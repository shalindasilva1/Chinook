namespace Chinook.Services
{
    public class SharedService
    {
        public event Action OnReload;

        public void Reload()
        {
            OnReload?.Invoke();
        }
    }
}
