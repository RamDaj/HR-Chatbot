using HRChatbot.ViewModels;

namespace HRChatbot;

public partial class MainPage : ContentPage
{
    public MainPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnEntryCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is ChatViewModel vm && !string.IsNullOrWhiteSpace(vm.CurrentInput))
        {
            vm.SendMessageCommand.Execute(null);
        }
    }
}
