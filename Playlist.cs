using System.IO;
using NAudio.Wave;

namespace MusicScheduler
{
    public class Playlist
    {
        private int currentSongIndex = -1;
        public string[] songPaths;
        public bool Ended { get; set; }

        private float volume = 0.1f;

        public Playlist()
        {
            string path = Path.GetFullPath("./Songs");

            if (Directory.Exists(path))
            {
                this.songPaths = Directory.GetFiles(path);
            }
        }


        public AudioFileReader Next()
        {
            if (this.currentSongIndex + 2 >= this.songPaths.Length)
            {
                this.Ended = true;
            }

            if (this.currentSongIndex + 1 < this.songPaths.Length)
            {
                this.currentSongIndex++;

                return new AudioFileReader(this.songPaths[this.currentSongIndex])
                {
                    Volume = this.volume
                };
            }

            return null;
        }

        //public AudioFileReader Previous()
        //{
        //    if (this.currentSongIndex - 1 >= 0)
        //    {
        //        this.Ended = false;
        //        this.currentSongIndex--;
        //
        //        return new AudioFileReader(this.songPaths[this.currentSongIndex])
        //        {
        //            Volume = this.volume
        //        };
        //    }
        //
        //    return null;
        //}

        public void Reset()
        {
            this.Ended = false;
            this.currentSongIndex = -1;
        }
    }
}
