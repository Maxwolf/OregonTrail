using System.Collections.Generic;
using System.Diagnostics;

namespace TrailEntities
{
    /// <summary>
    ///     Default events based on category.
    /// </summary>
    public static class Events
    {
        public static IEnumerable<TravelEvent> Medical
        {
            get
            {
                var medicalEvents = new List<TravelEvent>
                {
                    new TravelEvent(EventCategory.Medical, "", () => Debug.Print("lawl"))
                };
                return medicalEvents;
            }
        }
    }
}