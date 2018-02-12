using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace Dancer_android
{
    class MysqlConnector
    {
        private MySqlConnection connection;
        private MySqlCommand command;
        private MySqlDataAdapter adapter;
        public DataSet dataset = new DataSet();
        private string user_name;
        public void Init(string MysqlServerIP, string MysqlCatalog, string MysqlUserID, string MysqlPassword, string MysqlPort)
        {
            //string conn_mes = String.Format("Data Source=47.92.75.9;Initial Catalog=dancer;User id=root;password=fakepwd;CharSet=utf8;Port=8783");
            string conn_mes = String.Format("Data Source={0};Initial Catalog={1};User id={2};password={3};CharSet=utf8;Port={4}", MysqlServerIP, MysqlCatalog, MysqlUserID, MysqlPassword, MysqlPort);
            user_name = MysqlUserID;
            connection = new MySqlConnection(conn_mes);
            command = new MySqlCommand("", connection);
            adapter = new MySqlDataAdapter(command);
        }
        public int AddNewSong(string music_name, string singer, string belong_to_list, string other_singer = null, string album = null, int publish_year = 0)
        {
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT count(*) FROM music WHERE user_name = '" + user_name + "' AND music_name = '" + music_name + "' AND singer = '" + singer + "'";
            connection.Open();
            object cnt = command.ExecuteScalar();
            connection.Close();
            if(Convert.ToInt32(cnt) != 0)return -1;
            AddNewList(belong_to_list);
            string _other_singer = other_singer == null ? "" : ",other_singer";
            string _album = album == null ? "" : ",album";
            string _publish_year = publish_year == 0 ? "" : ",publish_year";
            string other_singer_ = other_singer == null ? "" : Decorate(other_singer);
            string album_ = album == null ? "" : Decorate(album);
            string publish_year_ = publish_year == 0 ? "" : Decorate(publish_year.ToString());
            string otherParamName = _other_singer + _album + _publish_year;
            string wholeParam = Decorate(user_name,false) + Decorate(music_name) + Decorate(singer) + Decorate(belong_to_list) + other_singer_ + album_ + publish_year_;
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO music(user_name, music_name, singer, belong_to_list" + otherParamName + ")VALUES(" + wholeParam + ")";
            connection.Open();
            int Return = command.ExecuteNonQuery();
            connection.Close();
            return Return;
        }
        public int GetSongList()
        {
            if (dataset.Tables.Contains("all_songs")) dataset.Tables["all_songs"].Clear();
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT music_name, singer FROM music WHERE user_name = '" + user_name + "'";
            connection.Open();
            int Return = adapter.Fill(dataset, "all_songs");
            connection.Close();
            return Return;
        }
        public int AddNewList(string list_name)
        {
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT count(*) FROM lists WHERE user_name = '" + user_name + "' AND list_name = '" + list_name + "'";
            connection.Open();
            object cnt = command.ExecuteScalar();
            connection.Close();
            if (Convert.ToInt32(cnt) != 0) return -1;
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO lists(user_name, list_name)VALUES(" + Decorate(user_name,false) + Decorate(list_name) + ")";
            connection.Open();
            int res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }
        public int AddListeningRecord(string music_name, string singer)
        {
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO listening(music_name, singer, where_to_listen, user_name)VALUES(" + Decorate(music_name,false) + Decorate(singer) + Decorate("Android") + Decorate(user_name) + ")";
            connection.Open();
            int res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }
        public int GetCurrentSong(ref string music_name, ref string singer)
        {
            command.Parameters.Clear();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "get_current_song";
            MySqlParameter _music_name = new MySqlParameter("_music_name", MySqlDbType.VarChar);
            MySqlParameter _singer = new MySqlParameter("_singer", MySqlDbType.VarChar);
            MySqlParameter _user_name = new MySqlParameter("_user_name", MySqlDbType.VarChar);
            _music_name.Direction = ParameterDirection.Output;
            _singer.Direction = ParameterDirection.Output;
            _user_name.Direction = ParameterDirection.Input;
            _user_name.Value = user_name;
            command.Parameters.Add(_music_name);
            command.Parameters.Add(_singer);
            command.Parameters.Add(_user_name);
            connection.Open();
            int res = command.ExecuteNonQuery();
            music_name = _music_name.Value.ToString();
            singer = _singer.Value.ToString();
            connection.Close();
            return res;
        }
        private string Decorate(string _item, bool comma = true) => (comma ? "," : "") + "'" + _item + "'";
    }
}
