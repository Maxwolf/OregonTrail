using System;
using System.IO;
using System.Linq;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Enforces the per-tree encoding convention so a bulk edit can't quietly pollute a diff: the game tree
    ///     (src/OregonTrailDotNet) is UTF-8 with BOM; every other tree — Presentation, Minigames, Assets, bot,
    ///     tests — is UTF-8 without. The convention is documented in CLAUDE.md; this is what holds it.
    /// </summary>
    public class EncodingConventionTests
    {
        private static readonly byte[] Bom = [0xEF, 0xBB, 0xBF];

        /// <summary>Walks up from the test binary to the repository root, marked by the solution file.</summary>
        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "OregonTrailDotNet.sln")))
                directory = directory.Parent;
            return directory?.FullName;
        }

        private static bool HasBom(string path)
        {
            using var stream = File.OpenRead(path);
            var head = new byte[3];
            return stream.Read(head, 0, 3) == 3 && head.SequenceEqual(Bom);
        }

        /// <summary>
        ///     What a BOM check alone misses: a UTF-16 save (FF FE / FE FF head) or an ANSI re-save of a file with
        ///     non-ASCII content (em-dashes in comments), both of which are exactly the diff pollution the
        ///     convention exists to prevent. Strict UTF-8 decoding catches the mojibake; the head check the rest.
        /// </summary>
        private static bool IsCleanUtf8(string path)
        {
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length >= 2 && ((bytes[0] == 0xFF && bytes[1] == 0xFE) || (bytes[0] == 0xFE && bytes[1] == 0xFF)))
                return false;

            try
            {
                new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                    .GetString(bytes);
                return true;
            }
            catch (System.Text.DecoderFallbackException)
            {
                return false;
            }
        }

        private static string[] Sources(string root, string tree) =>
            Directory.EnumerateFiles(Path.Combine(root, tree), "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") &&
                               !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                .ToArray();

        [Fact]
        public void TheGameTree_CarriesTheByteOrderMark()
        {
            var root = FindRepositoryRoot();
            Assert.SkipWhen(root == null, "test host is running outside the repository");

            var strays = Sources(root, Path.Combine("src", "OregonTrailDotNet"))
                .Where(path => !HasBom(path) || !IsCleanUtf8(path))
                .Select(path => Path.GetRelativePath(root, path))
                .ToList();

            Assert.True(strays.Count == 0,
                "game-tree files missing the UTF-8 BOM (or not valid UTF-8):\n" + string.Join("\n", strays));
        }

        [Theory]
        [InlineData("src/OregonTrailDotNet.Presentation")]
        [InlineData("src/OregonTrailDotNet.Minigames")]
        [InlineData("src/OregonTrailDotNet.Assets")]
        [InlineData("bot")]
        [InlineData("tests")]
        public void EveryOtherTree_CarriesNone(string tree)
        {
            var root = FindRepositoryRoot();
            Assert.SkipWhen(root == null, "test host is running outside the repository");

            var strays = Sources(root, tree.Replace('/', Path.DirectorySeparatorChar))
                .Where(path => HasBom(path) || !IsCleanUtf8(path))
                .Select(path => Path.GetRelativePath(root, path))
                .ToList();

            Assert.True(strays.Count == 0,
                $"files in {tree} carrying a UTF-8 BOM (or not valid UTF-8):\n" + string.Join("\n", strays));
        }
    }
}
