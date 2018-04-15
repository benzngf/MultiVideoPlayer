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
using System.Windows.Shapes;

namespace EffectsVideoPlayer
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class CommonBtn : Button
    {
        Brush EnabledBG;
        Brush ActiveBG;
        Brush DisabledBG;
        public bool IsActive;
        public CommonBtn()
        {
            IsActive = true;
            InitializeComponent();
            EnabledBG = Background;
        }
        public void Disable()
        {
            if (DisabledBG == null)
            {
                DisabledBG = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/UI_0017_FrameDisabled.png")));
            }
            Background = DisabledBG;
            if(IsEnabled) IsEnabled = false;
        }
        public void Enable()
        {
            if (EnabledBG == null)
            {
                EnabledBG = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/UI_0015_Frame.png")));
            }
            Background = EnabledBG;
            if(!IsEnabled) IsEnabled = true;
        }
        public void Active(bool _isActive)
        {
            IsActive = _isActive;
            if (_isActive)
            {
                if (ActiveBG == null)
                {
                    ActiveBG = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/UI_0013_FrameActive.png")));
                }
                Background = ActiveBG;
            }
            else if (IsEnabled) Enable();
            else Disable();
        }
    }
}
