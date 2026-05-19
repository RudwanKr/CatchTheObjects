using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CatchTheObjects.CustomControles;

public partial class AddPointsEffect : UserControl
{
    private readonly Random _rand = new Random();

    public AddPointsEffect()
    {
        InitializeComponent();
    }
    public void Start(Color fruitColor, int points, Point position)
    {
        Canvas.SetLeft(this, position.X);
        Canvas.SetTop(this, position.Y);

        for (int i = 0; i < 8; i++)
        {
            CreateParticle(fruitColor);
        }

        if (points > 0)
        {
            AnimateText(points, fruitColor);
        }
    }

    private void CreateParticle(Color color)
    {
        Ellipse dot = new Ellipse { Width = 6, Height = 6, Fill = new SolidColorBrush(color) };
        ParticleCanvas.Children.Add(dot);

        double angle = _rand.NextDouble() * 2 * Math.PI;
        double distance = _rand.Next(20, 50);

        Storyboard sb = new Storyboard();
        var animX = new DoubleAnimation(0, Math.Cos(angle) * distance, TimeSpan.FromSeconds(0.4));
        var animY = new DoubleAnimation(0, Math.Sin(angle) * distance, TimeSpan.FromSeconds(0.4));
        var fade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));

        Storyboard.SetTarget(animX, dot); Storyboard.SetTargetProperty(animX, new PropertyPath("(Canvas.Left)"));
        Storyboard.SetTarget(animY, dot); Storyboard.SetTargetProperty(animY, new PropertyPath("(Canvas.Top)"));
        Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity")); Storyboard.SetTarget(fade, dot);

        sb.Children.Add(animX); sb.Children.Add(animY); sb.Children.Add(fade);
        sb.Begin();
    }

    private void AnimateText(int points, Color color)
    {
        PointsText.Text = $"+{points}";
        PointsText.Foreground = new SolidColorBrush(color);
        PointsText.Visibility = Visibility.Visible;

        Storyboard sb = new Storyboard();

        DoubleAnimation moveUp = new DoubleAnimation(0, -60, TimeSpan.FromSeconds(0.8))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(moveUp, PointsText);
        Storyboard.SetTargetProperty(moveUp, new PropertyPath("(RenderTransform).(TranslateTransform.Y)"));

        DoubleAnimation fade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.8));
        Storyboard.SetTarget(fade, PointsText);
        Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));

        PointsText.RenderTransform = new TranslateTransform();
        sb.Children.Add(moveUp);
        sb.Children.Add(fade);
        sb.Begin();
    }
}
