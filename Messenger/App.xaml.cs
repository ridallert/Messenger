using Messenger.Dialogs;
using Messenger.Views;
using Messenger.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>("NotificationDialog");
        }
    }
}
