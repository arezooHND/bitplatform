﻿using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace Bit.BlazorUI.Demo.Client.Core.Shared;

public partial class MainLayout : IDisposable
{
    private bool _disposed;
    private bool _isMenuOpen;
    private bool _isUserAuthenticated;
    private string? _pageTitle;
    private Action _unsubscribe = default!;
    private ErrorBoundary ErrorBoundaryRef = default!;

    [AutoInject] private IStateService _stateService = default!;

    [AutoInject] private IPubSubService _pubSubService = default!;

    [AutoInject] private IExceptionHandler _exceptionHandler = default!;

    [AutoInject] private NavigationManager _navigationManager = default!;

    public string CurrentUrl { get; set; }

    protected override void OnParametersSet()
    {
        // TODO: we can try to recover from exception after rendering the ErrorBoundary with this line.
        // but for now it's better to persist the error ui until a force refresh.
        // ErrorBoundaryRef.Recover();

        base.OnParametersSet();
    }

    protected override void OnInitialized()
    {
        try
        {
            SetCurrentUrl();
            _navigationManager.LocationChanged += OnLocationChanged;

            base.OnInitialized();
        }
        catch (Exception exp)
        {
            _exceptionHandler.Handle(exp);
        }

        _unsubscribe = _pubSubService.Sub(PubSubMessages.PAGE_TITLE_CHANGED, async payload =>
        {
            _pageTitle = payload?.ToString();
            await Task.Yield();
            StateHasChanged();
        });
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
        SetCurrentUrl();
        StateHasChanged();
    }

    private void SetCurrentUrl()
    {
        CurrentUrl = _navigationManager.Uri.Replace(_navigationManager.BaseUri, "/", StringComparison.InvariantCultureIgnoreCase);
    }

    public void Dispose()
    {
        _unsubscribe.Invoke();
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}
