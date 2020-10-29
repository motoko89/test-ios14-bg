using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using System;
using System.Diagnostics;
using UIKit;

namespace TestNative
{
    public partial class ViewController : UIViewController
    {
        private const string AVASSET_PLAYABLE_KEY = "playable";
        private AVPlayerItem currentItem;
        private NSObject playEndHandle;
        private AVPlayer Player;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            string url = "https://aod-rfi.akamaized.net/savoirs/apprendre/actu/jff/jff-28102020.mp3";
            // Perform any additional setup after loading the view, typically from a nib.
            var asset = new AVUrlAsset(
                    NSUrl.FromString(url),
                    new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                                            NSNumber.FromBoolean(true),
                                            AVUrlAsset.PreferPreciseDurationAndTimingKey)));
            asset.LoadValuesAsynchronously(new string[] { AVASSET_PLAYABLE_KEY }, () => AVAssetLoadComplete(url, asset));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
            Debug.WriteLine("DidReceiveMemoryWarning");
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
            if (Player == null)
            {
                currentItem = new AVPlayerItem(asset);
                Player = new AVPlayer(currentItem);
                Player.AddPeriodicTimeObserver(new CMTime(1, 1), DispatchQueue.MainQueue, TimeUpdate);
            }
            else
            {
                currentItem.Dispose();
                currentItem = new AVPlayerItem(
                    new AVUrlAsset(NSUrl.FromString(url),
                    new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                                            NSNumber.FromBoolean(true),
                                            AVUrlAsset.PreferPreciseDurationAndTimingKey))));
                Player.ReplaceCurrentItemWithPlayerItem(currentItem);
            }
        }

        private void TimeUpdate(CMTime obj)
        {
            Debug.WriteLine("TimeUpdate " +obj.Seconds.ToString());
        }

        private void HandlePlayEnd(NSNotification obj)
        {
            Debug.WriteLine("HandlePlayEnd");
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
            NSError err = session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.MixWithOthers);
            if (err != null)
            {
                Debug.WriteLine("ToActiveSessionState SetCategory FAILED " + err.Description);
            }
            err = null;
            session.SetActive(true, AVAudioSessionFlags.NotifyOthersOnDeactivation, out err);
            if (err != null)
            {
                Debug.WriteLine("ToActiveSessionState SetActive FAILED " + err.Description);
            }
        }

        public void Play()
        {
            ToActiveSessionState();
            Player.Play();
            Debug.WriteLine("AVPlayer play");
        }

        partial void UIButtonW55ViD6q_TouchUpInside(UIButton sender)
        {
            Player.Play();
        }
    }
}