using TaleWorlds.Library;

namespace LanceSystem.Logger
{
    public static class LanceLogger
    {
        public static ILanceLogger Logger { get; set; } = new InfoLogger();
    }
    public interface ILanceLogger
    {
        void Warning(string message);
        void Error(string message);
    }
    public class InfoLogger : ILanceLogger
    {
        public void Error(string message)
        {
            InformationManager.DisplayMessage(new(message, new Color(1, 0, 0)));
        }

        public void Warning(string message)
        {
            InformationManager.DisplayMessage(new(message, new Color(1, 1, 0)));
        }
    }
}