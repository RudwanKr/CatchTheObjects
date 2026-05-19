using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace CatchTheObjects.Models;

public partial class GameObject : ObservableObject
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    public TimeSpan FallDuration { get; set; }
    public ObjectType Type { get; set; }
}

public partial class ObjectType
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public double Speed { get; set; }
    public int Points { get; set; }
    public double SpeedMultiplier { get; set; } = 1;
    public Color ThemeColor { get; set; }
}