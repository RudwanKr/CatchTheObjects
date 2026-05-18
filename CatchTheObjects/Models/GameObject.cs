using CommunityToolkit.Mvvm.ComponentModel;

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
}