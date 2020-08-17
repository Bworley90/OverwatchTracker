﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Test5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


        // Add role to datatable and fill row 
    public partial class MainWindow : Window
    {
        DataTable dt = new DataTable("OverwatchMatchData");
        GeneralStats gs = new GeneralStats();
        TankData td = new TankData();
        HealsData hd = new HealsData();
        DPSData dd = new DPSData();

        DataSet ds = new DataSet();
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
            LoadGeneralStatsFromXML();
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
            DataColumn map = new DataColumn("Map", typeof(string));

            try
            {
                dt.Columns.Add(matchNumber);
                dt.Columns.Add(mapType);
                dt.Columns.Add(attackDefend);
                dt.Columns.Add(win);
                dt.Columns.Add(role);
                dt.Columns.Add(map);
            }
            catch
            {
                Trace.WriteLine("Table already created");
            }
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

        public void LoadGeneralStatsFromXML()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OverwatchTrackerData");
                DataTable tempTable = new DataTable();
                FileStream stream = new FileStream(@path, FileMode.Open, FileAccess.Read, FileShare.Read);
                tempTable.ReadXml(stream);
                dt = tempTable;
                stream.Close();
                Stats();
                dg.ItemsSource = dt.DefaultView;

            }
            catch
            {
                Trace.WriteLine("Missing Datatable information");
            }
            dg.ItemsSource = dt.DefaultView;
        }


        public void SaveFile()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OverwatchTrackerData");
            dt.WriteXml(@path, XmlWriteMode.WriteSchema);

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
            TableSearch("Attacked = True", gs.AttackWins, gs.AttackGames, GSAttackWinPercentage);
            TableSearch("Attacked = False", gs.DefendWins, gs.DefendGames, GSDefendWinPercentage);
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
            TableSearch("Role = 'Tank' AND Attacked = True", td.AttackWin, td.AttackTotal, TankAttackWinPercentage_Textbox);
            TableSearch("Role = 'Tank' AND Attacked = False", td.DefendWin, td.DefendTotal, TankDefendWinPercentage_Textbox);


        }


        public void HealsWinTotal()
        {
            GameCount("Role = 'Heals'", hd.Total, HealsTotalGames_TextBox);
            TableSearch("Role = 'Heals'", hd.Wins, hd.Total, HealsWinPercentage_TextBox);
            TableSearch("Role = 'Heals' AND Type = 'Control'", hd.ControlWins, hd.ControlTotal, HealsControlWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Assault'", hd.AssaultWins, hd.AssaultTotal, HealsAssaultWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Escort'", hd.EscortWins, hd.EscortTotal, HealsEscortWinPercentage_Textbox);
            TableSearch("Role = 'Heals' AND Type = 'Hybrid'", hd.HybridWins, hd.HybridTotal, HealsHybridWinPercentage_TextBox);
            TableSearch("Role = 'Heals' AND Attacked = True", hd.AttackWin, hd.AttackTotal, HealsAttackWinPercentage_TextBox);
            TableSearch("Role = 'Heals' AND Attacked = False", hd.DefendWin, hd.DefendTotal, HealsDefendWinPercentage_TextBox);
        }


        public void DPSWinTotal()
        {
            GameCount("Role = 'DPS'", dd.Total, DPSTotalGames_TextBox);
            TableSearch("Role = 'DPS'", dd.Wins, dd.Total, DPSWinPercentage_TextBox);
            TableSearch("Role = 'DPS' AND Type = 'Control'", dd.ControlWins, dd.ControlTotal, DPSControlWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Assault'", dd.AssaultWins, dd.AssaultTotal, DPSAssaultWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Escort'", dd.EscortWins, dd.EscortTotal, DPSEscortWinPercentage_Textbox);
            TableSearch("Role = 'DPS' AND Type = 'Hybrid'", dd.HybridWins, dd.HybridTotal, DPSHybridWinPercentage_TextBox);
            TableSearch("Role = 'DPS' AND Attacked = True", dd.AttackWin, dd.AttackTotal, DPSAttackWinPercentage_TextBox);
            TableSearch("Role = 'DPS' AND Attacked = False", dd.DefendWin, dd.DefendTotal, DPSDefendWinPercentage_TextBox);
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


        private void ClearData_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Clear all data?", "Erase all", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
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
            if (mapComboBox.SelectedItem != null)
            {
               row[5] = mapComboBox.SelectedItem.ToString();
                
            }
            else
            {
                MessageBox.Show("No Map selected. Please select a Map", "Map Error", MessageBoxButton.OK);
                return;
            }
            dt.Rows.Add(row);
            gs.totalGames++;

            Stats();// Test Function
            dg.ItemsSource = dt.DefaultView;
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRow();
        }

        private void LoadFromFile_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Load from file? \n !!THIS WILL ERASE THE CURRENT LOGGED MATCHES!!", "Load File", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OverwatchTrackerData");
                DataTable tempTable = new DataTable();
                FileStream stream = new FileStream(@path, FileMode.Open, FileAccess.Read, FileShare.Read);
                tempTable.ReadXml(stream);
                dt = tempTable;
                stream.Close();
                dg.ItemsSource = dt.DefaultView;
                Stats();
            }

        }

        private void SaveToFile_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Save Session?", "Save", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveFile();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Save Session?", "Save", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveFile();
            }
        }
        #endregion

        #region Public Functions

        private void CheckBoxesActive(bool isActive)
        {
            if (isActive)
            {
                attackCheck.IsEnabled = true;
            }
            else
            {
                attackCheck.IsEnabled = false;
            }
        }

        private void TextBoxBackgroundColor(float meteredNumber, TextBox textBox,  float lowCutoff, float highCutoff)
        {
            if (meteredNumber <= 25)
            {
                textBox.Background = Brushes.Red;
            }
            else if (meteredNumber > 25 && meteredNumber <= 35)
            {
                textBox.Background = Brushes.IndianRed;
            }
            else if (meteredNumber > 35 && meteredNumber <= 45)
            {
                textBox.Background = Brushes.LightYellow;
            }
            else if(meteredNumber > 45 && meteredNumber <= 55)
            {
                textBox.Background = Brushes.Yellow;
            }
            else if (meteredNumber > 55 && meteredNumber <= 65)
            {
                textBox.Background = Brushes.LightGreen;
            }
            else if (meteredNumber > 65 && meteredNumber <= 75)
            {
                textBox.Background = Brushes.LawnGreen;
            }
            else if (meteredNumber > 75)
            {
                textBox.Background = Brushes.Green;
            }
            else
            {
                textBox.Background = Brushes.LightBlue;
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
