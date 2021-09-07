using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GeocoordinateTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private Geolocator Geolocator { get; set; }

        private DispatcherTimer Timer { get; set; } = new DispatcherTimer();

        private double currentSpeed;


        public double CurrentSpeed
        {
            get { return currentSpeed; }
            set {
                currentSpeed = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainPage()
        {
            this.InitializeComponent();
            Initialize();
            
        }

        private async void Timer_Tick(object sender, object e)
        {
            try
            {
                var geoPosition = await Geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(15));
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    CurrentSpeed = geoPosition.Coordinate.Speed.Value;
                    listBoxSpeed.Items.Add($"{CurrentSpeed * 60 * 60 / 1000} Kilometers per hour.");
                });
            }
            catch (Exception ex)
            {
                listBoxSpeed.Items.Add(ex.Message);
            }
        }

        private async void Initialize()
        {
            FakeData();
            var status = await Geolocator.RequestAccessAsync();
            if (status == GeolocationAccessStatus.Allowed)
            {
                Geolocator = new Geolocator();
                Geolocator.DesiredAccuracy = PositionAccuracy.High;
                Geolocator.DesiredAccuracyInMeters = 1;
                Geolocator.PositionChanged += Geolocator_PositionChanged;
                Timer.Interval = new TimeSpan(0, 0, 2);
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
             {
                 CurrentSpeed = args.Position.Coordinate.Speed.Value;
                 listBoxPosition.Items.Add($"{CurrentSpeed * 60 * 60 / 1000} Kilometers per hour.");
             });
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var data = listBoxSpeed.Items.Select(i => i.ToString());
            
                await SaveGeoLocationSpeedData(data);
            
        }

        private async Task SaveGeoLocationSpeedData(IEnumerable<string> data)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "GeoLocation Speed";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
            // write to file
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await FileIO.WriteLinesAsync(file, data);

            });
            }
        }

        private void FakeData()
        {
            for (var i = 0; i < 10; i++)
            {
                listBoxSpeed.Items.Add($"{i * 60 * 60 / 1000} Kilometers per hour.");
            }
        }
    }
}
