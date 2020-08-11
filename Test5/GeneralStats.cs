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
        public float totalWins;
        public float totalGames;
        public float totalAssaultGames;
        public float totalAssaultWins;
        public float totalHybridWins;
        public float totalHybridGames;
        public float totalEscortGames;
        public float totalEscortWins;
        public float totalControlWins;
        public float totalControlGames;
        public float totalDefendGames;
        public float totalDefendWins;
        public float totalAttackWins;
        public float totalAttackGames;
        
        public void ResetGeneralStats()
        {
            GeneralStats gs = new GeneralStats();
            gs.totalWins = 0;
            gs.totalGames = 0;
            gs.totalAssaultGames = 0;
            gs.totalAssaultWins = 0;
            gs.totalHybridWins = 0;
            gs.totalHybridGames = 0;
            gs.totalEscortGames = 0;
            gs.totalEscortWins = 0;
            gs.totalControlWins = 0;
            gs.totalControlGames = 0;
            gs.totalDefendGames = 0;
            gs.totalDefendWins = 0;
            gs.totalAttackWins = 0;
            gs.totalAttackGames = 0;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("GeneralStatsBin", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            formatter.Serialize(stream, gs);
            stream.Close();
        }
    }

    

}
