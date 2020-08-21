using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // https://aod-rfi.akamaized.net/savoirs/apprendre/actu/jff/jff-19082020.mp3
            playbackService.Init(@"https://aod-rfi.akamaized.net/savoirs/apprendre/actu/jff/jff-19082020.mp3");
            base.OnAppearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                playbackService.Pause();
                /* TODO CurrentItem.MediaProgress = playbackLocation;
                UpdateHomeItemHistoryBackground();*/
            }
            else
            {
                playbackService.Play();
            }
            isPlaying = !isPlaying;
        }
    }
}
