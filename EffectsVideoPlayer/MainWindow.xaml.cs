using System;
using System.Collections.Generic;
using System.Linq;
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

namespace EffectsVideoPlayer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        List<VWindowControl> Controllers;
        public MainWindow()
        {
            InitializeComponent();
            Controllers = new List<VWindowControl>();
        }

        private void NewVideoWindow(object sender, RoutedEventArgs e)
        {
            String wName = winName.Text;
            int width = 320, height = 180, x = 0, y = 0;
            if(!int.TryParse(winWidth.Text, out width) || width<=0) width = 320;
            if(!int.TryParse(winHeight.Text, out height) || height <= 0) height = 180;
            if(!int.TryParse(winPosX.Text, out x)) x = 0;
            if(!int.TryParse(winPosY.Text, out y)) y = 0;
            VWindowControl tmp = new VWindowControl();
            Controllers.Add(tmp);
            ControlStack.Children.Insert(ControlStack.Children.Count-1, tmp);
            tmp.master = this;
            tmp.initWinData(wName,width, height,x,y);
        }
        public void RemoveController(VWindowControl c)
        {
            ControlStack.Children.Remove(c);
            Controllers.Remove(c);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            while(Controllers.Count > 0)
            {
                Controllers[Controllers.Count-1].controlledWindow.Close();
            }
            base.OnClosing(e);
        }

        private void checkNumber(object sender, TextChangedEventArgs e)
        {
            TextBox tmp = (TextBox)sender;
            try
            {
                int val = int.Parse(tmp.Text);
                tmp.BorderThickness = new Thickness(0);
            }
            catch (Exception)
            {
                tmp.BorderThickness = new Thickness(2);
            }
        }

        private void checkNumberPositive(object sender, TextChangedEventArgs e)
        {
            TextBox tmp = (TextBox)sender;
            try
            {
                int val = int.Parse(tmp.Text);
                if (val <= 0) throw new Exception();
                tmp.BorderThickness = new Thickness(0);
            }
            catch (Exception)
            {
                tmp.BorderThickness = new Thickness(2);
            }
        }
    }
}

