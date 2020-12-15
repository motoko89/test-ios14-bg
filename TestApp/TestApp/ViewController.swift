//
//  ViewController.swift
//  TestApp
//
//  Created by Hung H Ho on 10/22/20.
//

import UIKit
import AVFoundation

class ViewController: UIViewController {
    var audioPlayer = AVPlayer()
    var playableKey = "playable"
    
    var ding = AVAudioPlayer()
    @IBOutlet weak var playButton: UIButton!
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        let assetOptions = [AVURLAssetPreferPreciseDurationAndTimingKey : 1]
        let asset = AVURLAsset(url: URL(string: "https://aod-rfi.akamaized.net/savoirs/apprendre/actu/jff/jff-28102020.mp3")!, options: assetOptions)
        asset.loadValuesAsynchronously(forKeys: [playableKey]) {
            var error: NSError? = nil
            let status = asset.statusOfValue(forKey: self.playableKey, error: &error)
            switch status {
                case .loaded:
                // Sucessfully loaded. Continue processing.
                    DispatchQueue.main.sync {
                        let currentItem = AVPlayerItem(asset: asset)
                        self.audioPlayer = AVPlayer(playerItem: currentItem)
                        print("Init")
                    }
                    print("Loaded")
            case .failed:
                // Handle error
                print("Failed")
            case .cancelled:
                print("Cancelled")
                // Terminate processing
            default:
                // Handle all other cases
                print("status " + String(status.rawValue))
            }
        }
        
        let ds = Bundle.main.path(forResource: "ding", ofType: "wav");
        do{
            ding = try AVAudioPlayer(contentsOf: URL(fileURLWithPath: ds!), fileTypeHint: "wav")
        } catch {
            print("Unexpected AVAudioPlayer error: \(error).")
        }
    }

    @IBAction func play(_ sender: UIButton) {
       /* UIApplication.shared.beginReceivingRemoteControlEvents()
        UIApplication.shared.becomeFirstResponder()
        let session = AVAudioSession.sharedInstance()
        do {
            try session.setCategory(.playback, options: .mixWithOthers)
            try session.setActive(true, options: .notifyOthersOnDeactivation)
        } catch {
            print("Unexpected session error: \(error).")
        }
        audioPlayer.play()*/
        ding.play();
    }
}

