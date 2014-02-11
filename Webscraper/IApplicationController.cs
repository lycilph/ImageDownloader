using System.Collections.ObjectModel;

namespace Webscraper
{
    public interface IApplicationController
    {
        PageLoader Loader { get; }
        bool IsBusy { get; set; }
        string BusyText { get; set; }
        ObservableCollection<ItemViewModel> Pages { get; }
        ObservableCollection<ItemViewModel> Images { get; }

        void Restart();
    }
}
