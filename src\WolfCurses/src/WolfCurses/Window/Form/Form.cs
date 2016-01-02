// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit.Form
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Forms are attached to windows.</summary>
    /// <typeparam name="TData">UserData type.</typeparam>
    public abstract class Form<TData> :
        Comparer<Form<TData>>,
        IComparable<Form<TData>>,
        IEquatable<Form<TData>>,
        IEqualityComparer<Form<TData>>,
        IForm
        where TData : WindowData, new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Form{TData}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        protected Form(IWindow window)
        {
            ParentWindow = window;
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        public TData UserData
        {
            get { return ParentWindow.UserData as TData; }
        }

        /// <summary>
        ///     Current parent game Windows which this state is binded to and is doing work on behalf of.
        /// </summary>
        protected IWindow ParentWindow { get; }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Form<TData> other)
        {
            return Compare(this, other);
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(Form<TData> x, Form<TData> y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(Form<TData> obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Form<TData> other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return UserData.Equals(other.UserData) &&
                   ParentWindow.Equals(other.ParentWindow);
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        WindowData IForm.UserData
        {
            get { return UserData; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public virtual bool InputFillsBuffer
        {
            get { return !ParentWindow.ShouldRemoveMode; }
        }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public virtual bool AllowInput
        {
            get { return !ParentWindow.ShouldRemoveMode; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The text user interface.<see cref="string" />.
        /// </returns>
        public abstract string OnRenderForm();

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public abstract void OnInputBufferReturned(string input);

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public virtual void OnFormPostCreate()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired when the window is activated and or refocused after another window was removed from being on-top of it.
        ///     Useful for re-initializing form data after something like a random event runs which might kill people or alter the
        ///     vehicle inventory.
        /// </summary>
        public virtual void OnFormActivate()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the
        ///     other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(IForm x, IForm y)
        {
            var result = string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
            if (result != 0)
                return result;

            return result;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IForm other)
        {
            return string.Compare(other.GetType().Name, GetType().Name, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public virtual void OnTick(bool systemTick, bool skipDay)
        {
            // Nothing to see here, move along...
        }

        /// <summary>Creates and adds the specified type of state to currently active game Windows.</summary>
        /// <param name="stateType">The state Type.</param>
        public void SetForm(Type stateType)
        {
            // Pass the state wanted to the parent game Windows.
            ParentWindow.SetForm(stateType);
        }

        /// <summary>
        ///     Removes the current state from the active game Windows.
        /// </summary>
        protected void ClearForm()
        {
            // Refers to parent game Windows to actually clear the state.
            ParentWindow.ClearForm();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        ///     When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
        ///     indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override int Compare(Form<TData> x, Form<TData> y)
        {
            var result = string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
            return result;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 23;
            hash = (hash*31) + UserData.GetHashCode();
            hash = (hash*31) + ParentWindow.GetHashCode();
            return hash;
        }
    }
}