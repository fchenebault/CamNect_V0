using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace CamNect.Camera
{
    public class VlcPlayer
    {
        private VlcControl control;

        public VlcPlayer(VlcControl control, Image player1)
        {
            this.control = control;

            VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_X86;
            VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_X86;

            VlcContext.StartupOptions.IgnoreConfig = true;
            VlcContext.StartupOptions.LogOptions.LogInFile = false;
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
            VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.None;
            
            Binding Bing = new Binding();
            Bing.Source = control;
            Bing.Path = new PropertyPath("VideoSource");
            player1.SetBinding(Image.SourceProperty, Bing);
        }

        public VlcControl Control
        {
            get{return control;}
        }

        public MediaBase Media
        {
            get;
            set;
        }
    }
}
