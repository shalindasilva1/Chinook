namespace Chinook.Services
{
    public class SharedService
    {
        public event Action OnNavigationUpdated;

        public void Reload()
        {
            OnNavigationUpdated?.Invoke();
        }
    }
}
