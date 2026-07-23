// Created by Maxwolf (bigmaxwolf.com)

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Persistence
{
    /// <summary>
    ///     Reads and writes player-earned high scores in game.db. Only the name and points are stored; the performance rating
    ///     is derived from points in code, so it is never persisted. Rows are returned best-first to match the game's top-ten
    ///     display. This holds only scores the player actually earned — the seeded "original" list lives in code.
    /// </summary>
    public sealed class HighScoreStore
    {
        private readonly SqliteConnection _connection;

        public HighScoreStore(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Insert(string name, int points)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO high_scores (name, points) VALUES ($name, $points);";
            cmd.Parameters.AddWithValue("$name", name ?? string.Empty);
            cmd.Parameters.AddWithValue("$points", points);
            cmd.ExecuteNonQuery();
        }

        /// <summary>All persisted scores, ranked best-first (highest points, oldest as tie-break).</summary>
        public IReadOnlyList<(string Name, int Points)> All()
        {
            var list = new List<(string, int)>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT name, points FROM high_scores ORDER BY points DESC, id ASC;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add((reader.GetString(0), reader.GetInt32(1)));
            return list;
        }

        public int Count()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM high_scores;";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Clear()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM high_scores;";
            cmd.ExecuteNonQuery();
        }
    }
}
