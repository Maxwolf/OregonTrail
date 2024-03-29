﻿// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Defines a bunch of predefined animal items that can be hunted for food using bullets by the player.
    /// </summary>
    public static class Animals
    {
        /// <summary>
        ///     Gets the bear.
        /// </summary>
        public static SimItem Bear => new(Entities.Food, "Bear", "pounds", "pound", 2000, 0);

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
        ///     Gets the duck.
        /// </summary>
        public static SimItem Duck => new(Entities.Food, "Duck", "pounds", "pound", 2000, 0);

        /// <summary>
        ///     Gets the goose.
        /// </summary>
        public static SimItem Goose => new(Entities.Food, "Goose", "pounds", "pound", 2000, 0, 2);

        /// <summary>
        ///     Gets the rabbit.
        /// </summary>
        public static SimItem Rabbit => new(Entities.Food, "Rabbit", "pounds", "pound", 2000, 0, 2);

        /// <summary>
        ///     Gets the squirrel.
        /// </summary>
        public static SimItem Squirrel => new(Entities.Food, "Squirrel", "pounds", "pound", 2000, 0);
    }
}