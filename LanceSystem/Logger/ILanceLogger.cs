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
    }
    public class InfoLogger : ILanceLogger
    {
        public void Warning(string message)
        {
            InformationManager.DisplayMessage(new(message));
        }
    }
}