using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using NAudio.Wave;

namespace MusicScheduler
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private Timer TrackTimer { get; set; }
        private Timer SongTimer { get; set; }
        private Playlist Playlist = new Playlist();
        private bool isNotPlaying = true;
        private int Min = 6;
        private int Max = 22;

        public MainWindow()
        {
            this.InitializeComponent();

            this.SongTimer = new Timer(1000) {AutoReset = true};
            this.SongTimer.Elapsed += (sender, args) =>
            {
                this.Dispatcher?.Invoke(() =>
                {
                    var now = DateTime.Now;

                    if (now.Hour >= this.Min && now.Hour < this.Max)
                    {
                        if (this.isNotPlaying)
                        {
                            this.Playlist.Reset();
                            this.Play(null, new RoutedEventArgs());
                        }
                    }
                    else
                    {
                        this.Stop(null, new RoutedEventArgs());
                    }
                });
            };

            this.SongTimer.Start();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (this.Playlist.Ended)
            {
                this.isNotPlaying = true;
                return;
            }

            this.isNotPlaying = false;

            if (this.outputDevice == null)
            {
                this.outputDevice = new WaveOutEvent();
                this.outputDevice.PlaybackStopped += this.OnPlayBackStop;
            }

            if (this.audioFile == null)
            {
                this.audioFile = this.Playlist.Next();
                this.outputDevice.Init(this.audioFile);
            }

            this.outputDevice.Play();
            this.TrackTimer = new Timer(1000) { AutoReset = true };
            this.TrackTimer.Elapsed += this.TrackChanged;
            this.TrackTimer.Start();



            this.CurrentlyPlaying.Text = $"Currently playing: {Path.GetFileName(this.audioFile.FileName)} {this.Track()}";
        }

        private void TrackChanged(object sender,ElapsedEventArgs e)
        {
            this.Dispatcher?.Invoke(() =>
            {
                this.CurrentlyPlaying.Text = $"Currently playing: {Path.GetFileName(this.audioFile.FileName)} {this.Track()}";
                this.MainTimer.Text = this.Track();
                double totalSeconds = this.audioFile.TotalTime.Minutes * 60 + this.audioFile.TotalTime.Seconds;
                double currentSeconds = this.audioFile.CurrentTime.Minutes * 60 + this.audioFile.CurrentTime.Seconds;

                if (Math.Abs(currentSeconds) > 0)
                {
                    double result = Math.Round(currentSeconds / totalSeconds * 100);
                    this.TrackSlider.Value = result;
                }
            });
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            this.outputDevice?.Stop();
            this.TrackTimer.Stop();
        }

        private void Pause(object sedner, RoutedEventArgs e)
        {
            this.outputDevice?.Pause();
            this.TrackTimer.Stop();
        }

        private void OnPlayBackStop(object sedner, StoppedEventArgs e)
        {
            this.audioFile.Dispose();
            this.audioFile = null;
            this.TrackSlider.Value = 0;
            this.CurrentlyPlaying.Text = string.Empty;
            this.Play(null, new RoutedEventArgs());
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

        private void TrackChanged(object sender, MouseButtonEventArgs e)
        {
            double a = this.TrackSlider.Value;

            double totalSeconds = this.audioFile.TotalTime.Minutes * 60 + this.audioFile.TotalTime.Seconds;

            double result = a / 100 * totalSeconds;

            this.audioFile.CurrentTime = TimeSpan.FromSeconds(result);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (this.audioFile != null)
            {
                this.audioFile.Dispose();
                this.audioFile = null;
            }

            if (this.outputDevice != null)
            {
                this.outputDevice.Dispose();
                this.outputDevice = null;
            }
        }
    }
}
