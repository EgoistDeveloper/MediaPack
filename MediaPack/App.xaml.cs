using System;
using System.Threading;
using Dna;
using MediaPack.DI;
using MediaPack.Models.Common;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MediaPack.Data;
using MediaPack.Dialogs;
using MediaPack.ViewModel;
using MediaPack.ViewModel.App;
using static MediaPack.DI.DI;
using System.Diagnostics;

namespace MediaPack
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public App()
        {
            Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            ApplicationSetup();

            ViewModelApplication.GoToPage(ApplicationPage.Tv);

            //_ = new AppDbContext();

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.DataContext = new WindowViewModel(Current.MainWindow);
            Current.MainWindow.Show();

            new Setup();
        }

        private void ApplicationSetup()
        {
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .AddAppViewModels()
                .Build();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            //var dialog = new MessageDialog();
            //dialog.ShowDialogWindow(new MessageDialogViewModel(dialog, "Error: Application_DispatcherUnhandledException", args.Exception.ToString()));

            Debug.WriteLine(args.Exception.ToString());
            args.Handled = true;
        }

        private void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args)
        {
            //var dialog = new MessageDialog();
            //dialog.ShowDialogWindow(new MessageDialogViewModel(dialog, "Error: TaskSchedulerOnUnobservedTaskException", args.Exception.ToString()));
            Debug.WriteLine(args.Exception.ToString());
        }

        private void CurrentOnDispatcherUnhandledException(DispatcherUnhandledExceptionEventArgs args)
        {
            //var dialog = new MessageDialog();
            //dialog.ShowDialogWindow(new MessageDialogViewModel(dialog, "Error: CurrentOnDispatcherUnhandledException", args.Exception.ToString()));

            Debug.WriteLine(args.Exception.ToString());
            args.Handled = true;
        }
    }

}
