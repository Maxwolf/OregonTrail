using System;

namespace TrailCommon
{
    /// <summary>
    ///     Represents an even that can occur to player, vehicle, or triggered by outside influences such as climate.
    /// </summary>
    public abstract class EventItem<Thing, Verb, Noun> : IEventItem
        where Thing : struct, IComparable, IFormattable, IConvertible
        where Verb : struct, IComparable, IFormattable, IConvertible
        where Noun : struct, IComparable, IFormattable, IConvertible
    {
        protected EventItem(Thing targetThing, Verb actionVerb, Noun resultNoun, Date when)
        {
            // Complain the generics implemented are not of an enum type.
            if (!typeof (Thing).IsEnum || !typeof (Verb).IsEnum || !typeof (Noun).IsEnum)
            {
                throw new InvalidCastException("Event item generic types are only meant to be used with enumerations!");
            }

            TargetThing = targetThing;
            ActionVerb = actionVerb;
            ResultNoun = resultNoun;
            Timestamp = when;
        }

        public Thing TargetThing { get; set; }
        public Verb ActionVerb { get; set; }
        public Noun ResultNoun { get; set; }
        public Date Timestamp { get; set; }

        object IEventItem.TargetThing
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        object IEventItem.ActionVerb
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        object IEventItem.ResultNoun
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }
    }
}