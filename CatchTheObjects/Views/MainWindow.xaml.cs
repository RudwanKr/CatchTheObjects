using CatchTheObjects.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace CatchTheObjects.Views;

public partial class MainWindow : Window
{
    private TimeSpan _lastFrameTime;

    public MainWindow()
    {
        InitializeComponent();

        if (DataContext is MainViewModel vm)
        {
            CompositionTarget.Rendering += (s, e) =>
            {
                if (e is RenderingEventArgs args)
                {
                    if (_lastFrameTime == TimeSpan.Zero)
                    {
                        _lastFrameTime = args.RenderingTime;
                        return;
                    }

                    double deltaTime =
                        (args.RenderingTime - _lastFrameTime).TotalSeconds;

                    _lastFrameTime = args.RenderingTime;

                    vm.Update(deltaTime);
                }
            };

            MouseMove += (s, e) =>
            {
                Point mousePos = e.GetPosition(GameCanvas);

                double newX = mousePos.X - 45;

                vm.BasketX = Math.Clamp(
                    newX,
                    0,
                    GameCanvas.ActualWidth - 90
                );
            };
        }
    }
}