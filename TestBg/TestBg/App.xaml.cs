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

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
        }
    }
}
