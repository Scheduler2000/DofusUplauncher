using Shadow.Sound.Reg;
using Uplauncher.Sound.Game;

namespace Uplauncher.Managers
{
    public class SoundManager
    {
        public static GameServer GameServer { get; }
        public static RegServer RegServer { get; }


        static SoundManager()
        {
            GameServer = new GameServer();
            RegServer = new RegServer();
        }

        public static void Initialize()
        {
            GameServer.StartAuthentificate();
            RegServer.StartAuthentificate();
        }

    }
}
