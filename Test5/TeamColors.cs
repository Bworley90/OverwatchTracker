using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;


namespace RenownsOverwatchProgram
{

    class TeamColors
    {
        


        public void ChangeColorScheme(Menu toolbar, Grid grid, TabControl tabController, string toolbarColor, string backgroundColor, string TabControlColor, ColorScheme scheme)
        {
            var color = (Color)ColorConverter.ConvertFromString(toolbarColor);
            Brush brush = new SolidColorBrush(color);
            var colorTwo = (Color)ColorConverter.ConvertFromString(backgroundColor);
            Brush brushTwo = new SolidColorBrush(colorTwo);
            var colorThree = (Color)ColorConverter.ConvertFromString(TabControlColor);
            Brush brushThree = new SolidColorBrush(colorThree);
            toolbar.Background = brush;
            grid.Background = brushTwo;
            tabController.Background = brushThree;
            scheme.dataColor = TabControlColor;
            scheme.toolbarColor = toolbarColor;
            scheme.menuColor = backgroundColor;


        }
    }
}
