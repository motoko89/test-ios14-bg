using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using TestBg.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(TestBg.iOS.Services.PlaybackService))]
namespace TestBg.iOS.Services
{
    public class PlaybackService : IPlaybackService
    {
        private const string AVASSET_PLAYABLE_KEY = "playable";
        private AVPlayer player;
        private AVPlayerItem currentItem;
        private NSObject playEndHandle;
        private AVAudioPlayer speechPlayer;

        public long GetLengthSecond()
        {
            if (currentItem == null)
            {
                return 0;
            }

            return (long)currentItem.Asset.Duration.Seconds;
        }

        public void Init(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var asset = new AVUrlAsset(
                    NSUrl.FromString(url),
                    new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                                            NSNumber.FromBoolean(true),
                                            AVUrlAsset.PreferPreciseDurationAndTimingKey)));
            asset.LoadValuesAsynchronously(new string[] { AVASSET_PLAYABLE_KEY }, () => AVAssetLoadComplete(url, asset));
        }

        private void AVAssetLoadComplete(string url, AVUrlAsset asset)
        {
            try
            {
                NSError err;
                var value = asset.StatusOfValue(AVASSET_PLAYABLE_KEY, out err);
                switch (value)
                {
                    case AVKeyValueStatus.Loaded:

                        DispatchQueue.MainQueue.DispatchSync(() =>
                        {
                            InitPlayerForeground(url, asset);
                        });
                        break;
                    case AVKeyValueStatus.Failed:
                    case AVKeyValueStatus.Cancelled:
                        Debug.WriteLine("FailPlaybackServiceInit");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FailAVAssetLoadComplete");
            }
        }

        private void InitPlayerForeground(string url, AVUrlAsset asset)
        {
            playEndHandle = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, HandlePlayEnd);
            if (player == null)
            {
                currentItem = new AVPlayerItem(asset);
                player = new AVPlayer(currentItem);
                player.AddPeriodicTimeObserver(new CMTime(1, 1), DispatchQueue.MainQueue, TimeUpdate);
            }
            else
            {
                currentItem.Dispose();
                currentItem = new AVPlayerItem(
                    new AVUrlAsset(NSUrl.FromString(url),
                    new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                                            NSNumber.FromBoolean(true),
                                            AVUrlAsset.PreferPreciseDurationAndTimingKey))));
                player.ReplaceCurrentItemWithPlayerItem(currentItem);
            }
            Debug.WriteLine("InitPlayerForeground done");
        }

        private void HandlePlayEnd(NSNotification obj)
        {
            Debug.WriteLine("HandlePlayEnd");
            ToPauseSessionState();
            Debug.WriteLine("PLAYBACK_END");
        }

        private void TimeUpdate(CMTime obj)
        {
            Debug.WriteLine("PLAYBACK_PROGRESS " + obj.Seconds.ToString());
        }

        public void Pause()
        {
            player?.Pause();
            ToPauseSessionState();
        }

        private void ToPauseSessionState()
        {
            if (UIApplication.SharedApplication.CanResignFirstResponder)
            {
                UIApplication.SharedApplication.ResignFirstResponder();
                UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            }
            AVAudioSession.SharedInstance().SetActive(false);
        }

        private void ToActiveSessionState()
        {
            /*if (UIApplication.SharedApplication.CanBecomeFirstResponder)
            {*/
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            UIApplication.SharedApplication.BecomeFirstResponder();
            //}
            var session = AVAudioSession.SharedInstance();
            session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.MixWithOthers);
            session.SetActive(true);
        }

        public void Play()
        {
            ToActiveSessionState();
            player?.Play();
        }

        public Task Play(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return Task.CompletedTask;
            }

            NSData src = NSData.FromArray(data);
            speechPlayer = new AVAudioPlayer(src, "mp3", out NSError err);
            if (err != null)
            {
                Debug.WriteLine("Speech Play FAILED " + err.Description);
                return Task.CompletedTask;
            }
            speechPlayer.FinishedPlaying += SpeechPlayer_FinishedPlaying;
            ToActiveSessionState();
            speechPlayer.Play();
            return Task.CompletedTask;
        }

        private void SpeechPlayer_FinishedPlaying(object sender, AVStatusEventArgs e)
        {
            if (speechPlayer != null)
            {
                Device.BeginInvokeOnMainThread(() => {
                    speechPlayer.Stop();
                    speechPlayer.FinishedPlaying -= SpeechPlayer_FinishedPlaying;
                    speechPlayer.Dispose();
                    speechPlayer = null;
                    Debug.WriteLine("SPEECH_PLAY_DONE");
                });
            }
            ToPauseSessionState();
        }

        public void Dispose()
        {
            if (playEndHandle != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(playEndHandle);
            }
        }
    }
}
