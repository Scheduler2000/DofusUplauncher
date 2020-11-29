using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Uplauncher.Managers
{
    public class ProcessManager
    {
        private readonly ProcessStartInfo _reg;
        private bool _regIsInitialized;

        public ProcessStartInfo Site { get; }

        public ProcessStartInfo Forum { get; }

        public ProcessStartInfo Vote { get; }

        public ProcessStartInfo Game { get; }



        public ProcessManager()
        {
            var folder = Directory.GetCurrentDirectory();

            this.Site = new ProcessStartInfo(Constants.SITE_URI) { UseShellExecute = true };
            this.Forum = new ProcessStartInfo(Constants.FORUM_URI) { UseShellExecute = true };
            this.Vote = new ProcessStartInfo(Constants.VOTE_URI) { UseShellExecute = true };
            this.Game = new ProcessStartInfo($"{folder}/{Constants.LOCAL_GAME_URI}") { UseShellExecute = true };
            this._reg = new ProcessStartInfo($"{folder}/{Constants.LOCAL_REG_URI}") { UseShellExecute = true };
        }

        public void OpenSite()
        { Process.Start(Site); }

        public void OpenForum()
        { Process.Start(Forum); }

        public void OpenVote()
        { Process.Start(Vote); }

        public void OpenGame()
        {
            if (!_regIsInitialized)
                OpenReg();

            Process.Start(Game);
        }

        private void OpenReg()
        {
            Process.Start(_reg);
            _regIsInitialized = true;
        }
    }
}
