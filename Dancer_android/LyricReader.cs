﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Dancer_android
{
    static class LyricReader
    {
        private static string file_name, list_name, music_path;
        public static List<MainActivity.Lyric> lyrics = new List<MainActivity.Lyric>();
        public static void Init(string _file_name, string _list_name, string _music_path)
        {
            file_name = _file_name;
            list_name = _list_name;
            music_path = _music_path;
        }
        
        public static List<MainActivity.Lyric> LoadLyric()
        {
            lyrics.Clear();
            string lyrics_path = String.Format(@"{0}//{1}//{2}.lrc", music_path, list_name, file_name);
            if (File.Exists(lyrics_path))
            {
                string[] lyrics_text;
                lyrics_text = File.ReadAllLines(lyrics_path, Encoding.UTF8);
                foreach(string lyric_text in lyrics_text)
                {
                    Match match = Regex.Match(lyric_text, @"(\[\d+:[\d\.]+\])+(.*)");
                    if (!match.Success) continue;
                    string content = match.Groups[2].ToString();
                    String time = match.Groups[1].ToString();
                    foreach(Match time_match in Regex.Matches(time, @"\[(\d+):([\d\.]+)\]"))
                    {
                        double m, s;
                        m = Convert.ToDouble(time_match.Groups[1].ToString());
                        s = Convert.ToDouble(time_match.Groups[2].ToString());
                        s = m * 60 + s;
                        MainActivity.Lyric lyric;
                        lyric.position = s;
                        lyric.lyric_content = content;
                        lyrics.Add(lyric);
                    }
                }
            }
            return lyrics;
        }
    }
}
