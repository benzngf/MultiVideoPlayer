using Microsoft.Win32;
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
    /// WindowControl.xaml 的互動邏輯
    /// </summary>
    public partial class VWindowControl : StackPanel
    {
        public VideoWindow controlledWindow;
        List<VLayerControl> underlyingLayers;
        public MainWindow master;
        Thickness t0, t2;
        bool isDragging;
        Point MouseStartDragP;
        double WindowStartDragTop, WindowStartDragLeft;
        public void initWinData(String Name, int w, int h, int x, int y)
        {
            winWidth.Text = "" + w;
            winHeight.Text = "" + h;
            winName.Text = Name;
            VideoWindow tmpvw = new VideoWindow();
            controlledWindow = tmpvw;
            tmpvw.myController = this;
            tmpvw.Title = Name;
            tmpvw.Show();
            controlledWindow.Width = w;
            controlledWindow.Height = h;
            controlledWindow.Left = x;
            controlledWindow.Top = y;
        }
        public void ControlledWindowClosed()
        {
            master.RemoveController(this);
        }
        public VWindowControl()
        {
            InitializeComponent();
            t0 = new Thickness(0.0);
            t2 = new Thickness(2.0);
            isDragging = false;
            underlyingLayers = new List<VLayerControl>();
        }
        private void DraggableEnd_Move(object sender, MouseButtonEventArgs e)
        {
             isDragging = false;
            Mouse.Capture(null);
            Mouse.OverrideCursor = Cursors.Arrow;
            MoveBtn.Active(false);
        }

        private void DraggableStart_Move(object sender, MouseButtonEventArgs e)
        {
            MouseStartDragP = Mouse.GetPosition(Application.Current.MainWindow);
            WindowStartDragTop = controlledWindow.Top;
            WindowStartDragLeft = controlledWindow.Left;
            isDragging = true;
            Mouse.Capture((Button)sender);
            Mouse.OverrideCursor = Cursors.ScrollAll;
            MoveBtn.Active(true);
        }
        private void MouseMoving(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Vector tmpP = (Mouse.GetPosition(Application.Current.MainWindow) - MouseStartDragP);
                controlledWindow.Top = WindowStartDragTop + tmpP.Y;
                controlledWindow.Left = WindowStartDragLeft + tmpP.X;
            }
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (controlledWindow != null) controlledWindow.Close();
            else ControlledWindowClosed();
        }

        private void Click_AddVidLayer(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = VLayerControl.VidFilterString;
            if (openFileDialog.ShowDialog() == true)
            {
                VLayerControl tmp = new VLayerControl();
                underlyingLayers.Add(tmp);
                tmp.belongWindow = this;
                tmp.openFile(new Uri(openFileDialog.FileName));
                this.Children.Insert(this.Children.Count - 1, tmp);
            }
        }

       

        void ChangeWindowSize(object sender, TextChangedEventArgs e)
        {
            int width = 0, height = 0;
            bool setNewVal = true;
            try
            {
                width = int.Parse(winWidth.Text);
                if (width <= 0) throw new Exception();
                winWidth.BorderThickness = t0;
            }
            catch (Exception)
            {
                winWidth.BorderThickness = t2;
                setNewVal = false;
            }
            try
            {
                height = int.Parse(winHeight.Text);
                if (height <= 0) throw new Exception();
                winHeight.BorderThickness = t0;
            }
            catch (Exception)
            {
                winHeight.BorderThickness = t2;
                setNewVal = false;
            }
            if (setNewVal && controlledWindow!=null)
            {
                controlledWindow.Width = width;
                controlledWindow.Height = height;
            }
        }
    }
}
