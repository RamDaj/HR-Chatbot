using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRChatbot.Models;
using HRChatbot.Services;
using System.Collections.ObjectModel;

namespace HRChatbot.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly IOpenAIService _openAIService;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> messages = new();

    [ObservableProperty]
    private string currentInput = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string apiKey = string.Empty;

    [ObservableProperty]
    private bool hasApiKey = false;

    public ChatViewModel(IOpenAIService openAIService)
    {
        _openAIService = openAIService;
        LoadApiKey();
    }

    private async void LoadApiKey()
    {
        try
        {
            var storedKey = await SecureStorage.GetAsync("openai_api_key");
            if (!string.IsNullOrEmpty(storedKey))
            {
                ApiKey = storedKey;
                HasApiKey = true;
            }
        }
        catch
        {
            // API key not set yet
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentInput) || IsLoading)
            return;

        if (!HasApiKey || string.IsNullOrEmpty(ApiKey))
        {
            await Application.Current.MainPage.DisplayAlert("API Key Required", "Please set your OpenAI API key first.", "OK");
            return;
        }

        var userMessage = new ChatMessage
        {
            Role = "user",
            Content = CurrentInput,
            Timestamp = DateTime.Now
        };

        Messages.Add(userMessage);
        var messageText = CurrentInput;
        CurrentInput = string.Empty;
        IsLoading = true;

        try
        {
            var history = Messages.ToList();
            var response = await _openAIService.SendMessageAsync(messageText, history);

            var assistantMessage = new ChatMessage
            {
                Role = "assistant",
                Content = response,
                Timestamp = DateTime.Now
            };

            Messages.Add(assistantMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = new ChatMessage
            {
                Role = "assistant",
                Content = $"Error: {ex.Message}",
                Timestamp = DateTime.Now
            };
            Messages.Add(errorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveApiKeyAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "API key cannot be empty.", "OK");
            return;
        }

        try
        {
            // Validate the API key
            var isValid = await _openAIService.ValidateApiKeyAsync(ApiKey);
            if (!isValid)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Failed", "The API key appears to be invalid. Please check and try again.", "OK");
                return;
            }

            await SecureStorage.SetAsync("openai_api_key", ApiKey);
            _openAIService.UpdateApiKey(ApiKey);
            HasApiKey = true;
            await Application.Current.MainPage.DisplayAlert("Success", "API key saved successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save API key: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
    }
}
