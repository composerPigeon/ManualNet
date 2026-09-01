using Client.Services;
using Shared.Exceptions;
using Shared.Model.Auth;
using Shared.Requests;

namespace Client;

public partial class LoginPage : ContentPage
{
    private readonly IServerProxy _proxy;
    private readonly IAuthService _auth;
    private bool _isSubmitting;

    public LoginPage(IServerProxy proxy, IAuthService auth)
    {
        _proxy = proxy;
        _auth = auth;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e) => await LoginAsync();

    private async void OnLoginSubmitted(object? sender, EventArgs e) => await LoginAsync();

    private async Task LoginAsync()
    {
        if (_isSubmitting)
            return;

        if (!ManualNetEmail.TryParseFrom(EmailEntry.Text, out var email))
        {
            ShowError("Enter a valid email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowError("Enter your password.");
            return;
        }

        var password = Password.Parse(PasswordEntry.Text);

        SetBusy(true);
        try
        {
            var authResponse = await _proxy.LoginAsync(new LoginRequest(email, password));
            await _auth.StoreRefreshTokenAsync(authResponse.RefreshToken);
            await _auth.StoreAuthTokenAsync(authResponse.AuthToken);
            
            await DisplayAlertAsync("Logged in", "You have successfully logged in.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (ManualNetException e)
        {
            ShowError(e.UserMessage);
        }
        catch (HttpRequestException)
        {
            ShowError("Could not connect to the server.");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnSignupClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(SignupPage));

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private void SetBusy(bool isBusy)
    {
        _isSubmitting = isBusy;
        BusyIndicator.IsVisible = isBusy;
        BusyIndicator.IsRunning = isBusy;
        LoginButton.IsEnabled = !isBusy;
        if (isBusy)
            ErrorLabel.IsVisible = false;
    }
}
