using Client.Services;
using Shared.Model.Auth;
using Shared.Requests;
using Email = Shared.Model.Auth.Email;

namespace Client;

public partial class LoginPage : ContentPage
{
    private readonly IServerProxy _serverProxy;
    private bool _isSubmitting;

    public LoginPage(IServerProxy serverProxy)
    {
        _serverProxy = serverProxy;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e) => await LoginAsync();

    private async void OnLoginSubmitted(object? sender, EventArgs e) => await LoginAsync();

    private async Task LoginAsync()
    {
        if (_isSubmitting)
            return;

        if (!Email.TryParseFrom(EmailEntry.Text, out var email))
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
            await _serverProxy.LoginAsync(new LoginRequest(email, password));
            await DisplayAlertAsync("Logged in", "You have successfully logged in.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (ServerProxyException exception)
        {
            ShowError(exception.Message);
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
