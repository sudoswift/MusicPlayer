using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MusicPlayer
{
    public partial class MainWindow : Window
    {

        private MediaPlayer MusicPlayer = new MediaPlayer();
        public delegate void timeTick();
        DispatcherTimer ticks = new DispatcherTimer();
        string filePath;

        public MainWindow()
        {
            InitializeComponent();
            volumeSlider.Maximum = 100.0;
            volumeSlider.Minimum = 0.0;
            volumeSlider.Value = 30.0;

            if (filePath == null)
            {
                artistNameLabel.Content = "MP3 파일을 업로드 해주세요!";
            }

        }
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (filePath == null)
            {
                artistNameLabel.Content = "MP3 파일을 업로드 해주세요!";
            }

            MusicPlayer.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.Pause();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.Stop();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeSlider.Maximum = 100.0;
            volumeSlider.Minimum = 0;
            MusicPlayer.Volume = volumeSlider.Value / 100.0;
            volumeLabel.Content = "Volume : " + (int)volumeSlider.Value;
        }

        private void loadedMusic(object sender, EventArgs e)
        {
            progressSlider.Maximum = MusicPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            totalTimeLabel.Content = MusicPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += ticks_Tick;
            ticks.Start();
        }
        void ticks_Tick(object sender, object e)
        {
            TimeSpan newTimeSpan = MusicPlayer.Position;
            progressSlider.Value = newTimeSpan.TotalMilliseconds;
            progressLabel.Content = newTimeSpan.ToString("mm':'ss':'ff");
        }
        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan newTimeSpan = MusicPlayer.Position;
            progressLabel.Content = newTimeSpan.ToString(@"mm\:ss");
        }
        private void progressSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            MusicPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)progressSlider.Value);

            TimeSpan newTimeSpan = MusicPlayer.Position;
            progressLabel.Content = newTimeSpan.ToString(@"mm\:ss");
        }
        class MusicID3Tag
        {
            public byte[] TAGID = new byte[3];
            public byte[] Title = new byte[30];
            public byte[] Artist = new byte[30];
            public byte[] Album = new byte[30];
            public byte[] Year = new byte[4];
            public byte[] Comment = new byte[30];
            public byte[] Genre = new byte[1];
        }
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".mp3";
            openFileDlg.Filter = "MP3 file (.mp3)|*.mp3";
            Nullable<bool> music = openFileDlg.ShowDialog();

            if (music == true)
            {
                filePath = openFileDlg.FileName;
                MusicPlayer.Open(new Uri(filePath));
                MusicPlayer.MediaOpened += loadedMusic;
                using (FileStream fs = File.OpenRead(filePath))
                {
                    if (fs.Length >= 128)
                    {
                        MusicID3Tag tag = new MusicID3Tag();
                        fs.Seek(-128, SeekOrigin.End);
                        fs.Read(tag.TAGID, 0, tag.TAGID.Length);
                        fs.Read(tag.Title, 0, tag.Title.Length);
                        fs.Read(tag.Artist, 0, tag.Artist.Length);
                        fs.Read(tag.Album, 0, tag.Album.Length);
                        fs.Read(tag.Year, 0, tag.Year.Length);
                        fs.Read(tag.Comment, 0, tag.Comment.Length);
                        fs.Read(tag.Genre, 0, tag.Genre.Length);
                        string theTAGID = Encoding.Default.GetString(tag.TAGID);

                        if (theTAGID.Equals("TAG"))
                        {
                            string Title = Encoding.Default.GetString(tag.Title);
                            string Artist = Encoding.Default.GetString(tag.Artist);
                            string Album = Encoding.Default.GetString(tag.Album);
                            string Year = Encoding.Default.GetString(tag.Year);
                            string Comment = Encoding.Default.GetString(tag.Comment);
                            string Genre = Encoding.Default.GetString(tag.Genre);

                            songTitleLabel.Content = (Title);
                            artistNameLabel.Content = (Artist);
                            albumTitleLabel.Content = (Album);
                        }
                    }
                }
                MusicPlayer.Play();
            }
        }
    }
}
