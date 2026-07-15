// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Defines a bunch of predefined animal items that can be hunted for food using bullets by the player.
    ///     The seventh constructor argument is the pounds of meat the animal yields when killed — leaving it off silently
    ///     defaults the yield to 1 lb (the bug that once made a bear worth the same as a squirrel), so every animal below
    ///     sets it explicitly.
    /// </summary>
    public static class Animals
    {
        /// <summary>
        ///     Gets the bear. Big game — yields 150 lb of meat, so it always reads as a "giant" kill. Kept a fixed weight
        ///     (like the deer/goose/rabbit below) rather than a random range on purpose: the animal getters run off the
        ///     shared game randomizer, and adding a draw here shifts every downstream random roll, which perturbs the
        ///     deterministic seeded games the training bot relies on.
        /// </summary>
        public static SimItem Bear => new(Entities.Food, "Bear", "pounds", "pound", 2000, 0, 150);

        /// <summary>
        ///     You must use *all* the buffalo...
        /// </summary>
        public static SimItem Buffalo => new(Entities.Food, "Buffalo", "pounds", "pound", 2000, 0,
            GameSimulationApp.Instance.Random.Next(350, 500));

        /// <summary>
        ///     Gets the caribou.
        /// </summary>
        public static SimItem Caribou => new(Entities.Food, "Caribou", "pounds", "pound", 2000, 0,
            GameSimulationApp.Instance.Random.Next(300, 350));

        /// <summary>
        ///     Gets the deer.
        /// </summary>
        public static SimItem Deer => new(Entities.Food, "Deer", "pounds", "pound", 2000, 0, 50);

        /// <summary>
        ///     Gets the duck. Small game — yields 2 lb of meat.
        /// </summary>
        public static SimItem Duck => new(Entities.Food, "Duck", "pounds", "pound", 2000, 0, 2);

        /// <summary>
        ///     Gets the goose.
        /// </summary>
        public static SimItem Goose => new(Entities.Food, "Goose", "pounds", "pound", 2000, 0, 2);

        /// <summary>
        ///     Gets the rabbit.
        /// </summary>
        public static SimItem Rabbit => new(Entities.Food, "Rabbit", "pounds", "pound", 2000, 0, 2);

        /// <summary>
        ///     Gets the squirrel. The smallest game — yields 1 lb of meat.
        /// </summary>
        public static SimItem Squirrel => new(Entities.Food, "Squirrel", "pounds", "pound", 2000, 0, 1);
    }
}