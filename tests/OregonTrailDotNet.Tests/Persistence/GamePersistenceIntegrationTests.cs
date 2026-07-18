using System;
using System.IO;
using System.Linq;
using OregonTrailDotNet.Module.Scoring;
using OregonTrailDotNet.Persistence;
using Xunit;

namespace OregonTrailDotNet.Tests.Persistence
{
    /// <summary>
    ///     Drives the real game-simulation singleton lifecycle (create → tick → destroy, exactly as Program.Main does) with
    ///     persistence enabled and pointed at a temp game.db, proving the OnPostCreate/OnPreDestroy wiring actually opens the
    ///     database and that a high score recorded in one session is loaded by the next. Restores the persistence flag and the
    ///     path override on dispose so it can't leak into the other (in-memory) simulation tests.
    /// </summary>
    public sealed class GamePersistenceIntegrationTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gamedbint_{Guid.NewGuid():N}.db");
        private readonly string _previousEnv;


        public GamePersistenceIntegrationTests()
        {
            _previousEnv = Environment.GetEnvironmentVariable(GameDatabase.PathEnvVar);
            Environment.SetEnvironmentVariable(GameDatabase.PathEnvVar, _dbPath);
            GameSimulationApp.PersistenceEnabled = true;
        }

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
            GameSimulationApp.PersistenceEnabled = false;
            Environment.SetEnvironmentVariable(GameDatabase.PathEnvVar, _previousEnv);
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + suffix))
                    File.Delete(_dbPath + suffix);
        }

        [Fact]
        public void High_Score_Added_In_One_Session_Survives_Into_The_Next()
        {
            // First session: boot the singleton with persistence on, record a score, shut it down (which disposes the db).
            GameSimulationApp.Create();
            GameSimulationApp.Instance.OnTick(false); // OnFirstTick -> Restart creates the per-game modules, so Destroy is clean
            GameSimulationApp.Instance.OnTick(false);
            Assert.NotNull(GameSimulationApp.Instance.Database); // game.db opened at the temp path
            GameSimulationApp.Instance.Scoring.Add(new Highscore("Persisted Pioneer", 8123));
            GameSimulationApp.Instance.Destroy();

            // Second session: a fresh singleton loads the saved score from the same file.
            GameSimulationApp.Create();
            GameSimulationApp.Instance.OnTick(false);
            GameSimulationApp.Instance.OnTick(false);
            var top = GameSimulationApp.Instance.Scoring.TopTen.ToList();
            Assert.Contains(top, s => s.Name == "Persisted Pioneer" && s.Points == 8123);
            GameSimulationApp.Instance.Destroy();
        }
    }
}
