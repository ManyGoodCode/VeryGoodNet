
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Server.UI.Components.Common;

public partial class ErrorHandler
{
    public List<Exception> receivedExceptions = new();

    protected override  Task OnErrorAsync(Exception exception)
    {
        receivedExceptions.Add(exception);
        switch (exception)
        {
            case UnauthorizedAccessException:
                Snackbar.Add("Authentication Failed", Severity.Error);
                break;
        }

        return Task.CompletedTask;
    }

    public new void Recover()
    {
        receivedExceptions.Clear();
        base.Recover();
    }
}