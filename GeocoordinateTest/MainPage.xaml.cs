using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
                    listBoxSpeed.Items.Add(CurrentSpeed);
                });
            }
            catch (Exception ex)
            {
                listBoxSpeed.Items.Add(ex.Message);
            }
        }

        private async void Initialize()
        {
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
                 listBoxPosition.Items.Add(CurrentSpeed);
             });
            
        }

        //private async void CalculateSpeed()
        //{
        //    Geolocator = new Geolocator();
        //    var geoPosition = await Geolocator.GetGeopositionAsync();
        //    CurrentSpeed = geoPosition.Coordinate.Speed.Value;
        //}
    }
}
