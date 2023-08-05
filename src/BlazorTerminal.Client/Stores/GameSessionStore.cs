namespace BlazorTerminal.Client.Stores;

internal class GameSessionStore
{
    public GameSessionState State { get; private set; } = GameSessionState.Initializing();
    public event Action? OnChange;
    
    public void SetState(GameSessionState state)
    {
        State = state;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}