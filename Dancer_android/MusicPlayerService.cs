using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dancer_android
{

    [Service(Name = "com.zhengjry.Dancer.MusicPlayerService")]
    public class MusicPlayerService : Service
    {

        public class MusicController : Binder
        {
            public MediaPlayer player = new MediaPlayer();
            public delegate void EventHandler(object sender, EventArgs e);
            public EventHandler Completion;
            public MusicController()
            {
                player.Completion += Player_Completion;
            }

            private void Player_Completion(object sender, EventArgs e)
            {
                Completion(sender, e);
            }
            public int CurrentPosition
            {
                get { return player.CurrentPosition; }
            }
            public int Duration
            {
                get { return player.Duration; }
            }
            public bool IsPlaying
            {
                get { return player.IsPlaying; }
            }

            public void Start()
            {
                player.Start();
            }
            public void Prepare()
            {
                player.Prepare();
            }
            public void Pause()
            {
                player.Pause();
            }
            public void Stop()
            {
                player.Stop();
            }
            public void Reset()
            {
                player.Reset();
            }
            public void SetDataSource(string music_path)
            {
                player.SetDataSource(music_path);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return new MusicController();
        }
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
             return StartCommandResult.Sticky;
        }
    }
}