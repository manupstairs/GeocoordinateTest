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
            Timer.Interval = new TimeSpan(0,0,1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            var geoPosition = await Geolocator.GetGeopositionAsync();
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                CurrentSpeed = geoPosition.Coordinate.Speed.Value;
                listBoxSpeed.Items.Add(CurrentSpeed);
            });
        }

        private void Initialize()
        {
            Geolocator = new Geolocator();
            //Geolocator.PositionChanged += Geolocator_PositionChanged;
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
             {
                 CurrentSpeed = args.Position.Coordinate.Speed.Value;
                 listBoxSpeed.Items.Add(CurrentSpeed);
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
