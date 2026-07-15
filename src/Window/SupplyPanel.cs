// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using OregonTrailDotNet.Entity;
using WolfCurses.Window.Control;

namespace OregonTrailDotNet.Window
{
    /// <summary>
    ///     Renders the party's wagon inventory as the shared "supplies" table shown on the check-supplies screen, the trading
    ///     screen, and the death summary. Each of those three screens previously rebuilt the identical item-name wording,
    ///     thousands-separator formatting, and <c>ToStringTable</c> call; this centralizes them so the phrasing lives in one
    ///     place. The result is a WolfCurses text table (already a
    ///     self-framed panel), so callers pair it with their own heading rather than wrapping it in another border.
    /// </summary>
    public static class SupplyPanel
    {
        /// <summary>
        ///     Builds the vehicle-supplies table from the current inventory.
        /// </summary>
        /// <param name="includeCash">
        ///     When true the party's remaining money is listed as a row (the status and death screens do this); the trading
        ///     screen passes false because cash is not part of what is being bartered.
        /// </param>
        public static string Build(bool includeCash)
        {
            var suppliesList = new List<Tuple<string, string>>();

            // Loop through every inventory item in the vehicle, giving each a human-facing name and a formatted amount.
            foreach (var item in GameSimulationApp.Instance.Vehicle.Inventory)
            {
                var quantity = item.Value.Quantity.ToString("N0");
                switch (item.Key)
                {
                    case EntitiesEnum.Animal:
                        suppliesList.Add(new Tuple<string, string>("oxen", quantity));
                        break;
                    case EntitiesEnum.Clothes:
                        suppliesList.Add(new Tuple<string, string>("sets of clothing", quantity));
                        break;
                    case EntitiesEnum.Ammo:
                        suppliesList.Add(new Tuple<string, string>("bullets", quantity));
                        break;
                    case EntitiesEnum.Medicine:
                        suppliesList.Add(new Tuple<string, string>("medical kits", quantity));
                        break;
                    case EntitiesEnum.Wheel:
                        suppliesList.Add(new Tuple<string, string>("wagon wheels", quantity));
                        break;
                    case EntitiesEnum.Axle:
                        suppliesList.Add(new Tuple<string, string>("wagon axles", quantity));
                        break;
                    case EntitiesEnum.Tongue:
                        suppliesList.Add(new Tuple<string, string>("wagon tongues", quantity));
                        break;
                    case EntitiesEnum.Food:
                        suppliesList.Add(new Tuple<string, string>("pounds of food",
                            item.Value.TotalWeight.ToString("N0")));
                        break;
                    case EntitiesEnum.Cash:
                        if (includeCash)
                            suppliesList.Add(new Tuple<string, string>("money left",
                                item.Value.TotalValue.ToString("C")));
                        break;
                }

                // Vehicle, Person, and Location keys are not purchasable supplies and are simply not listed.
            }

            // Generate the formatted table of supplies we will show to the user.
            return suppliesList.ToStringTable(
                new[] {"Item Name", "Amount"},
                u => u.Item1,
                u => u.Item2);
        }
    }
}
