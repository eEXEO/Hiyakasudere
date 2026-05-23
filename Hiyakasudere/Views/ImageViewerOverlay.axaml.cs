using Avalonia.Controls;
using Avalonia.Input;
using Hiyakasudere.ViewModels;
using System.Reactive.Linq;

namespace Hiyakasudere.Views;

public partial class ImageViewerOverlay : UserControl
{
    public ImageViewerOverlay()
    {
        InitializeComponent();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ImageViewerViewModel vm) return;

        switch (e.Key)
        {
            case Key.Escape:
                vm.CloseCommand.Execute().Subscribe();
                e.Handled = true;
                break;
            case Key.Left:
                vm.PrevCommand.Execute().Subscribe();
                e.Handled = true;
                break;
            case Key.Right:
                vm.NextCommand.Execute().Subscribe();
                e.Handled = true;
                break;
            case Key.S:
                vm.SaveCommand.Execute().Subscribe();
                e.Handled = true;
                break;
        }
    }

    private void Background_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is ImageViewerViewModel vm)
        {
            vm.CloseCommand.Execute().Subscribe();
        }
    }
}
