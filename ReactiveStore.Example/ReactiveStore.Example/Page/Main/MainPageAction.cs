using System;

namespace ReactiveStore.Example.Page.Main
{
    public class MainPageAction
    {
        public class BarcodeScanned : MainPageAction
        {
            public string Barcode { get; set; }
        }
    
        public class ItemDownloaded : MainPageAction
        {
            public string Item { get; set; }
            public DateTime Time { get; set; }
        }
        
        public class Confirmed : MainPageAction
        {
            public string ConfirmedItem { get; set; }
        }
    }
}

