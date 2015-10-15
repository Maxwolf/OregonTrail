namespace TrailCommon
{
    public interface IGameWindow
    {
        void OpenWindow(IGameWindow window);
        void CloseWindow(IGameWindow window);

        event OpenWindow OpenWindowEvent;
        event CloseWindow CloseWindowEvent;

        IVehicle Vehicle { get; }
    }

    public delegate void OpenWindow();

    public delegate void CloseWindow();
}