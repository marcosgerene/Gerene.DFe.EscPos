using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using Gerene.DFe.EscPos.Demo.Avalonia.ViewModels;
using Gerene.DFe.EscPos.Demo.Avalonia.Views;
using System;
using System.Text;

namespace Gerene.DFe.EscPos.Demo.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            throw new NotImplementedException();
            //singleViewPlatform.MainView = new MainView
            //{
            //    DataContext = new MainViewModel()
            //};
        }

        base.OnFrameworkInitializationCompleted();


        //https://github.com/leandrovip/Vip.Printer/issues/62#issuecomment-1407498207
        Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
}
