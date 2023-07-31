namespace BlazorTerminal.Client.Components;

public partial class Dialog
{
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public string Title { get; set; }

    private async Task CloseAsync()
    {
        await IsOpenChanged.InvokeAsync(false);
    }
}