using Messenger.Views;
using Messenger.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Messenger.Models;

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

        protected override void RegisterTypes(IContainerRegistry container)
        {

            container.RegisterDialog<AuthorizationDialog, AuthorizationDialogViewModel>("AuthorizationDialog");

            container.RegisterSingleton<IState, State>();  //State>("state");
        }
    }
}
