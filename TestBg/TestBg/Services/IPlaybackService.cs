using System;
namespace TestBg.Services
{
    public interface IPlaybackService
    {
        void Init(string url);
        void Play();
        void Pause();
    }
}
