﻿using AdminPanel.Shared.Dtos.Identity;

namespace AdminPanel.Client.Core.Pages.Identity;

public partial class SignUpPage
{
    public bool _isLoading;
    public bool _isSignedUp;
    public string? _signUpMessage;
    public BitMessageBarType _signUpMessageType;
    public SignUpRequestDto _signUpModel = new();

    protected async override Task OnAfterFirstRenderAsync()
    {
        if (await AuthenticationStateProvider.IsUserAuthenticatedAsync())
        {
            NavigationManager.NavigateTo("/");
        }

        await base.OnAfterFirstRenderAsync();
    }

    private async Task DoSignUp()
    {
        if (_isLoading) return;

        _isLoading = true;
        _signUpMessage = null;

        try
        {
            await HttpClient.PostAsJsonAsync("Auth/SignUp", _signUpModel, AppJsonContext.Default.SignUpRequestDto);

            _isSignedUp = true;
        }
        catch (ResourceValidationException e)
        {
            _signUpMessageType = BitMessageBarType.Error;
            _signUpMessage = string.Join(Environment.NewLine, e.Payload.Details.SelectMany(d => d.Errors).Select(e => e.Message));
        }
        catch (KnownException e)
        {
            _signUpMessage = e.Message;
            _signUpMessageType = BitMessageBarType.Error;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task DoResendLink()
    {
        if (_isLoading) return;

        _isLoading = true;
        _signUpMessage = null;

        try
        {
            await HttpClient.PostAsJsonAsync("Auth/SendConfirmationEmail", new() { Email = _signUpModel.Email }, AppJsonContext.Default.SendConfirmationEmailRequestDto);

            _signUpMessageType = BitMessageBarType.Success;
            _signUpMessage = Localizer[nameof(AppStrings.ResendConfirmationLinkMessage)];
        }
        catch (KnownException e)
        {
            _signUpMessage = e.Message;
            _signUpMessageType = BitMessageBarType.Error;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
