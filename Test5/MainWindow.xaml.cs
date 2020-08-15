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
        HealsData hd = new HealsData();
        DPSData dd = new DPSData();
        public int gameNumber = 1;

        // Map Lists
        List<string> maps = new List<string>();

        List<string> EscortMaps = new List<string>();
        List<string> AssaultMaps = new List<string>();
        List<string> ControlMaps = new List<string>();
        List<string> HybridMaps = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            AddAllMaps();
            SetupTable();
            LoadGeneralStatsFromBin();
            Stats();
            


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
            DataColumn matchNumber = new DataColumn("Match", typeof(int));
            DataColumn mapType = new DataColumn("Type", typeof(string));
            DataColumn attackDefend = new DataColumn("Attacked", typeof(bool));
            DataColumn win = new DataColumn("Win", typeof(bool));
            DataColumn role = new DataColumn("Role", typeof(string));

            try
            {
                dt.Columns.Add(matchNumber);
                dt.Columns.Add(mapType);
                dt.Columns.Add(attackDefend);
                dt.Columns.Add(win);
                dt.Columns.Add(role); 
            }
            catch
            {
                Trace.WriteLine("Table already created");
            }
            

            dg.ItemsSource = dt.DefaultView;
        }


        public void RemoveRow()
        {
            if(dt.Rows.Count > 0)
            {
                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                gs.Games--;
                gameNumber--;
            }
            gs.totalGames--;
            Stats();
            
        }

        #endregion

        #region Binary Serialization

        public void LoadGeneralStatsFromBin()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("DataTable", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            try
            {
                dt = (DataTable)formatter.Deserialize(stream);
            }
                catch
            {
                Trace.WriteLine("Failed to open dataTable");
            }
                
            stream.Close();
            dg.ItemsSource = dt.DefaultView;
        }

        public void SaveStats()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("DataTable", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            formatter.Serialize(stream, dt);
            stream.Close();
        }
        #endregion

        #region Updating Text Fields

        


        public void GeneralWinsTotal() // Updating stats through for loops
        {
            DataRow[] rows;
            rows = dt.Select();
            gs.Games = 0;
            gs.Wins = 0;
            foreach(DataRow row in rows)
            {
                gs.Games++;
                if(row[3].Equals(true))
                {
                    gs.Wins++;
                }
            }
            double temp = Math.Round(gs.Wins / gs.Games * 100, 2);
            if(gs.Games > 0)
            {
                GSWinPercentage.Text = temp.ToString() + "%";
            }
            else
            {
                GSWinPercentage.Text = "0%";
            }
            
            TextBoxBackgroundColor((float)temp, GSWinPercentage, 40, 60);
            TableSearch("Type = 'Escort'", gs.EscortWins, gs.EscortGames, GSEscortWinPercentage);
            TableSearch("Type = 'Hybrid'", gs.HybridWins, gs.HybridGames, GSHybridWinPercentage);
            TableSearch("Type = 'Assault'", gs.AssaultWins, gs.AssaultGames, GSAssaultWinPercentage);
            TableSearch("Type = 'Control'", gs.ControlWins, gs.ControlGames, GSControlWinPercentage);
            GSTotalGames.Text = gs.Games.ToString();
            GSTotalWins_TextBox.Text = gs.Wins.ToString();
        }


        public void TankWinsTotal()
        {
            GameCount("Role = 'Tank'", td.Total, TankTotalGames_TextBox);
            TableSearch("Role = 'Tank'", td.Wins, td.Total, TankWinPercentage_TextBox);
            TableSearch("Role = 'Tank' AND Type = 'Control'", td.ControlWins, td.ControlTotal, TankControlWinPercentage_Textbox);
            TableSearch("Role = 'Tank' AND Type = 'Assault'", td.AssaultWins, td.AssaultTotal, TankAssaultWinPercentage_Textbox);
            TableSearch("Role ='Tank' AND Type = 'Escort'", td.EscortWins, td.AssaultTotal, TankEscortWinPercentage_Textbox);
            TableSearch("Role = 'Tank' AND Type = 'Hybrid'", td.HybridWins, td.HybridTotal, TankHybridWinPercentage_TextBox);

        }


        public void HealsWinTotal()
        {
            GameCount("Role = 'Heals'", hd.Total, HealsTotalGames_TextBox);
            TableSearch("Role = 'Heals'", hd.Wins, hd.Total, HealsWinPercentage_TextBox);
            TableSearch("Role = 'Heals' AND Type = 'Control'", hd.ControlWins, hd.ControlTotal, HealsControlWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Assault'", hd.AssaultWins, hd.AssaultTotal, HealsAssaultWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Escort'", hd.EscortWins, hd.EscortTotal, HealsEscortWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Hybrid'", hd.HybridWins, hd.HybridTotal, HealsHybridWinPercentage_TextBox);
        }


        public void DPSWinTotal()
        {
            GameCount("Role = 'DPS'", dd.Total, DPSTotalGames_TextBox);
            TableSearch("Role = 'DPS'", dd.Wins, dd.Total, DPSWinPercentage_TextBox);
            TableSearch("Role = 'DPS' AND Type = 'Control'", dd.ControlWins, dd.ControlTotal, DPSControlWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Assault'", dd.AssaultWins, dd.AssaultTotal, DPSAssaultWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Escort'", dd.EscortWins, dd.EscortTotal, DPSEscortWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Hybrid'", dd.HybridWins, dd.HybridTotal, DPSHybridWinPercentage_TextBox);
            //
        }

        public void Stats()
        {
            GeneralWinsTotal();
            TankWinsTotal();
            HealsWinTotal();
            DPSWinTotal();
        }



        #endregion

        #region Button Events

        private void Save_Session_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = System.Windows.MessageBox.Show("Save the current session?", "Save Session", System.Windows.MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
            {
                SaveStats();
                MessageBoxResult confirmedSave = System.Windows.MessageBox.Show("Saved current session", "Saved", System.Windows.MessageBoxButton.OK);
            }

        }
        private void ClearSession_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Clear all data?", "Erase all", MessageBoxButton.YesNo);
            if(mbr == MessageBoxResult.Yes)
            {
                dt.Clear();
                SetupTable();
                Stats();
            }
            
            
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = dt.NewRow();
            row[0] = gs.totalGames + 1;
            //
            if(mapTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("No Map Type selected. Please select a Map Type", "Map Type Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                row[1] = ((ComboBoxItem)mapTypeComboBox.SelectedItem).Content.ToString();
            }

            row[2] = attackCheck.IsChecked;
            row[3] = winCheckBox.IsChecked;
            row[4] = ((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString();
            dt.Rows.Add(row);
            gs.totalGames++;

            Stats();// Test Function
            dg.ItemsSource = dt.DefaultView;
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRow();
        }
        #endregion

        #region Public Functions

        private void CheckBoxesActive(bool isActive)
        {
            if (isActive)
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

        private void TextBoxBackgroundColor(float meteredNumber, TextBox textBox,  float lowCutoff, float highCutoff)
        {
            if(meteredNumber <= lowCutoff)
            {
                textBox.Background = Brushes.Red;
            }
            else if(meteredNumber >= highCutoff)
            {
                textBox.Background = Brushes.Green;
            }
            else if(meteredNumber > lowCutoff && meteredNumber < highCutoff)
            {
                textBox.Background = Brushes.Orange;
            }
            else
            {
                textBox.Background = Brushes.LightCyan;
            }
        }

        public void TableSearch(string selectFilterString, float totalWins, float totalGames, TextBox textbox)
        {
            DataRow[] datarow;
            datarow = dt.Select(selectFilterString);
            totalWins = 0;
            totalGames = 0;
            foreach(DataRow row in datarow)
            {
                totalGames++;
                if(row[3].Equals(true))
                {
                    totalWins++;
                }
            }
            float temp = totalWins / totalGames * 100;
            if(totalGames > 0)
            {
                textbox.Text = Math.Round(temp, 2).ToString() + "%";
            }
            else
            {
                textbox.Text = "None";
            }
            
            TextBoxBackgroundColor(temp, textbox, 40, 60);
        }

        public void GameCount(string filterExpression, float totalGames, TextBox textbox)
        {
            DataRow[] rows;
            rows = dt.Select(filterExpression);
            totalGames = 0;
            foreach(DataRow row in rows)
            {
                totalGames++;
            }

            textbox.Text = totalGames.ToString();
        }


        #endregion
    }
}
