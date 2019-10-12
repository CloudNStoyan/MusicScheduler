using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NAudio.Wave;

namespace MusicScheduler
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private Timer Timer { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (this.outputDevice == null)
            {
                this.outputDevice = new WaveOutEvent();
                this.outputDevice.PlaybackStopped += this.OnPlayBackStop;
            }

            if (this.audioFile == null)
            {
                this.audioFile =
                    new AudioFileReader("C:\\Users\\Stoyan\\Desktop\\Phoenix_LeagueofLegends.wav") {Volume = 0.1f};
                this.outputDevice.Init(this.audioFile);
            }

            this.outputDevice.Play();
            this.Timer = new Timer(500) { AutoReset = true };
            this.Timer.Elapsed += this.TrackChanged;
            this.Timer.Start();



            this.currentlyPlaying.Text = $"Currently playing: {Path.GetFileName(this.audioFile.FileName)} {this.Track()}";

            int totalSeconds = this.audioFile.TotalTime.Minutes * 60 + this.audioFile.TotalTime.Seconds;
            int currentSeconds = this.audioFile.CurrentTime.Minutes * 60 + this.audioFile.CurrentTime.Seconds;

            if (currentSeconds != 0)
            {
                MessageBox.Show((totalSeconds / currentSeconds).ToString());
            }
        }

        private void TrackChanged(object sender,ElapsedEventArgs e)
        {
            this.Dispatcher?.Invoke(() =>
            {
                this.currentlyPlaying.Text = $"Currently playing: {Path.GetFileName(this.audioFile.FileName)} {this.Track()}";
            });
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            this.outputDevice?.Stop();
            this.Timer.Stop();
        }

        private void Pause(object sedner, RoutedEventArgs e)
        {
            this.outputDevice?.Pause();
            this.Timer.Stop();
        }

        private void OnPlayBackStop(object sedner, StoppedEventArgs e)
        {
            this.outputDevice.Dispose();
            this.outputDevice = null;
            this.audioFile.Dispose();
            this.audioFile = null;
        }

        private void VolumeChanged(object sender, DragCompletedEventArgs e)
        {
            if (this.outputDevice == null)
            {
                return;
            }

            this.outputDevice.Volume = (float)(((Slider)sender).Value / 100);
        }

        private string Track()
        {
            if (this.audioFile == null)
            {
                return "";
            }

            return
                $"({this.audioFile.CurrentTime.Minutes.ToString().PadLeft(2, '0')}:" +
                $"{this.audioFile.CurrentTime.Seconds.ToString().PadLeft(2, '0')}/" +
                $"{this.audioFile.TotalTime.Minutes.ToString().PadLeft(2, '0')}:" +
                $"{this.audioFile.TotalTime.Seconds.ToString().PadLeft(2, '0')})";
        }
    }
}
