using System;
using TrailEntities.Game;
using TrailEntities.Game.RandomEvent;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
    /// </summary>
    public static class ModeFactory
    {
        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="modeCategory">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        public static IMode CreateInstance(ModeCategory modeCategory)
        {
            switch (modeCategory)
            {
                case ModeCategory.Travel:
                    return new TravelMode();
                case ModeCategory.ForkInRoad:
                    return new ForkInRoadMode();
                case ModeCategory.Hunt:
                    return new HuntingMode();
                case ModeCategory.MainMenu:
                    return new MainMenuMode();
                case ModeCategory.RiverCrossing:
                    return new RiverCrossMode();
                case ModeCategory.Store:
                    return new StoreMode();
                case ModeCategory.Trade:
                    return new TradingMode();
                case ModeCategory.Options:
                    return new OptionsMode();
                case ModeCategory.EndGame:
                    return new EndGameMode();
                case ModeCategory.RandomEvent:
                    return new RandomEventMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(modeCategory), modeCategory, null);
            }
        }
    }
}