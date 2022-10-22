namespace RecipeLewis.Business;

public class MenuService
{
    public bool IsAuthenticated { get; private set; }

    public void SetIsAuthenticated(bool isAuthenticated)
    {
        IsAuthenticated = isAuthenticated;
        NotifyStateChanged();
    }

    public event Action OnChange; // event raised when changed

    private void NotifyStateChanged() => OnChange?.Invoke();
}