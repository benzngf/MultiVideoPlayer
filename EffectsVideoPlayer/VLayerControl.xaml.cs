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
    /// VLayerControl.xaml 的互動邏輯
    /// </summary>
    public partial class VLayerControl : StackPanel
    {
        VBlock displayedMediaPlayer;
        public VWindowControl belongWindow;
        bool isVisible, isMute;
        public bool isLoop;
        bool isDraggingSeekBar;
        bool initialized;
        int curWidth, curHeight;
        double updatedSeekBarV;
        public static string VidFilterString = "Video files (*.mpg;*.mpeg;*.mp4)|*.mpg;*.mpeg;*.mp4|All files (*.*)|*.*";
        static BitmapImage hidePic = new BitmapImage(new Uri("pack://application:,,,/Images/UI_0000_Hide.png")), showPic = new BitmapImage(new Uri("pack://application:,,,/Images/UI_0001_Show.png"));
        static BitmapImage soundPic = new BitmapImage(new Uri("pack://application:,,,/Images/Sound.png")), mutePic = new BitmapImage(new Uri("pack://application:,,,/Images/NoSound.png"));
        public VLayerControl()
        {
            InitializeComponent();
            isLoop = false;
            initialized = false;
            curWidth = 0;
            curHeight = 0;
            SeekBar.Minimum = 0;
            SeekBar.Maximum = 10;
            isVisible = true;
            isMute = true;
        }
        private void initializeVBlock()
        {
            displayedMediaPlayer = new VBlock();
            displayedMediaPlayer.controller = this;
            belongWindow.controlledWindow.getCanvas().Children.Add(displayedMediaPlayer);
        }
        public void openFile(Uri uri)
        {
            if (displayedMediaPlayer == null) initializeVBlock();
            Filename.Text = displayedMediaPlayer.openMediaFile(uri);
        }
        
        public void updateSeekBar(double curFrac)
        {
            if (!isDraggingSeekBar)
            {
                SeekBar.Value = curFrac*10;
                updatedSeekBarV = SeekBar.Value;
            }
        }

        public void updateFileSize(int w, int h)
        {
            FileWidth.Text = "" + w;
            FileHeight.Text = "" + h;
            curWidth = w;
            curHeight = h;
            if (!initialized){
                initialized = true;
                snapToWindow();
            }
            FileLength.Text = TimeSpan.FromSeconds(displayedMediaPlayer.getTotalSeconds()).ToString(@"hh\:mm\:ss");
        }
        void snapToWindow()
        {
            double windowW, windowH;
            windowW = belongWindow.controlledWindow.Width;
            windowH = belongWindow.controlledWindow.Height;
            Matrix tmp = new Matrix();
           
            double ApplyScaleX = windowW / curWidth;
            double ApplyScaleY = windowH / curHeight;
            double ApplyScale = (ApplyScaleX < ApplyScaleY) ? ApplyScaleX : ApplyScaleY;
            tmp.Translate((windowW - curWidth* ApplyScale)/2, (windowH - curHeight* ApplyScale)/2);
            if (ApplyScale < 1)
            {
                tmp.ScalePrepend(ApplyScale, ApplyScale);
            }
            displayedMediaPlayer.RenderTransform = new MatrixTransform(tmp);
        }
        private void Click_OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = VidFilterString;
            if (openFileDialog.ShowDialog() == true)
            {
                openFile(new Uri(openFileDialog.FileName));
            }
        }
        bool isPlaying;
        private void PlayFile(object sender, RoutedEventArgs e)
        {
            if (displayedMediaPlayer != null)
            {
                bool succeed = displayedMediaPlayer.togglePlay(!isPlaying);
                if (succeed)
                {
                    isPlaying = !isPlaying;
                    PlayBtn.Active(isPlaying);
                }
            }
        }
        public void autoPaused()
        {
            isPlaying = false;
            PlayBtn.Active(isPlaying);
        }

        private void Click_SetLoop(object sender, RoutedEventArgs e)
        {
            isLoop = !isLoop;
            ((CommonBtn)sender).Active(isLoop);
        }
        bool isDragging;
        Point MouseStartDragP;
        Transform VBlockStartTransform;
        Cursor DraggingCur;
        private void DraggableEnd(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Mouse.Capture(null);
            Mouse.OverrideCursor = Cursors.Arrow;
            ((CommonBtn)sender).Active(false);
        }

        private void DraggableStart(object sender, MouseButtonEventArgs e)
        {
            MouseStartDragP = Mouse.GetPosition(Application.Current.MainWindow);
            VBlockStartTransform = displayedMediaPlayer.RenderTransform;
            displayedMediaPlayer.RenderTransform = new MatrixTransform(VBlockStartTransform.Value);
            isDragging = true;
            Mouse.Capture((Button)sender);
            DraggingCur = Cursors.ScrollAll;
            Mouse.OverrideCursor = DraggingCur;
            ((CommonBtn)sender).Active(true);
        }
        private void MouseMoving_Rot(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Vector tmpP = (Mouse.GetPosition(Application.Current.MainWindow) - MouseStartDragP);
                double applyVal = (tmpP.X * tmpP.X + tmpP.Y * tmpP.Y) * (((Math.Abs(tmpP.X) > Math.Abs(tmpP.Y)) ? (tmpP.X > 0) : (tmpP.Y > 0)) ? 1 : -1);
                Cursor tmpc;
                tmpc = (applyVal > 0) ? Cursors.ScrollSE : Cursors.ScrollNW;
                applyVal /= (360*360);
                if (tmpc != DraggingCur)
                {
                    Mouse.OverrideCursor = tmpc;
                    DraggingCur = tmpc;
                }
                Matrix tmp = VBlockStartTransform.Value;
                tmp.RotateAtPrepend(applyVal * 180, curWidth / 2, curHeight / 2);
                ((MatrixTransform)displayedMediaPlayer.RenderTransform).Matrix = tmp;
            }
        }
        private void MouseMoving_Scale(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Vector tmpP = (Mouse.GetPosition(Application.Current.MainWindow) - MouseStartDragP);
                double applyVal = (tmpP.X * tmpP.X + tmpP.Y * tmpP.Y) * (((Math.Abs(tmpP.X) > Math.Abs(tmpP.Y)) ? (tmpP.X > 0) : (tmpP.Y < 0)) ? 1 : -1);
                Cursor tmpc;
                if (Math.Abs(tmpP.X) > Math.Abs(tmpP.Y))
                {
                    tmpc = (applyVal > 0) ? Cursors.ScrollE : Cursors.ScrollW;
                }
                else
                {
                    tmpc = (applyVal > 0) ? Cursors.ScrollN : Cursors.ScrollS;
                }
                if (tmpc != DraggingCur)
                {
                    Mouse.OverrideCursor = tmpc;
                    DraggingCur = tmpc;
                }
                applyVal /= 10000;
                applyVal = Math.Pow(2,applyVal);
                Matrix tmp = VBlockStartTransform.Value;
                tmp.ScaleAtPrepend(applyVal, applyVal, curWidth / 2, curHeight / 2);
                ((MatrixTransform)displayedMediaPlayer.RenderTransform).Matrix = tmp;
            }
        }

        private void Click_ToStart(object sender, RoutedEventArgs e)
        {
            displayedMediaPlayer.seek(0);
        }

        private void Click_RemoveLayer(object sender, RoutedEventArgs e)
        {
            if (belongWindow != null)
            {
                belongWindow.controlledWindow.getCanvas().Children.Remove(displayedMediaPlayer);
                belongWindow.Children.Remove(this);
            }
        }

        private void SeekBar_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraggingSeekBar = true;
        }

        private void SeekBar_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingSeekBar = false;
            displayedMediaPlayer.seek(SeekBar.Value/10);
            updatedSeekBarV = SeekBar.Value;
        }

        private void DetectClickSeekBar(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurTime.Text = TimeSpan.FromSeconds(displayedMediaPlayer.getTotalSeconds()/10* SeekBar.Value).ToString(@"hh\:mm\:ss");
            if(Math.Abs(SeekBar.Value- updatedSeekBarV) > 0.5)
            {
                displayedMediaPlayer.seek(SeekBar.Value / 10);
                updatedSeekBarV = SeekBar.Value;
            }
        }

        private void ToggleSound(object sender, RoutedEventArgs e)
        {
            if (displayedMediaPlayer != null)
            {
                isMute = !isMute;
                displayedMediaPlayer.toggleMute();
                if (isMute) ((Image)SndBtn.Content).Source = mutePic;
                else ((Image)SndBtn.Content).Source = soundPic;
            }
        }

        private void Click_Snap(object sender, RoutedEventArgs e)
        {
            snapToWindow();
        }

        private void ToggleVisibility(object sender, RoutedEventArgs e)
        {
            if (displayedMediaPlayer != null)
            {
                isVisible = !isVisible;
                displayedMediaPlayer.toggleVisible();
                if(isVisible) ((Image)VisBtn.Content).Source = showPic;
                else ((Image)VisBtn.Content).Source = hidePic;
            }
        }

        private void MouseMoving_Translate(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Vector tmpP = (Mouse.GetPosition(Application.Current.MainWindow) - MouseStartDragP);
                Matrix tmp = VBlockStartTransform.Value;
                tmp.Translate(tmpP.X, tmpP.Y);
                ((MatrixTransform)displayedMediaPlayer.RenderTransform).Matrix = tmp;
            }
        }
    }
}
