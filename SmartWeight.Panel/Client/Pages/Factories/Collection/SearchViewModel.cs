using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace SmartWeight.Admin.Client.Pages.Factories
{
    /// <summary>
    /// Search
    /// </summary>
    public class SearchViewModel<T> : ReactiveObject
    {
        // Constructor
        public SearchViewModel()
        {
            // Here we're describing here, in a *declarative way*, the conditions in
            // which the Search command is enabled.  Now our Command IsEnabled is
            // perfectly efficient, because we're only updating the UI in the scenario
            // when it should change.
            // var canSearch = this.WhenAny(x => x.SearchQuery, x => true);

            // ReactiveCommand has built-in support for background operations and
            // guarantees that this block will only run exactly once at a time, and
            // that the CanExecute will auto-disable and that property IsExecuting will
            // be set according whilst it is running.
            Search = ReactiveCommand.CreateFromTask<string?, IEnumerable<T>?>(async _ => {
                return await SearchFunction?.Invoke(SearchQuery);
            });
            // ReactiveCommands are themselves IObservables, whose value are the results
            // from the async method, guaranteed to arrive on the UI thread. We're going
            // to take the list of search results that the background operation loaded,
            // and them into our SearchResults.
            Search.Subscribe(results => {
                SearchResults.Clear();
                SearchResults.AddRange(results);
            });
            // ThrownExceptions is any exception thrown from the CreateAsyncTask piped
            // to this Observable. Subscribing to this allows you to handle errors on
            // the UI thread.
            Search.ThrownExceptions
                .Subscribe(ex => {
                    SearchResults.Clear();
                });
            // Whenever the Search query changes, we're going to wait for one second
            // of "dead airtime", then automatically invoke the subscribe command.
            this.WhenAnyValue(x => x.SearchQuery)
                .InvokeCommand(this, x => x.Search);           
        }
        /// <summary>
        /// Command to search factories by qury
        /// </summary>
        public ReactiveCommand<string?, IEnumerable<T>?> Search { get; set; }
        /// <summary>
        /// Factories by search query
        /// </summary>
        private ObservableCollection<T> searchResults = new();
        /// <summary>
        /// Factories by search query
        /// </summary>
        public ObservableCollection<T> SearchResults
        {
            get { return searchResults; }
            set { this.RaiseAndSetIfChanged(ref searchResults, value); }
        }
        private string? searchQuery;
        /// <summary>
        /// Search query
        /// </summary>
        public string? SearchQuery
        {
            get { return searchQuery; }
            set { this.RaiseAndSetIfChanged(ref searchQuery, value); }
        }
        /// <summary>
        /// Function of search
        /// </summary>
        public Func<string?, Task<IEnumerable<T>?>>? SearchFunction { get; set; }
    }
}
