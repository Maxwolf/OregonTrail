// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventProduct.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Represents an event that can be triggered by the event director when vehicle is traveling along the trail.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Represents an event that can be triggered by the event director when vehicle is traveling along the trail.
    /// </summary>
    public abstract class EventProduct
    {
        /// <summary>
        ///     Grabs the current name of the event as it should be known by the simulation. Generally this is the friendly class
        ///     name.
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="userData">
        /// Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public abstract void Execute(RandomEventInfo userData);

        /// <summary>
        /// Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        /// Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        /// <returns>
        /// Text user interface string that can be used to explain what the event did when executed.
        /// </returns>
        public string Render(RandomEventInfo userData)
        {
            return OnRender(userData);
        }

        /// <summary>
        /// Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        /// Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        /// <returns>
        /// Text user interface string that can be used to explain what the event did when executed.
        /// </returns>
        protected abstract string OnRender(RandomEventInfo userData);

        /// <summary>
        ///     Fired when the event is closed by the user or system after being executed and rendered out on text user interface.
        /// </summary>
        public virtual void OnEventClose()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired when the event is created by the event factory, but before it is executed. Acts as a constructor mostly but
        ///     used in this way so that only the factory will call the method and there is no worry of it accidentally getting
        ///     called by creation.
        /// </summary>
        public virtual void OnEventCreate()
        {
            // Nothing to see here, move along...
        }
    }
}