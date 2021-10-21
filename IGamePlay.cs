using Nvidia.AtpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlayRun
{
    public interface IGamePlay
    {
        bool IsGameInstalled();
        bool LaunchGame(ResultAgent resultAgent);
        bool StartPlayback(Dictionary<string, int> settings, ResultAgent resultAgent, bool isBenchMark= false);
        bool StartPlayerAI();
        bool ExitGame();
        void PerformDesktopView();
        void PerformFullScreenToggle();
    }

    public abstract class GamePlayFactory
    {
        public abstract IGamePlay GetGame(string gameType, Log log);
    }

    public class GamePlayFactoryCreator : GamePlayFactory
    {
        public override IGamePlay GetGame(string gameType, Log log)
        {
            switch (gameType)
            {
                case "ForzaHorizon4": return new FrozaHorizon4(log);
                case "CSGO": return new CounterStrikeGlobalOffensive(log);
                case "Overwatch": return new Overwatch(log);
                default: throw new ArgumentException("Invalid gameType", "gameType");
            }
        }
    }
}
