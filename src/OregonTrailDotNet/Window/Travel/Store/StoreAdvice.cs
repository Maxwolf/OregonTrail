// Created by Maxwolf (bigmaxwolf.com)

using System;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;

namespace OregonTrailDotNet.Window.Travel.Store
{
    /// <summary>
    ///     What Matt tells you about a good before you name a quantity, and the running bill under the question.
    ///     <para>
    ///         These are the 1990 DOS port's own words, recovered from <c>legacy/dos/OREGON.UNPACKED.EXE</c>. They are
    ///         not decoration: the advice is how a first-time player learns what a survivable loadout even looks like
    ///         ("at least 200 pounds of food for each person", "at least 3 yoke"), which is the whole reason the
    ///         original put it in front of the prompt rather than in a manual.
    ///     </para>
    ///     <para>
    ///         Independence only. Out on the trail the forts quote a bare price and ask - no advice, no running bill -
    ///         and that difference is the original's too.
    ///     </para>
    /// </summary>
    internal static class StoreAdvice
    {
        /// <summary>
        ///     Matt's pitch for one good, or null where he has nothing to say (out on the trail, or for the medical
        ///     supplies that are ours rather than MECC's).
        /// </summary>
        /// <param name="item">The good the player just asked about.</param>
        internal static string For(SimItem item)
        {
            if (item == null || !StoreCounter.AtMatts)
                return null;

            switch (item.Category)
            {
                case EntitiesEnum.Animal:
                    return $"There are {item.LotSize} oxen in a yoke;{Environment.NewLine}" +
                           $"I recommend at least 3 yoke.{Environment.NewLine}" +
                           $"I charge {item.Cost*item.LotSize:C2} a yoke.";

                case EntitiesEnum.Food:
                    // The original names the party size here, and its own copy hardcodes five because that is the
                    // party it always had. Ours reads the real roster, which is the same number in a full game.
                    var mouths = GameSimulationApp.Instance.Vehicle.Passengers.Count;
                    var people = mouths == 1 ? "1 person" : $"{mouths} people";
                    return $"I recommend you take at least 200 pounds{Environment.NewLine}" +
                           $"of food for each person in your family.{Environment.NewLine}" +
                           $"I see that you have {people} in all. You'll{Environment.NewLine}" +
                           $"need flour, sugar, bacon, and coffee.{Environment.NewLine}" +
                           $"My price is {item.Cost:C2} a pound.";

                case EntitiesEnum.Clothes:
                    return $"You'll need warm clothing in the mountains.{Environment.NewLine}" +
                           $"I recommend taking at least 2 sets of{Environment.NewLine}" +
                           $"clothes per person. Each set is {item.Cost:C2}.";

                case EntitiesEnum.Ammo:
                    return $"I sell ammunition in boxes of {item.LotSize}{Environment.NewLine}" +
                           $"bullets. Each box costs {item.Cost*item.LotSize:C2}.";

                case EntitiesEnum.Wheel:
                case EntitiesEnum.Axle:
                case EntitiesEnum.Tongue:
                    return $"It's a good idea to have a few spare{Environment.NewLine}" +
                           $"parts for your wagon. A wagon wheel, axle{Environment.NewLine}" +
                           $"or tongue is {item.Cost:C2} each.";

                default:
                    return null;
            }
        }

        /// <summary>
        ///     The counter's running total, as the original pinned it under every purchase screen at Matt's, or null
        ///     out on the trail where there is no tab to run.
        /// </summary>
        /// <param name="store">The pending receipt.</param>
        internal static string RunningBill(StoreGenerator store)
        {
            if (store == null || !StoreCounter.AtMatts)
                return null;

            return $"Bill so far: {store.TotalTransactionCost:C2}";
        }
    }
}
