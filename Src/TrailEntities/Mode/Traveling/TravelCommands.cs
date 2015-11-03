namespace TrailEntities
{
    /// <summary>
    ///     All of the commands associated with the traveling game mode which is one of the primary simulation game modes since
    ///     it is at the bottom of the game modes stack and all others will be stacking on top of it.
    /// </summary>
    public enum TravelCommands
    {
        /// <summary>
        ///     If the simulation is paused and the player is on the traveling screen this will restart it back into whatever pace
        ///     they had it set to previously.
        /// </summary>
        ContinueOnTrail = 1,

        /// <summary>
        ///     "Status" tells you the medical conditions of everyone in your party, as well as your current inventory and
        ///     Occupation.
        /// </summary>
        CheckSupplies = 2,

        /// <summary>
        ///     "Map" shows you your current progress across the country, as well as some major landmarks, and your cute little
        ///     oxen running to nowhere in particular.
        /// </summary>
        LookAtMap = 3,

        /// <summary>
        ///     There are three settings for Pace
        /// </summary>
        ChangePace = 4,

        /// <summary>
        ///     "Rations" is where you can set how much your party eats
        /// </summary>
        ChangeFoodRations = 5,

        /// <summary>
        ///     Resting often improves/restores the health of a sick party member.  Resting is helpful, but if you do it too much,
        ///     you'll find yourself traveling through tough winter weather in the end of the game.
        /// </summary>
        StopToRest = 6,

        /// <summary>
        ///     "Trade" is a very useful feature.  You can often get items you need for cheap. Simply enter the item you wish to
        ///     trade for, and the number of them, and someone will offer you a trade.  If you don't like the trade, you can
        ///     "Haggle" with them in an attempt to get a better deal.  The more haggling you do with a person, the more their
        ///     prices will slowly be driven up.  If you haggle too high, simply exit the trade screen, continue on the trail (and
        ///     distance, as long as you've moved), then attempt to trade again.
        /// </summary>
        AttemptToTrade = 7,

        /// <summary>
        ///     Some locations along the trail offer up the chance to hunt for food using bullets, other situations require only
        ///     shooting and don't reward with animal meat only defending yourself from bandits or wild animals.
        /// </summary>
        HuntForFood = 8,

        /// <summary>
        ///     using "Talk," you can talk to fellow travelers to further the story or for advice.  If you don't know what to do,
        ///     talk to someone or consult the Guide.
        /// </summary>
        TalkToPeople = 9,

        /// <summary>
        ///     You can only buy items at forts along the trail.  If you're at a fort, click "Buy" to see what is in stock.  Prices
        ///     increase the farther along the trail you go.
        /// </summary>
        BuySupplies = 10
    }
}