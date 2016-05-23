using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.UI.Settings
{
    public class Settings : INotifyPropertyChanged
    {
        private Settings() { }

        private static readonly string SettingsPath = System.IO.Path.Combine(Globals.BaseDataPath, "Settings.json");

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler DisplayModeChanged;
        public event EventHandler DisplayStateChanged;
        public event EventHandler DisplaySizeChanged;

        public static Settings Current { get; private set; } = new Settings();

        [Newtonsoft.Json.JsonIgnore]
        public InputBindings InputBindings { get; set; } = new InputBindings();

        private WindowDisplayMode _displayMode = WindowDisplayMode.Windowed;
        public WindowDisplayMode DisplayMode
        { 
            get { return _displayMode; }
            set
            {
                if(value != _displayMode)
                {
                    _displayMode = value;
                    NotifyPropertyChanged("DisplayMode");
                    DisplayModeChanged?.Invoke(this, new EventArgs());
                }
                    
            }
        }

        private System.Windows.WindowState _displayState = System.Windows.WindowState.Maximized;
        public System.Windows.WindowState DisplayState
        {
            get { return _displayState; }
            set
            {
                if(value != _displayState)
                {
                    _displayState = value;
                    NotifyPropertyChanged("DisplayState");
                    DisplayStateChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private System.Windows.Size _displaySize;
        public System.Windows.Size DisplaySize
        {
            get { return _displaySize; }
            set
            {
                if(_displaySize != value)
                {
                    _displaySize = value;
                    NotifyPropertyChanged("DisplaySize");
                    DisplaySizeChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private bool _unitExpendedHighlighting = false;
        public bool UnitExpendedHighlighting
        {
            get { return _unitExpendedHighlighting; }
            set
            {
                _unitExpendedHighlighting = value;
                NotifyPropertyChanged("UnitExpendedHighlighting");
            }
        }

        public static void LoadSettings()
        {
            if (System.IO.File.Exists(SettingsPath))
            {
                try
                {
                    Current = Serializers.Serializer.Deserialize<Settings>(System.IO.File.ReadAllText(SettingsPath));
                    Current.UnitExpendedHighlighting = false;
                }
                catch (Exception ex)
                {
                    var res = System.Windows.MessageBox.Show("Failed to load settings, error: " + ex.Message + "\n\nWould you like to ignore this and continue? Doing so will clear any existing settings.\n\nClick 'Yes' to continue, 'No' to exit.", "Error loading settings - Continue?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.No);

                    if (res == System.Windows.MessageBoxResult.No)
                    {
                        Environment.Exit(1);
                    }
                    else
                    {
                        Current = new Settings();
                    }
                }
            }
            else
            {
                Current = new Settings();
            }
        }

        public static void SaveSettings()
        {
            try
            {
                System.IO.File.WriteAllText(SettingsPath, Serializers.Serializer.Serialize(Current));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to save settings, error: " + ex.Message);
            }
        }

        public enum WindowDisplayMode { Fullscreen, BorderlessFullscreen, Windowed}

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
