namespace Messenger
{
    using System.Windows;

    using Prism.Ioc;

    using Messenger.Views;
    using Messenger.ViewModels;
    using Messenger.Models;
    using Messenger.Network;

    public partial class App
    {
        #region Methods

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterDialog<NewChatDialog, NewChatDialogViewModel>("NewChatDialog");
            container.RegisterDialog<AuthorizationDialog, AuthorizationDialogViewModel>("AuthorizationDialog");
            container.RegisterDialog<LogWindow, LogWindowViewModel>("LogWindow");
            container.RegisterDialog<ServerConfig, ServerConfigViewModel>("ServerConfigDialog");
            container.RegisterDialog<NotificationWindow>();

            container.RegisterSingleton<ClientStateManager>();
            container.RegisterSingleton<WebSocketClient>();
        }

        #endregion //Methods
    }
}
