using System;
using TestBg.Services;
using Xamarin.Forms;

namespace TestBg
{
    public partial class MainPage : ContentPage
    {
        private readonly IPlaybackService playbackService;
        private volatile bool isPlaying = false;
        public MainPage()
        {
            InitializeComponent();
            playbackService = DependencyService.Get<IPlaybackService>();
        }

        protected override void OnAppearing()
        {
            playbackService.Init(@"https://aod-rfi.akamaized.net/savoirs/apprendre/actu/jff/JFF-21-10-20.mp3");
            base.OnAppearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                playbackService.Pause();
            }
            else
            {
                playbackService.Play();
            }
            isPlaying = !isPlaying;
        }
    }
}
