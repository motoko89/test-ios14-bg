using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TestBg.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestBg
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Handle when your app sleeps
            Task.Run(async () =>
            {
                try
                {
                    await DependencyService.Get<IBackgroundService>().PerformFetch();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("FailOnSleep: " + ex.Message + " : " + ex.StackTrace);
                }
            });
        }

        protected override void OnResume()
        {
        }
    }
}
