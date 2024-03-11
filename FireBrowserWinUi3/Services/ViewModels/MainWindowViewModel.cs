using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Behaviors;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FireBrowserWinUi3.Services.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient
{
    internal MainWindow MainView { get; set;  }
    public MainWindowViewModel(IMessenger messenger) : base(messenger)
    {
        Messenger.Register<Message_Settings_Actions>(this, ReceivedStatus);

    }

    private void ReceivedStatus(object recipient, Message_Settings_Actions message)
    {
        var note = new Notification
        {
            Title = "Fire Broswer WinUi3\n",
            Message = $"Welcomes, {AuthService.CurrentUser.Username.ToUpperInvariant()} !",
            Severity = InfoBarSeverity.Informational,
            IsIconVisible = true,
            Duration = TimeSpan.FromSeconds(5),
            
        };
        
        MainView.NotificationQueue.Show(note);


    }
}
