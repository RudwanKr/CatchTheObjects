using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace CatchTheObjects.CustomControles;

public partial class VanishEffect : UserControl
{
    private Random _rand = new Random();

    public void Start(Color fruitColor, System.Windows.Point position)
    {
        Canvas.SetLeft(this, position.X);
        Canvas.SetTop(this, position.Y);

        for (int i = 0; i < 12; i++)
        {
            CreateParticle(fruitColor);
        }
    }

    private void CreateParticle(Color color)
    {
        Ellipse dot = new Ellipse()
        {
            Width = _rand.Next(4, 8),
            Height = _rand.Next(4, 8),
            Fill = new SolidColorBrush(color)
        };

        ParticleCanvas.Children.Add(dot);

        double angle = _rand.NextDouble() * 2 * Math.PI;
        double distance = _rand.Next(30, 70);
        double targetX = Math.Cos(angle) * distance;
        double targetY = Math.Sin(angle) * distance;

        Storyboard sb = new Storyboard();

        DoubleAnimation animX = new DoubleAnimation(0, targetX, TimeSpan.FromSeconds(0.6));
        Storyboard.SetTarget(animX, dot);
        Storyboard.SetTargetProperty(animX, new PropertyPath("(Canvas.Left)"));

        DoubleAnimation animY = new DoubleAnimation(0, targetY, TimeSpan.FromSeconds(0.6));
        Storyboard.SetTarget(animY, dot);
        Storyboard.SetTargetProperty(animY, new PropertyPath("(Canvas.Top)"));

        DoubleAnimation animFade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.6));
        Storyboard.SetTarget(animFade, dot);
        Storyboard.SetTargetProperty(animFade, new PropertyPath("Opacity"));

        sb.Children.Add(animX);
        sb.Children.Add(animY);
        sb.Children.Add(animFade);

        sb.Completed += (s, e) => ParticleCanvas.Children.Remove(dot);
        sb.Begin();
    }
    public VanishEffect()
    {
        InitializeComponent();
    }
}
