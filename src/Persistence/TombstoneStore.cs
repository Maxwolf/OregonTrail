// Created by Maxwolf (bigmaxwolf.com)

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Persistence
{
    /// <summary>
    ///     Reads and writes tombstones in game.db. The mile marker is the primary key, reproducing the in-memory dictionary's
    ///     "one tombstone per mile marker" rule via <c>INSERT OR IGNORE</c> — the first grave to occupy a spot keeps it.
    /// </summary>
    public sealed class TombstoneStore
    {
        private readonly SqliteConnection _connection;

        public TombstoneStore(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Insert(int mileMarker, string playerName, string epitaph)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText =
                "INSERT OR IGNORE INTO tombstones (mile_marker, player_name, epitaph) VALUES ($mile, $name, $epitaph);";
            cmd.Parameters.AddWithValue("$mile", mileMarker);
            cmd.Parameters.AddWithValue("$name", playerName ?? string.Empty);
            cmd.Parameters.AddWithValue("$epitaph", epitaph ?? string.Empty);
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyList<(int MileMarker, string PlayerName, string Epitaph)> All()
        {
            var list = new List<(int, string, string)>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT mile_marker, player_name, epitaph FROM tombstones;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            return list;
        }

        public void Clear()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM tombstones;";
            cmd.ExecuteNonQuery();
        }
    }
}
