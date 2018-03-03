using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Media;
using Java.IO;
using Android.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Android.Runtime;
using Android.Content;

namespace Dancer_android
{
    [Activity(Label = "Dancer", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public string cur_music_name, cur_singer;
        private ImageButton btnPlay, btnMenu, btnMode;
        private TextView txtTitle;
        private MediaPlayer player;
        private string musicRootPath; private struct Music
        {
            public string musicPath, musicName, fileName, singer, album, belongToList, otherSinger;
            public int publishYear;
        };
        private struct SimMusic
        {
            public string musicName, Singer;
            public SimMusic(string _musicName, string _singer)
            {
                musicName = _musicName;
                Singer = _singer;
            }
        };
        public struct Lyric
        {
            public double position;
            public string lyric_content;
        };
        private List<SimMusic> to_upload_music;
        private List<SimMusic> to_download_music;
        private List<Music> musicPath = new List<Music>();
        private List<Music> to_upload_music_list = new List<Music>();
        private MysqlConnector mysqlConnector = new MysqlConnector();
        private Handler refreshLyric;
        private List<Lyric> lyric;
        private TextView[] txtLyrics = new TextView[9];
        private ProgressBar playProgress;
        private int curLine = 0;
        private bool curMode = true;  // true means random
        private static PowerManager.WakeLock wakeLock;  

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            InitMysql(ref mysqlConnector);
            LoadMusic();
            SetContentView(Resource.Layout.Main);
            btnPlay = FindViewById<ImageButton>(Resource.Id.btnPlay);
            btnPlay.Click += BtnPlay_Click;
            btnMode = FindViewById<ImageButton>(Resource.Id.btnMode);
            btnMode.Click += BtnMode_Click;
            btnMenu = FindViewById<ImageButton>(Resource.Id.btnMenu);
            btnMenu.Click += BtnMenu_Click;
            txtTitle = FindViewById<TextView>(Resource.Id.musicTitle);
            playProgress = FindViewById<ProgressBar>(Resource.Id.playProgress);
            playProgress.Max = 100;
            txtLyrics[0] = FindViewById<TextView>(Resource.Id.lyricLine1);
            txtLyrics[1] = FindViewById<TextView>(Resource.Id.lyricLine2);
            txtLyrics[2] = FindViewById<TextView>(Resource.Id.lyricLine3);
            txtLyrics[3] = FindViewById<TextView>(Resource.Id.lyricLine4);
            txtLyrics[4] = FindViewById<TextView>(Resource.Id.lyricLine5);
            txtLyrics[5] = FindViewById<TextView>(Resource.Id.lyricLine6);
            txtLyrics[6] = FindViewById<TextView>(Resource.Id.lyricLine7);
            txtLyrics[7] = FindViewById<TextView>(Resource.Id.lyricLine8);
            txtLyrics[8] = FindViewById<TextView>(Resource.Id.lyricLine9);
            player = new MediaPlayer();
            player.Completion += Player_Completion;
            PlayNewSong();
        }

        private void InitMysql(ref MysqlConnector m)
        {
            m.Init("47.92.75.9", "dancer", "root", "luoyuan119026", "8783");
        }
        //单曲循环
        private string cycleMusicName = "", cycleSinger = "";
        public int CheckSong(string songInfo)
        {
            string music_name, singer;
            Match match = Regex.Match(songInfo, @"^(.+?)(\s(.+))?$");
            if (match.Success)
            {
                music_name = match.Groups[1].ToString();
                if (match.Groups.Count > 3 && match.Groups[3].ToString() != "")
                {
                    singer = match.Groups[3].ToString();
                    List<Music> find_music_list = musicPath.FindAll(name => { return name.musicName == music_name && name.singer == singer; });
                    if (find_music_list.ToArray().Length == 0) return -1;
                    else
                    {
                        cycleMusicName = music_name;
                        cycleSinger = singer;
                    }
                }
                else
                {
                    List<Music> find_music_list = musicPath.FindAll(name => { return name.musicName == music_name; });
                    if (find_music_list.ToArray().Length == 0) return -1;
                    else
                    {
                        cycleMusicName = music_name;
                        cycleSinger = "";
                    }
                }
            }
            else
            {
                cycleSinger = "";
                cycleMusicName = "";
                if (songInfo == "") return 1;
                else return -1;
            }
            return 0;
        }
        //自动播放下一曲
        private void Player_Completion(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            if (directClose)
            {
                this.Finish();
                return;
            }
            if (cycleMusicName == "" && cycleSinger == "") PlayNewSong();
            else if (cycleSinger == "") PlayNewSong(cycleMusicName);
            else PlayNewSong(cycleMusicName, cycleSinger);
        }
        private void PlayNewSong()
        {
            string music_name = "", singer = "";
            mysqlConnector.GetCurrentSong(ref music_name, ref singer);
            mysqlConnector.AddListeningRecord(music_name, singer);
            Music finded_music = musicPath.Find(name => { return name.musicName == music_name && name.singer == singer; });
            txtTitle.Text = singer + " - " + music_name;
            //log.Info(String.Format("Playing song {0} by {1}...", music_name, singer));
            PlaySong(finded_music.musicPath);
            //processTimer.Start();
            cur_music_name = music_name;
            cur_singer = singer;
            LyricReader.Init(finded_music.fileName, finded_music.belongToList, musicRootPath);
            InitLyric(txtTitle.Text, LyricReader.LoadLyric());
        }
        private void PlayNewSong(string music_name)
        {
            Music finded_music = musicPath.Find(name => { return name.musicName == music_name; });
            string singer = finded_music.singer;
            mysqlConnector.AddListeningRecord(music_name, singer);
            txtTitle.Text = singer + " - " + music_name;
            //log.Info(String.Format("Playing song {0} by {1}...", music_name, singer));
            PlaySong(finded_music.musicPath);
            cur_music_name = music_name;
            cur_singer = singer;
            LyricReader.Init(finded_music.fileName, finded_music.belongToList, musicRootPath);
            InitLyric(txtTitle.Text, LyricReader.LoadLyric());
        }
        private void PlayNewSong(string music_name, string singer)
        {
            mysqlConnector.AddListeningRecord(music_name, singer);
            Music finded_music = musicPath.Find(name => { return name.musicName == music_name && name.singer == singer; });
            txtTitle.Text = singer + " - " + music_name;
            //log.Info(String.Format("Playing song {0} by {1}...", music_name, singer));
            PlaySong(finded_music.musicPath);
            cur_music_name = music_name;
            cur_singer = singer;
            LyricReader.Init(finded_music.fileName, finded_music.belongToList, musicRootPath);
            InitLyric(txtTitle.Text, LyricReader.LoadLyric());
        }
        //更新歌词和进度条

        public void InitLyric(string _title, List<MainActivity.Lyric> _lyric)
        {
            foreach (TextView txtLyric in txtLyrics) txtLyric.Text = "";
            txtLyrics[4].Text = _title;
            if (_lyric.Count != 0)
            {
                lyric = _lyric;
                lyric.Sort((x, y) => { return x.position == y.position ? 0 : (x.position > y.position ? 1 : -1); });
                for (int i = 0; i < 4; i++) txtLyrics[i + 5].Text = lyric[i].lyric_content;
                curLine = 0;
            }
            else lyric.Clear();
            StartUpdateLyric(10, () => player.CurrentPosition < player.Duration - 1, new Handler(Looper.MainLooper));
        }
        private void StartUpdateLyric(long totalMilliseconds, Func<bool> callback, Handler refreshLyric)
        {
            refreshLyric.PostDelayed(() =>
            {
                playProgress.Progress = player.CurrentPosition * 100 / player.Duration;
                if (curLine < lyric.Count && player.CurrentPosition > lyric[curLine].position * 1000)
                {
                    for (int i = 0; i < 8; i++) txtLyrics[i].Text = txtLyrics[i + 1].Text;
                    if (curLine + 4 < lyric.Count) txtLyrics[8].Text = lyric[curLine + 4].lyric_content;
                    else txtLyrics[8].Text = "";
                    curLine++;
                }
                if (callback()) StartUpdateLyric(totalMilliseconds, callback, refreshLyric);
                //refreshLyric.Dispose();
                //refreshLyric = null;
            }, totalMilliseconds);
        }
        //加载音乐
        private void LoadMusic()
        {
            musicRootPath = GetString(Resource.String.music_root_path);
            File f = new File(musicRootPath);
            if (!f.Exists())
            {
                txtTitle.Text = "歌曲路径不存在！";
                this.Finish();
            }
            musicPath.Clear();
            File[] dirInfo = f.ListFiles();
            if (dirInfo.Length == 0)
            {
                txtTitle.Text = "歌单不存在！";
                this.Finish();
            }
            foreach (File NextFolder in dirInfo)
            {
                mysqlConnector.AddNewList(NextFolder.Name);
                if (NextFolder.IsFile) continue;
                File[] fileInfo = NextFolder.ListFiles();
                foreach (File NextFile in fileInfo)
                {
                    Match match_nc = Regex.Match(NextFile.Name, @"(.*?)\s-\s(.*)\.mp3");
                    Match match_xm = Regex.Match(NextFile.Name, @"(.*?)_(.*)\.mp3");
                    if (match_nc.Success || match_xm.Success)
                    {
                        Match match = match_nc.Success ? match_nc : match_xm;
                        int nc_or_xm = match_nc.Success ? 2 : 1;
                        Music music = new Music();
                        music.musicPath = NextFile.Path;
                        music.musicName = match.Groups[nc_or_xm].ToString();
                        music.fileName = System.IO.Path.GetFileNameWithoutExtension(NextFile.Name);
                        music.belongToList = NextFolder.Name;
                        Match singer_match = Regex.Match(match.Groups[3 - nc_or_xm].ToString(), @"(.*?)(、|&|\s|,)(.*)");
                        if (singer_match.Success)
                        {
                            music.singer = singer_match.Groups[1].ToString();
                            music.otherSinger = singer_match.Groups[3].ToString();
                        }
                        else music.singer = match.Groups[3 - nc_or_xm].ToString();
                        musicPath.Add(music);
                    }
                }
            }
        }

        private void PlaySong(string music_path)
        {
            try
            {
                player.Stop();
                player.Reset();
            }
            catch { }
            player.SetDataSource(music_path);
            player.Prepare();
            player.Start();
        }

        private bool directClose = false;
        private void BtnMenu_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            if (directClose)
            {
                player.Stop();
                this.Finish();
            }
            else
            {
                directClose = true;
                txtLyrics[3].Text = "播放结束时退出";
            }
        }

        private void BtnMode_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            /*btnMode.Text = btnMode.Text == "∝" ? "¹" : "∝";
            if (btnMode.Text == "∝") CheckSong("");
            else btnMode.Text = CheckSong(cur_music_name + " " + cur_singer)== 0 ? "¹": "∝";*/
            if (curMode)
                btnMode.SetImageResource(Resource.Drawable.cycle);
            else
                btnMode.SetImageResource(Resource.Drawable.random);
            curMode = !curMode;
            if (curMode) CheckSong("");
            else if (CheckSong(cur_music_name + " " + cur_singer) < 0)
            {
                btnMode.SetImageResource(Resource.Drawable.random);
                curMode = true;
            }
        }

        private void BtnPlay_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            //btnPlay.Text = player.IsPlaying ? " ▷" : "∥";
            if (player.IsPlaying)
                btnPlay.SetImageResource(Resource.Drawable.logo);
            else
                btnPlay.SetImageResource(Resource.Drawable.Pause);
            if (player.IsPlaying) player.Pause();
            else player.Start();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                MoveTaskToBack(false);
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}

