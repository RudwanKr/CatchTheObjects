using CatchTheObjects.CustomControles;
using CatchTheObjects.Models;
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
            vm.ItemMissed += (item) =>
            {
                Dispatcher.Invoke(() => HandleMissEffect(item));
            };

            vm.ItemCaught += (item) => Dispatcher.Invoke(() =>
            {
                var effect = new AddPointsEffect();
                GameCanvas.Children.Add(effect);
                effect.Start(item.Type.ThemeColor, item.Type.Points, new Point(item.X + 22, item.Y + 10));

                Task.Delay(1000).ContinueWith(_ => Dispatcher.Invoke(() => GameCanvas.Children.Remove(effect)));
            });

            CompositionTarget.Rendering += (s, e) =>
            {
                if (e is RenderingEventArgs args)
                {
                    if (_lastFrameTime == TimeSpan.Zero)
                    {
                        _lastFrameTime = args.RenderingTime;
                        return;
                    }

                    double deltaTime = (args.RenderingTime - _lastFrameTime).TotalSeconds;
                    _lastFrameTime = args.RenderingTime;

                    if (vm.IsGameRunning)
                    {
                        vm.Update(deltaTime);
                    }
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

    private void HandleMissEffect(GameObject item)
    {
        var effect = new VanishEffect();

        GameCanvas.Children.Add(effect);

        effect.Start(item.Type.ThemeColor, new Point(item.X + 22, item.Y + 11));

        Task.Delay(1000).ContinueWith(_ =>
        {
            Dispatcher.Invoke(() => GameCanvas.Children.Remove(effect));
        });
    }
}