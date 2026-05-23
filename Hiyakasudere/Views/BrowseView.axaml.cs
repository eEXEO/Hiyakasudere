using Avalonia.Controls;
using Avalonia.Input;
using Hiyakasudere.ViewModels;
using System.Reactive.Linq;

namespace Hiyakasudere.Views;

public partial class BrowseView : UserControl
{
    public BrowseView()
    {
        InitializeComponent();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is BrowseViewModel vm)
            {
                vm.SearchCommand.Execute().Subscribe();
            }
        }
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer && DataContext is BrowseViewModel vm)
        {
            var scrollableHeight = scrollViewer.Extent.Height - scrollViewer.Viewport.Height;
            if (scrollableHeight <= 0) return;

            var scrollPercentage = scrollViewer.Offset.Y / scrollableHeight;

            // Load more when scrolled past 85%
            if (scrollPercentage > 0.85 && !vm.IsLoading && vm.CurrentPage < vm.TotalPages)
            {
                vm.LoadNextPageCommand.Execute().Subscribe();
            }
        }
    }
}
