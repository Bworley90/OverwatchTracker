using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


        // Add role to datatable and fill row 
    public partial class MainWindow : Window
    {
        DataTable dt = new DataTable();
        GeneralStats gs = new GeneralStats();
        TankData td = new TankData();
        public int index = 1;
        //public float winCount = 0;
        //public float totalGames;

        // Role Variables
        public float dpsTotalGames = 0;
        public float dpsWins = 0;
        

       

        public float healsTotalGames = 0;
        public float healsWins = 0;


        // Map Lists
        List<string> maps = new List<string>();

        List<string> EscortMaps = new List<string>();
        List<string> AssaultMaps = new List<string>();
        List<string> ControlMaps = new List<string>();
        List<string> HybridMaps = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            SetupTable();
            AddAllMaps();
            //LoadGeneralStatsFromBin();
            //UpdateGeneralStats();
        }


       


        private void CheckBoxesActive(bool isActive)
        {
            if(isActive)
            {
                attackCheck.IsEnabled = true;
                defendCheck.IsEnabled = true;
            }
            else
            {
                attackCheck.IsEnabled = false;
                defendCheck.IsEnabled = false;
            }
        }


        #region Map Setup
        private void AddAllMaps()
        {
            EscortMapsSetup();
            AssaultMapsSetup();
            ControlMapsSetup();
            HybridMapsSetup();
        }

        
        private void EscortMapsSetup()
        {
            EscortMaps.Add("Dorado");
            EscortMaps.Add("Havana");
            EscortMaps.Add("Junkertown");
            EscortMaps.Add("Rialto");
            EscortMaps.Add("Route 66");
            EscortMaps.Add("Watchpoint: Gibralter");
        }

        private void AssaultMapsSetup()
        {
            AssaultMaps.Add("Hanamura");
            AssaultMaps.Add("Horizon Lunar Colony");
            AssaultMaps.Add("Paris");
            AssaultMaps.Add("Temple of Anubus");
            AssaultMaps.Add("Volskaya Industries");
        }

        private void ControlMapsSetup()
        {
            ControlMaps.Add("Busan");
            ControlMaps.Add("Ilios");
            ControlMaps.Add("Lijiang Tower");
            ControlMaps.Add("Nepal");
            ControlMaps.Add("Oasis");
        }

        private void HybridMapsSetup()
        {
            HybridMaps.Add("Blizzard World");
            HybridMaps.Add("Eichenwalde");
            HybridMaps.Add("Hollywood");
            HybridMaps.Add("King's Row");
            HybridMaps.Add("Numbani");
        }

        private void UpdateMapsMenu()
        {
            maps.Clear();
            mapComboBox.Items.Clear();
            if (((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString() == "Hybrid")
            {
                for (int i = 0; i < HybridMaps.Count; i++)
                {
                    maps.Add(HybridMaps[i]);
                    CheckBoxesActive(true);
                }
            }
            else if (((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString() == "Assault")
            {
                for (int i = 0; i < HybridMaps.Count; i++)
                {
                    maps.Add(AssaultMaps[i]);
                    CheckBoxesActive(true);
                }
            }
            else if (((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString() == "Control")
            {
                for (int i = 0; i < ControlMaps.Count; i++)
                {
                    maps.Add(ControlMaps[i]);
                    CheckBoxesActive(false);
                    attackCheck.IsChecked = false;
                    defendCheck.IsChecked = false;
                }

            }
            else if (((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString() == "Escort")
            {
                for (int i = 0; i < EscortMaps.Count; i++)
                {
                    maps.Add(EscortMaps[i]);
                    CheckBoxesActive(true);
                }
            }
            else
            {
                Trace.WriteLine("Map Type Name isn't matching");
            }

            for (int i = 0; i < maps.Count; i++)
            {
                mapComboBox.Items.Add(maps[i]);
            }
        }

        private void mapTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMapsMenu();

        }


        #endregion

        #region DataTable

        public void SetupTable()
        {
            DataColumn matchNumber = new DataColumn("Match #", typeof(int));
            DataColumn mapType = new DataColumn("Map Type", typeof(string));
            DataColumn attack = new DataColumn("Attack", typeof(bool));
            DataColumn defend = new DataColumn("Defend", typeof(bool));
            DataColumn win = new DataColumn("Win", typeof(string));
            DataColumn role = new DataColumn("Role", typeof(string));

            dt.Columns.Add(matchNumber);
            dt.Columns.Add(mapType);
            dt.Columns.Add(attack);
            dt.Columns.Add(defend);
            dt.Columns.Add(win);
            dt.Columns.Add(role); // Here

            dg.ItemsSource = dt.DefaultView;
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = dt.NewRow();

            // Row 0 Match Number
            row[0] = index;



            //Row 1 Map Name
            if (mapTypeComboBox.SelectedItem != null)
            {
                row[1] = ((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString();
            }
            else
            {
                return;
            }



            //Row 2 Attack
            if (attackCheck.IsChecked == true)
            {
                row[2] = true;
                gs.totalAttackGames++;
                if (winCheckBox.IsChecked == true)
                {
                    gs.totalAttackWins++;
                }
            }
            else
            {
                row[2] = false;
                gs.totalAttackGames++;
            }

            //Row 3 Defend

            if (defendCheck.IsChecked == true)
            {
                row[3] = true;
                gs.totalDefendGames++;
                if (winCheckBox.IsChecked == true)
                {
                    gs.totalDefendWins++;
                }
            }
            else
            {
                row[3] = false;
                gs.totalDefendGames++;
            }

            // Row 4 Win Loss

            if (winCheckBox.IsChecked == true)
            {
                row[4] = "Win";
                gs.totalGames++;
                gs.totalWins++;
            }
            else
            {
                row[4] = "Loss";
                gs.totalGames++;
            }

            // Row 5 Role

            if (((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString() == "DPS")
            {
                // DPS
                dpsTotalGames++;
                if (winCheckBox.IsChecked == true)
                {
                    dpsWins++;
                }

            }
            if (((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString() == "DPS")
            {
                // Tank
                //tankTotalGames++;
                if (winCheckBox.IsChecked == true)
                {
                    //tankWins++;
                }
            }
            if (((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString() == "Heals")
            {
                // Heals
                healsTotalGames++;
                if (winCheckBox.IsChecked == true)
                {
                    healsWins++;
                }

            }

            row[5] = ((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString();


            MapType();
            UpdateGeneralStats();
            TankStatsUpdate();
            dt.Rows.Add(row);


            index++;



            TestOne();
            TestTwo();



        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRow();
            if(dt.Rows.Count > 0)
            {
                dt.Rows.RemoveAt(dt.Rows.Count - 1);
            }
            
        }
        public void RemoveRow()
        {
            if(dt.Rows.Count > 0)
            {
                gs.totalGames--;
                if ((string)dt.Rows[dt.Rows.Count - 1][5] == "DPS") // IF DPS
                {
                    dpsTotalGames--;
                    if((string)dt.Rows[dt.Rows.Count -1][4] == "Win")
                    {
                        dpsWins--;
                        gs.totalWins--;
                    }
                }
                else if ((string)dt.Rows[dt.Rows.Count - 1][5] == "Heals") // IF DPS
                {
                    healsTotalGames--;
                    if ((string)dt.Rows[dt.Rows.Count - 1][4] == "Win")
                    {
                        healsWins--;
                        gs.totalWins--;
                    }
                }
                else if ((string)dt.Rows[dt.Rows.Count - 1][5] == "Tank") // Tank
                {
                    td.tankTotal--;
                    if ((string)dt.Rows[dt.Rows.Count - 1][4] == "Win")
                    {
                        td.tankWins--;
                        gs.totalWins--;
                    }
                }
            }
            UpdateGeneralStats();
            UpdateTankText();
            
            if(index > 0)
            {
                index--;
            }
        }

        public void MapType()
        {
            if(mapTypeComboBox.SelectedItem.ToString() == "Hybrid")
            {
                gs.totalHybridGames++;
                if (winCheckBox.IsChecked == true)
                {
                    gs.totalHybridWins++;
                }
                
            }
            else if(mapTypeComboBox.SelectedItem.ToString() == "Escort")
            {
                gs.totalEscortGames++;
                if(winCheckBox.IsChecked == true)
                {
                    gs.totalEscortWins++;
                }
            }
            else if(mapTypeComboBox.SelectedItem.ToString() == "Control")
            {
                gs.totalControlGames++;
                if(winCheckBox.IsChecked == true)
                {
                    gs.totalControlWins++;
                }
            }
            else if(mapTypeComboBox.SelectedItem.ToString() == "Assault")
            {
                gs.totalAssaultGames++;
                if(winCheckBox.IsChecked == true)
                {
                    gs.totalAssaultWins++;
                }
            }
        }

        #endregion

        #region Binary Serialization

        public void LoadGeneralStatsFromBin()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("GeneralStatsBin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            gs = (GeneralStats)formatter.Deserialize(stream);
            stream.Close();
        }

        public void SaveStats()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("GeneralStatsBin", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            formatter.Serialize(stream, gs);
            stream.Close();
        }
        #endregion

        #region Updating Text Fields


        public void UpdateGeneralStats()
        {
            GSWinPercentage.Text = ((gs.totalWins / gs.totalGames) * 100).ToString();
            GSTotalGames.Text = gs.totalGames.ToString();
            if(gs.totalControlGames > 0)
            {
                GSControlWinPercentage.Text = ((gs.totalControlWins / gs.totalControlGames) * 100).ToString();
            }
            if(gs.totalEscortGames > 0)
            {
                GSEscortWinPercentage.Text = ((gs.totalEscortWins / gs.totalEscortGames) * 100).ToString();
            }
            if(gs.totalAssaultGames > 0)
            {
                GSAssaultWinPercentage.Text = ((gs.totalAssaultWins / gs.totalAssaultGames) * 100).ToString();
            }
            if(gs.totalHybridGames > 0)
            {
                GSHybridWinPercentage.Text = ((gs.totalHybridWins / gs.totalHybridGames) * 100).ToString();
            }
            
        }

        public void UpdateTankText()
        {
            if(td.tankTotal > 0)
            {
                tankWinPercentageTextbox.Text = Math.Round(((td.tankWins / td.tankTotal) * 100), 2).ToString();
            }
            if(td.tankHybridTotal > 0)
            {
                TankHybridWinPercentage.Text = ((td.tankHybridWins / td.tankHybridTotal) * 100).ToString();
            }
            if(td.tankControlTotal > 0)
            {
                tankControlWinPercentage_Textbox.Text = ((td.tankControlWins / td.tankControlTotal) * 100).ToString();
            }
            if(td.tankAssaultTotal > 0)
            {
                TankAssaultWinPercentage_Textbox.Text = ((td.tankAssaultWins / td.tankAssaultTotal) * 100).ToString();
            }
            if(td.tankEscortTotal > 0)
            {
                TankEscortWinPercentage_Checkbox.Text = ((td.tankEscortWins / td.tankEscortTotal) * 100).ToString();
            }
            TankTotalGames_TextBox.Text = td.tankTotal.ToString();
            TankTotalWins_TextBox.Text = td.tankWins.ToString();

            
        }

        private void Save_Session_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveStats();
        }

       public void TankStatsUpdate()
        {
            if(roleComboBox.SelectedItem.ToString() == "Tank")
            {
                /*if(mapTypeComboBox.SelectedItem.ToString() == "Hybrid")
                {
                    td.tankHybridTotal++;
                    if(winCheckBox.IsChecked == true)
                    {
                        td.tankHybridWins++;
                    }
                }*/
                if(mapTypeComboBox.SelectedItem.ToString() == "Escort")
                {
                    td.tankEscortTotal++;
                    if (winCheckBox.IsChecked == true)
                    {
                        td.tankEscortWins++;
                    }
                }
                else if(mapTypeComboBox.SelectedItem.ToString() == "Assault")
                {
                    td.tankAssaultTotal++;
                    if (winCheckBox.IsChecked == true)
                    {
                        td.tankAssaultWins++;
                    }
                }
                else if(mapTypeComboBox.SelectedItem.ToString() == "Control")
                {
                    td.tankControlTotal++;
                    if (winCheckBox.IsChecked == true)
                    {
                        td.tankControlWins++;
                    }
                }
                td.tankTotal++;
                if(winCheckBox.IsChecked == true)
                {
                    td.tankWins++;
                }
                UpdateTankText();
            }
        }
        
        public void TestOne()
        {
            DataRow[] tankRows;
            td.tankTotal = 0;
            tankRows = dt.Select("Role = 'Tank'");
            foreach (DataRow row in tankRows)
            {
                td.tankTotal++;
            }
            Trace.WriteLine(td.tankTotal);
        }

        public void TestTwo()
        {
            DataRow[] temp;
            td.tankHybridTotal = 0;
            temp = dt.Select("Role = 'Tank' AND 'Map Type' = 'Hybrid'");
            foreach(DataRow row in temp)
            {
                td.tankHybridTotal++;
                if(row[4].ToString() == "Win")
                {
                    td.tankHybridWins++;
                }
            }
            UpdateTankText();
        }


        #endregion
        
    }
}
