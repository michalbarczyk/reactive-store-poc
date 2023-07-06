using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveStore.Net;
using Xamarin.Forms;

namespace ReactiveStore.Example.Page.Main
{
    public partial class MainPage : ContentPage
    {
        private Store<MainPageState, MainPageAction, MainPageReducer> _store;

        private readonly IObservable<string> _someAdditionalData = Observable.Return("some additional data");
        
        public ICommand DownloadCommand { get; }

        public MainPage()
        {
            SetupStore();
            
            DownloadCommand = new Command(() =>
            {
                _store.Dispatch(new MainPageAction.BarcodeScanned { Barcode = $"333333444444{new Random().Next(10, 100)}" });
            });
            
            BindingContext = this; // for simplification ViewModel is omitted

            InitializeComponent();
        }

        private void SetupStore()
        {
            _store = new Store<MainPageState, MainPageAction, MainPageReducer>(
                new MainPageState { ConfirmedValue = string.Empty }
            );

            _store.Effect(action => action
                .OfType<MainPageAction.BarcodeScanned>()
                .SelectMany(barcodeScanned =>
                {
                    return DownloadAsync(barcodeScanned.Barcode)
                        .ToObservable()
                        .Select(response => new MainPageAction.ItemDownloaded 
                        { 
                            Item = response, 
                            Time = DateTime.Now 
                        });
                })
            );

            _store.Effect(action => action
                .OfType<MainPageAction.ItemDownloaded>()
                .WithLatestFrom(_someAdditionalData)
                .SelectMany(itemDownloadedWithAdditionalData =>
                {
                    var itemDownloaded = itemDownloadedWithAdditionalData.First;
                    var additionalData = itemDownloadedWithAdditionalData.Second;
                    
                    return DisplayAlert("Attention", $"Downloaded item is:\n{itemDownloaded.Item}\nat {itemDownloaded.Time}\nwith {additionalData}.", "Confirm")
                        .ToObservable()
                        .Select(unit => new MainPageAction.Confirmed { ConfirmedItem = itemDownloaded.Item });
                })
            );
        }
        
        private async Task<string> DownloadAsync(string request)
        {
            await Task.Delay(500);
            return "Response to: [" + request + "]";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            _store.State
                .Select(state => state.ConfirmedValue)
                .Where(text => !string.IsNullOrEmpty(text))
                .Subscribe(text =>
                {
                    label.Text = $"Confirmed: {text}";
                });
        }
    }
}