namespace WikiSum
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                System.Diagnostics.Debug.WriteLine($"FIRST CHANCE EXCEPTION: {eventArgs.Exception}");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {eventArgs.ExceptionObject}");
            };
            //FIRST CHANCE EXCEPTION: System.InvalidOperationException:
            //Unable to resolve service for type 'WikiSum.ViewModels.MainViewModel'
            //while attempting to activate 'WikiSum.MainPage'.
            //at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.CreateArgumentCallSites(ServiceIdentifier serviceIdentifier, Type implementationType, CallSiteChain callSiteChain, ParameterInfo[] parameters, Boolean throwIfCallSiteNotFound)

            InitializeComponent();
        }

    }
}
