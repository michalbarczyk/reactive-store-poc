using System.Diagnostics;
using ReactiveStore.Net;

namespace ReactiveStore.Example.Page.Main
{
    public class MainPageReducer : IReducer<MainPageState, MainPageAction>
    {
        public MainPageState Reduce(MainPageState state, MainPageAction action)
        {
            Debug.WriteLine($"Reducing {action.GetType()}...");
            switch (action)
            {
                case MainPageAction.Confirmed itemDownloaded:
                    return new MainPageState
                    {
                        ConfirmedValue = itemDownloaded.ConfirmedItem
                    };
                default:
                    return state;
            };
        }
    }
}