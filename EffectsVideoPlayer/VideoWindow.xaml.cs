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
    ///  VideoWindow.xaml 的互動邏輯
    /// </summary>
    public partial class VideoWindow : Window
    {
        public VWindowControl myController;
        public VideoWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if(myController != null)
            {
                myController.ControlledWindowClosed();
            }
            base.OnClosing(e);
        }
        public Canvas getCanvas() { return MainCanvas; }
    }
}