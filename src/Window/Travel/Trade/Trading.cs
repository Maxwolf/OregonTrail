// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Vehicle;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Trade
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class Trading : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Representation of text that shows current supplies and the trade offer if one exists.
        /// </summary>
        private static StringBuilder _supplyPrompt;

        /// <summary>
        ///     Determines if the player is able to make the current trade offer with the supplies they have available in their
        ///     vehicles inventory.
        /// </summary>
        private bool _playerCanTrade;

        /// <summary>
        ///     The swap the emigrant the party met today has put to them, or null if nobody wanted to trade. One emigrant
        ///     comes by per visit and makes one offer; there is no picking through a list of them.
        /// </summary>
        private TradeOffer _offer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Trading" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public Trading(IWindow window) : base(window)
        {
            _supplyPrompt = new StringBuilder();
        }

        /// <summary>
        ///     The trade currently on the table, or null when nobody wants to trade. Exposed read-only for the headless bot,
        ///     the same way <see cref="Travel.ActiveHunt" /> exposes the live hunt.
        /// </summary>
        public TradeOffer CurrentOffer => _offer;

        /// <summary>
        ///     Whether the player holds the full quantity the current offer demands (the Y/N prompt only shows when true).
        /// </summary>
        public bool PlayerCanTrade => _playerCanTrade;

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogTypeEnum DialogType
        {
            get
            {
                // Dialog type is determined by players ability to trade against the generated offer.
                if ((_offer != null) && _playerCanTrade)
                    return DialogTypeEnum.YesNo;

                return DialogTypeEnum.Prompt;
            }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            UpdateTrade();

            // Returns the completed table of supplies and selected trade offer.
            return _supplyPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the window is activated and or refocused after another window was removed from being on-top of it.
        ///     Useful for re-initializing form data after something like a random event runs which might kill people or alter the
        ///     vehicle inventory.
        /// </summary>
        public override void OnFormActivate()
        {
            base.OnFormActivate();

            UpdateTrade();
        }

        /// <summary>
        ///     Creates a new possible trade for the player, or not.
        /// </summary>
        private void UpdateTrade()
        {
            // Tick the people, but not the trail or the day. Passing false here advanced a real calendar day every
            // time the trade screen opened or refocused (unlike CheckSupplies, which correctly skips the day). Merely
            // looking over an offer is free; it is accepting one that costs the day, which OnDialogResponse charges.
            GameSimulationApp.Instance.TakeTurn(true);

            // Grabs all the data for the player current vehicle inventory (cash is excluded — it is not bartered).
            _supplyPrompt.Clear();
            _supplyPrompt.AppendLine($"{Environment.NewLine}Your Supplies{Environment.NewLine}");
            _supplyPrompt.AppendLine(SupplyPanel.Build(includeCash: false));

            // One emigrant comes by, and either has something to propose or does not.
            _offer = GenerateOffer();

            // Check the player actually holds the full quantity the trader is demanding, not merely the item's minimum.
            // ContainsItem only compares against MinQuantity, so it would green-light a trade the player cannot honor,
            // letting them hand over less than agreed while still receiving the full offered item.
            _playerCanTrade = (_offer != null) &&
                              (GameSimulationApp.Instance.Vehicle.Inventory[_offer.WantedItem.Category].Quantity >=
                               _offer.WantedItem.Quantity);

            if (_offer != null)
            {
                // Generates the default prompt for trading that is shown if you have items to trade back or not.
                var wantedItem = _offer.WantedItem;
                var offeredItem = _offer.OfferedItem;
                var wrapText =
                    $"You meet another emigrant who wants {wantedItem.ToQuantityString(wantedItem.Quantity)}. " +
                    $"He will trade you {offeredItem.ToQuantityString(offeredItem.Quantity)}.";

                // Depending if the player has enough of what the trader wants we change up last part of message.
                _supplyPrompt.Append(_playerCanTrade
                    ? $"{wrapText.WordWrap()}{Environment.NewLine}Are you willing to trade? Y/N"
                    : $"{wrapText.WordWrap()}{Environment.NewLine}You don't have this.{Environment.NewLine}{Environment.NewLine}");
            }
            else
            {
                // Nobody came by today, or the one who did wanted to hand over goods the wagon has no room for.
                _supplyPrompt.AppendLine($"No one wants to trade with you today.{Environment.NewLine}");
            }
        }

        /// <summary>
        ///     Works out what the emigrant the party met today is proposing, if anything. Most days somebody has something;
        ///     one day in twenty nobody does. An offer of goods the wagon cannot hold is quietly dropped rather than shown,
        ///     because taking it would mean handing over the party's side and watching the overflow of the other side be
        ///     thrown away - which is why a party already carrying three of every spare part is so often told nobody wants
        ///     to trade.
        /// </summary>
        /// <returns>The swap on the table, or NULL when there is none.</returns>
        private static TradeOffer GenerateOffer()
        {
            if (GameSimulationApp.Instance.Random.NextDouble() > 0.95)
                return null;

            var offer = TradeOffer.Generate();
            if (offer == null)
                return null;

            var inventory = GameSimulationApp.Instance.Vehicle.Inventory;
            var offered = offer.OfferedItem;
            if (inventory[offered.Category].Quantity + offered.Quantity > offered.MaxQuantity)
                return null;

            return offer;
        }

        /// <summary>
        ///     Sets a context-specific input prompt in place of the generic "What is your choice?", then renders as normal.
        /// </summary>
        public override string OnRenderForm()
        {
            ParentWindow.PromptText = "Yes or no?";
            return base.OnRenderForm();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            switch (reponse)
            {
                case DialogResponseEnum.Yes:
                {
                    // Remove the quantity of item from the vehicle inventory the trader wants.
                    GameSimulationApp.Instance.Vehicle.Inventory[_offer.WantedItem.Category].ReduceQuantity(
                        _offer.WantedItem.Quantity);

                    // Give the vehicle the item the trade said he would.
                    GameSimulationApp.Instance.Vehicle.Inventory[_offer.OfferedItem.Category].AddQuantity(
                        _offer.OfferedItem.Quantity);

                    // A wagon sitting on a broken part is repaired on the spot when the party now holds a matching
                    // spare — completing the "trade for the part you need" rescue the stranding screens point toward.
                    var vehicle = GameSimulationApp.Instance.Vehicle;
                    if ((vehicle.BrokenPart != null) && vehicle.TryUseSparePart())
                        vehicle.BrokenPart = null;

                    // Checks if the player has animals to pull their vehicle.
                    GameSimulationApp.Instance.Vehicle.Status =
                        GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Animal].Quantity <= 0
                            ? VehicleStatusEnum.Disabled
                            : VehicleStatusEnum.Moving;

                    // Striking the deal costs a day: the party eats, and winter comes a day closer. Browsing offers is
                    // free, but every accepted trade burns a day exactly as it did in the original. Without this the
                    // trade screen is an unlimited free reroll, since re-entering it regenerates the offer at no cost.
                    GameSimulationApp.Instance.TakeTurn(false);

                    // Return to the travel menu.
                    ClearForm();
                    return;
                }
                case DialogResponseEnum.Custom:
                case DialogResponseEnum.No:
                {
                    // Return to the travel menu.
                    ClearForm();
                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}