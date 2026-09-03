using Client.Services;
using Shared.Exceptions;
using Shared.Model.Auth;
using Shared.Requests;

namespace Client;

public partial class SignupPage : ContentPage
{
    private readonly IServerProxy _serverProxy;
    private bool _isSubmitting;

    public SignupPage(IServerProxy serverProxy)
    {
        _serverProxy = serverProxy;
        InitializeComponent();
    }

    private async void OnSignupClicked(object? sender, EventArgs e) => await SignupAsync();

    private async void OnSignupSubmitted(object? sender, EventArgs e) => await SignupAsync();

    private async Task SignupAsync()
    {
        if (_isSubmitting)
            return;

        var firstName = FirstNameEntry.Text?.Trim();
        var lastName = LastNameEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            ShowError("Enter your first and last name.");
            return;
        }

        if (!ManualNetEmail.TryParseFrom(EmailEntry.Text, out var email))
        {
            ShowError("Enter a valid email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowError("Enter a password.");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            ShowError("The passwords do not match.");
            return;
        }
        
        var password = Password.Parse(PasswordEntry.Text);

        if (!password.IsValid)
        {
            ShowError(password.ErrorMessage);
            return;
        }

        SetBusy(true);
        try
        {
            var userDto = new ManualNetUserDto { FirstName = firstName, LastName = lastName, Email = email };
            var request = new RegisterRequest(userDto, password);
            await _serverProxy.RegisterAsync(request);

            await DisplayAlertAsync("Account created", "You can now log in.", "Continue");
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}/{nameof(LoginPage)}");

        }
        catch (ManualNetException e)
        {
            ShowError(e.Message);
        }
        catch (HttpRequestException)
        {
            ShowError("Could not connect to the server.");
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnLoginClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(LoginPage));

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
        SignupButton.IsEnabled = !isBusy;
        if (isBusy)
            ErrorLabel.IsVisible = false;
    }
}
