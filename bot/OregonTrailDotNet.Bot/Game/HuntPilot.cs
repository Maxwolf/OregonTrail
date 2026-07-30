using OregonTrailDotNet.Presentation;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Works the rifle on the field hunt - the same hunt, on the same <see cref="HuntGame" />, that a player
    ///     works by eye. This is marksmanship, not strategy: it decides where to point and when to squeeze, exactly
    ///     the motor skill a person supplies with their hands. Whether to hunt at all, and when the bag is enough to
    ///     walk away, stay with the policy (<see cref="HuntStrategy" />) so the learned part of the bot keeps making
    ///     the decisions a person actually deliberates over.
    ///     <para>
    ///         It aims by casting the bullet's own path - the original steps a round <c>AimVectors[Aim] * 2</c> per
    ///         tick and stops it dead on scenery - and only fires when that path genuinely reaches the animal. The
    ///         rifle swings one compass point per three ticks the short way round, so pointing early and firing late
    ///         is the whole game.
    ///     </para>
    /// </summary>
    public static class HuntPilot
    {
        /// <summary>Absolute-aim keys, indexed by compass heading (0 = N, clockwise) - the DOS port's keypad.</summary>
        private static readonly ConsoleKey[] AimKeys =
        [
            ConsoleKey.NumPad8, ConsoleKey.NumPad9, ConsoleKey.NumPad6, ConsoleKey.NumPad3,
            ConsoleKey.NumPad2, ConsoleKey.NumPad1, ConsoleKey.NumPad4, ConsoleKey.NumPad7
        ];

        /// <summary>How far ahead a bullet is traced, in ticks. The field is 320 wide and a round covers 4 a tick.</summary>
        private const int TraceTicks = 90;

        /// <summary>
        ///     The key to press this tick, or null to simply let the hunt run on. Null is a real answer and the
        ///     common one: most ticks of a hunt are spent waiting for an animal to wander in or for the rifle to
        ///     finish swinging.
        /// </summary>
        /// <param name="game">The hunt in progress.</param>
        public static ConsoleKey? NextKey(HuntGame game)
        {
            if (game == null || game.Finished || game.Bullets <= 0)
                return null;

            // Let a swing finish before thinking again. The rifle turns one compass point per three ticks, so a
            // half-turn takes a dozen ticks and an animal walks most of its own length in that time; re-deciding
            // every tick means chasing it round the compass and never firing. Commit, then reassess.
            if (game.Aim != game.TargetAim)
                return null;

            // Pick the animal worth the round: the heaviest one there is a clean shot to, since the wrapper caps
            // the whole bag at a hundred pounds and a bison ends the hunt on its own.
            var bestHeading = -1;
            var bestValue = -1;

            foreach (var animal in game.Animals)
            {
                if (!animal.Active)
                    continue;

                var heading = HeadingThatHits(game, animal);
                if (heading < 0)
                    continue;

                var value = HuntGame.Species[animal.Species].MaximumPounds;
                if (value <= bestValue)
                    continue;

                bestValue = value;
                bestHeading = heading;
            }

            // Nothing reachable. Point at whatever is on the field anyway so the swing is already made when it
            // walks into the open - the rifle takes three ticks a step and a late start is a missed animal.
            if (bestHeading < 0)
            {
                var waiting = FirstActive(game);
                if (waiting < 0)
                    return null;

                var approximate = Compass(game, game.Animals[waiting]);
                return game.Aim == approximate ? null : AimKeys[approximate];
            }

            // Already pointing the right way: squeeze, unless the one round the original allows is still in the air.
            if (game.Aim == bestHeading)
                return game.Shot.Active ? null : ConsoleKey.Spacebar;

            return AimKeys[bestHeading];
        }

        private static int FirstActive(HuntGame game)
        {
            for (var i = 0; i < game.Animals.Length; i++)
                if (game.Animals[i].Active)
                    return i;

            return -1;
        }

        /// <summary>
        ///     The heading whose bullet actually reaches this animal, or -1 if none does. Traced rather than
        ///     estimated, because the field's scenery is solid to a round and an animal standing behind a tree is
        ///     simply not a shot - the same thing a player learns by watching one round after another stop short.
        /// </summary>
        private static int HeadingThatHits(HuntGame game, HuntGame.Animal animal)
        {
            var info = HuntGame.Species[animal.Species];

            for (var heading = 0; heading < 8; heading++)
            {
                var (vx, vy) = HuntGame.AimVectors[heading];

                // Where the round starts and how it steps, straight out of HuntGame.Fire.
                var x = game.HunterX + 10;
                var y = game.HunterY + 12;
                var stepX = vx * 2;
                var stepY = vy * 2;

                for (var tick = 1; tick <= TraceTicks; tick++)
                {
                    x += stepX;
                    y += stepY;

                    if (x < 0 || y < 0 || x >= HuntGame.FieldWidth || y >= HuntGame.FieldHeight)
                        break;
                    if (game.IsBlocked(x, y, 1, 1))
                        break;

                    // Lead the animal: it walks one pixel a tick the way it faces, and by the time the round gets
                    // there it has moved. The original's own hit test is this rectangle overlap.
                    var ax = animal.X + animal.Facing * tick;
                    if (x >= ax && x < ax + info.Width && y >= animal.Y && y < animal.Y + info.Height)
                        return heading;
                }
            }

            return -1;
        }

        /// <summary>The rough compass bearing to an animal, for pre-aiming at one there is no clean shot to yet.</summary>
        private static int Compass(HuntGame game, HuntGame.Animal animal)
        {
            var info = HuntGame.Species[animal.Species];
            var dx = animal.X + info.Width / 2 - (game.HunterX + 10);
            var dy = animal.Y + info.Height / 2 - (game.HunterY + 12);

            // The aim vectors are 2:1, so compare on that footing before deciding a bearing is "mostly sideways".
            var horizontal = Math.Abs(dx) > Math.Abs(dy) * 2;
            var vertical = Math.Abs(dy) * 2 > Math.Abs(dx);

            if (horizontal && dx > 0) return 2;
            if (horizontal) return 6;
            if (vertical && dy < 0) return 0;
            if (vertical) return 4;
            if (dx > 0) return dy < 0 ? 1 : 3;
            return dy < 0 ? 7 : 5;
        }
    }
}
