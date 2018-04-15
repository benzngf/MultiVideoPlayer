using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EffectsVideoPlayer
{
    public partial class VBlock : Canvas
    {
        public VLayerControl controller;
        public bool isPlaying;
        bool isVisible, isMute;
        DispatcherTimer timer;
        DispatcherTimer startPlayListener;
        private double checkingPositionV;
        public VBlock()
        {
            InitializeComponent();
            isPlaying = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            startPlayListener = new DispatcherTimer();
            startPlayListener.Interval = TimeSpan.FromMilliseconds(30);
            startPlayListener.Tick += checkPlayingState;
            isVisible = true;
            isMute = true;
            mePlayer.IsMuted = true;
            mePlayer.Volume = 1.0;
        }
        private void checkPlayingState(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan))
            {
               if((Math.Abs(mePlayer.Position.TotalSeconds - checkingPositionV) > 0.02))
                {
                    mePlayer.Pause();
                    startPlayListener.Stop();
                }
            }
            else { startPlayListener.Stop(); }
        }

        private void RefreshFrame()
        {
            mePlayer.Play();
            checkingPositionV = mePlayer.Position.TotalSeconds;
            startPlayListener.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (controller!=null))
            {
                controller.updateSeekBar(mePlayer.Position.TotalSeconds/ mePlayer.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        public string openMediaFile(Uri uri)
        {
            if (uri != null)
            {
                mePlayer.Source = uri;
                if (uri.IsFile)
                {
                    PlayVid();
                    return System.IO.Path.GetFileName(uri.LocalPath);
                }
            }
            return "not valid file";
        }
        public double getTotalSeconds() {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan))
            {
                return mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
            else return -1.0;
        }
        public double getCurrentSeconds()
        {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan))
            {
                return mePlayer.Position.TotalSeconds;
            }
            else return -1.0;
        }
        public bool togglePlay(bool isPlay){
            if (mePlayer.Source != null)
            {
                if (isPlaying != isPlay)
                {
                    isPlaying = isPlay;
                    if (isPlay) PlayVid();
                    else PauseVid();
                    return true;
                }
            }
            return false;
        }

        private void MediaReady(object sender, RoutedEventArgs e)
        {
            if (isPlaying) PlayVid();
            else PauseVid();
            if (controller != null) controller.updateFileSize(mePlayer.NaturalVideoWidth, mePlayer.NaturalVideoHeight);
        }

        private void OnMediaEnd(object sender, RoutedEventArgs e)
        {
            if (isPlaying){
                if (controller.isLoop)
                {
                    mePlayer.Position = new TimeSpan(0L);
                    mePlayer.Play();
                }
                else
                {
                    mePlayer.Stop();
                    isPlaying = false;
                    controller.autoPaused();
                }
            }
        }
        public void seek(double durationFrac)
        {
            double tmp = (durationFrac < 0.0) ? 0.0 : ((durationFrac > 1.0) ? 1.0 : durationFrac);
            tmp *= mePlayer.NaturalDuration.TimeSpan.TotalMilliseconds*10000;
            mePlayer.Position = new TimeSpan((long)tmp);
            if (!isPlaying){
                RefreshFrame();
            }
        }

        private void PlayVid()
        {
            mePlayer.Play();
            timer.Start();
        }
        private void PauseVid()
        {
            mePlayer.Pause();
            timer.Stop();
            
        }
        public void toggleVisible()
        {
            isVisible = !isVisible;
            mePlayer.Opacity = isVisible? 1.0:0.0;
        }
        public void toggleMute()
        {
            isMute = !isMute;
            mePlayer.IsMuted = isMute;
        }
    }
}
