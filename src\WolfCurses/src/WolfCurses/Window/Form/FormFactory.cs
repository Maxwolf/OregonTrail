// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit.Form
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Keeps track of all the possible states a given game Windows can have by using attributes and reflection to keep
    ///     track of which user data object gets mapped to which particular state.
    /// </summary>
    public sealed class FormFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SimUnit.Form.FormFactory" /> class.
        /// </summary>
        public FormFactory()
        {
            // Create dictionaries for reference tracking for what states belong to what game modes.
            LoadedForms = new Dictionary<Type, Type>();

            // Collect all of the states with the custom attribute decorated on them.
            var foundStates = AttributeExtensions.GetTypesWith<ParentWindowAttribute>(false);
            foreach (var stateType in foundStates)
            {
                // GetModule the attribute itself from the state we are working on, which gives us the game Windows enum.
                var stateAttribute = stateType.GetAttributes<ParentWindowAttribute>(false).First();
                var stateParentMode = stateAttribute.ParentWindow;

                // Add the state reference list for lookup and instancing later during runtime.
                LoadedForms.Add(stateType, stateParentMode);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the reflected state types.
        /// </summary>
        private Dictionary<Type, Type> LoadedForms { get; set; }

        /// <summary>Creates and adds the specified type of state to currently active game Windows.</summary>
        /// <param name="stateType">Role object that is the actual type of state that needs created.</param>
        /// <param name="activeMode">Current active game Windows passed to factory so no need to call game simulation singleton.</param>
        /// <returns>Created state instance from reference types build on startup.</returns>
        public IForm CreateStateFromType(Type stateType, IWindow activeMode)
        {
            // Check if the state exists in our reference list.
            if (!LoadedForms.ContainsKey(stateType))
                throw new ArgumentException(
                    "State factory cannot create state from type that does not exist in reference states! " +
                    "Perhaps developer forgot [RequiredMode] attribute on state?!");

            // States are based on abstract class, but never should be one.
            if (stateType.IsAbstract)
                return null;

            // Create the state, it will have constructor with one parameter.
            var stateInstance = Activator.CreateInstance(stateType, activeMode);

            // Pass the created state back to caller.
            return stateInstance as IForm;
        }

        /// <summary>
        ///     Called when primary simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            LoadedForms.Clear();
            LoadedForms = null;
        }
    }
}