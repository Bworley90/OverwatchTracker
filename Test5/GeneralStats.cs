using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Test5
{
    [Serializable]
    public class GeneralStats
    {
        public float Wins;
        public float Games;
        public float AssaultGames;
        public float AssaultWins;
        public float HybridWins;
        public float HybridGames;
        public float EscortGames;
        public float EscortWins;
        public float ControlWins;
        public float ControlGames;
        public float DefendGames;
        public float DefendWins;
        public float AttackWins;
        public float AttackGames;
        
        public void ResetGeneralStats()
        {
            GeneralStats gs = new GeneralStats();
            gs.Wins = 0;
            gs.Games = 0;
            gs.AssaultGames = 0;
            gs.AssaultWins = 0;
            gs.HybridWins = 0;
            gs.HybridGames = 0;
            gs.EscortGames = 0;
            gs.EscortWins = 0;
            gs.ControlWins = 0;
            gs.ControlGames = 0;
            gs.DefendGames = 0;
            gs.DefendWins = 0;
            gs.AttackWins = 0;
            gs.AttackGames = 0;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("GeneralStats", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            formatter.Serialize(stream, gs);
            stream.Close();
        }
    }

    

}
