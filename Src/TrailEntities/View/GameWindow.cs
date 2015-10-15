using System;
using TrailCommon;

namespace TrailEntities
{
    public abstract class GameWindow : IGameWindow
    {
        protected Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        protected GameWindow(Vehicle vehicle)
        {
            // Complain if game manager does not exist.
            if (GameManager.Instance == null)
                throw new InvalidOperationException("Called game window constructor when game manager is null!");

            _vehicle = vehicle;

            // Hook events that all game windows will want.
            GameManager.Instance.KeypressEvent += Instance_KeypressEvent;
            GameManager.Instance.TickEvent += Instance_TickEvent;
        }

        public void OpenWindow(IGameWindow window)
        {
            OnOpenWindow(window);
        }

        public void CloseWindow(IGameWindow window)
        {
            OnCloseWindow(window);
            GameManager.Instance.KeypressEvent -= Instance_KeypressEvent;
            GameManager.Instance.TickEvent -= Instance_TickEvent;
        }

        public event OpenWindow OpenWindowEvent;
        public event CloseWindow CloseWindowEvent;

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        private void Instance_TickEvent(ulong tickCount)
        {
            OnTick();
        }

        protected virtual void OnTick()
        {
            // Complain if game manager does not exist.
            if (GameManager.Instance == null)
                throw new InvalidOperationException("Unable to continue to tick game window since game manager is null!");
        }

        private void Instance_KeypressEvent(string keyCode)
        {
            OnKeyPress(keyCode);
        }

        protected virtual void OnKeyPress(string keyCode)
        {
            // Complain if game manager does not exist.
            if (GameManager.Instance == null)
                throw new InvalidOperationException("Unable to continue to tick game window since game manager is null!");
        }

        protected virtual void OnOpenWindow(IGameWindow window)
        {
            OpenWindowEvent?.Invoke();
        }

        protected virtual void OnCloseWindow(IGameWindow window)
        {
            CloseWindowEvent?.Invoke();
        }
    }
}