// Created by Maxwolf (bigmaxwolf.com)

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Persistence
{
    /// <summary>
    ///     Reads and writes tombstones in game.db. The trail half (0 or 1) is the primary key, so the table holds at most two
    ///     graves — one per half of the trail — and <c>INSERT OR REPLACE</c> overwrites the grave in a half when a new party
    ///     dies there, matching the original game's two-record TOMBS.REC behavior. Each row also records the landmarks that
    ///     bracket the death, reproducing the location data the original file format stored.
    /// </summary>
    public sealed class TombstoneStore
    {
        private readonly SqliteConnection _connection;

        public TombstoneStore(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Insert(int trailHalf, int mileMarker, string playerName, string epitaph, string lastLandmark,
            string nextLandmark, int milesToNext)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText =
                "INSERT OR REPLACE INTO tombstones " +
                "(trail_half, mile_marker, player_name, epitaph, last_landmark, next_landmark, miles_to_next) " +
                "VALUES ($half, $mile, $name, $epitaph, $last, $next, $milesToNext);";
            cmd.Parameters.AddWithValue("$half", trailHalf);
            cmd.Parameters.AddWithValue("$mile", mileMarker);
            cmd.Parameters.AddWithValue("$name", playerName ?? string.Empty);
            cmd.Parameters.AddWithValue("$epitaph", epitaph ?? string.Empty);
            cmd.Parameters.AddWithValue("$last", lastLandmark ?? string.Empty);
            cmd.Parameters.AddWithValue("$next", nextLandmark ?? string.Empty);
            cmd.Parameters.AddWithValue("$milesToNext", milesToNext);
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyList<(int TrailHalf, int MileMarker, string PlayerName, string Epitaph, string LastLandmark,
            string NextLandmark, int MilesToNext)> All()
        {
            var list = new List<(int, int, string, string, string, string, int)>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText =
                "SELECT trail_half, mile_marker, player_name, epitaph, last_landmark, next_landmark, miles_to_next " +
                "FROM tombstones;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3),
                    reader.GetString(4), reader.GetString(5), reader.GetInt32(6)));
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
