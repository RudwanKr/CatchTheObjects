using CatchTheObjects.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Media;
using System.Windows.Media;

namespace CatchTheObjects.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isGameRunning;

    [ObservableProperty]
    private int _score;

    [ObservableProperty]
    private double _basketX;

    [ObservableProperty]
    private string _title = "Catch The Objects";

    [ObservableProperty]
    private string _description = "Collect fruits and survive!";

    private bool _isGameOver = false;
    private readonly List<ObjectType> _normalObjects;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LivesCollection))]
    private int _lives;

    private readonly Random _random = new();

    private readonly SoundPlayer _catchSound;
    private readonly SoundPlayer _missSound;
    private readonly SoundPlayer _heartSound;
    private readonly SoundPlayer _gameOverSound;
    private readonly MediaPlayer _bgMusic;

    public event Action<GameObject>? ItemMissed;
    public event Action<GameObject>? ItemCaught;

    private double _spawnTimer;
    private double _spawnInterval = 1.6; // seconds

    private double _gameTime;

    public ObservableCollection<int> LivesCollection =>
            new ObservableCollection<int>(Enumerable.Range(0, Lives));

    private readonly List<ObjectType> _objectTypes = new()
    {
        new ObjectType { Name = "Apple", Icon = "pack://application:,,,/images/apple.png", Speed = 1, Points = 5, ThemeColor = Colors.Red },
        new ObjectType { Name = "Banana", Icon = "pack://application:,,,/images/banana.png", Speed = 1.5, Points = 10, ThemeColor = Colors.Yellow },
        new ObjectType { Name = "Cherry", Icon = "pack://application:,,,/images/cherry.png", Speed =2, Points = 15, ThemeColor = Colors.Pink },
        new ObjectType { Name = "Orange", Icon = "pack://application:,,,/images/orange.png", Speed = 2.5, Points = 20, ThemeColor = Colors.Orange },
        new ObjectType { Name = "Grapes", Icon = "pack://application:,,,/images/grapes.png", Speed = 3, Points = 25, ThemeColor = Colors.Purple },
        new ObjectType { Name = "Strawberry", Icon = "pack://application:,,,/images/Strawberry.png", Speed =3.5, Points = 30, ThemeColor = Colors.HotPink },
        new ObjectType { Name = "GoldHeart", Icon = "pack://application:,,,/images/heart.png", Speed =2.5, Points = 0, ThemeColor = Colors.Gold }
    };

    public ObservableCollection<GameObject> Items { get; } = new();

    public MainViewModel()
    {
        BasketX = 375;

        _normalObjects = _objectTypes
            .Where(x => x.Name != "GoldHeart")
            .ToList();

        _catchSound = new SoundPlayer("sounds/catch.wav");
        _missSound = new SoundPlayer("sounds/miss.wav");
        _heartSound = new SoundPlayer("sounds/heart.wav");
        _gameOverSound = new SoundPlayer("sounds/gameover.wav");
        _bgMusic = new MediaPlayer();

        _catchSound.Load();
        _missSound.Load();
        _heartSound.Load();
        _gameOverSound.Load();

        _bgMusic.MediaEnded += BgMusic_MediaEnded;
    }

    public void Update(double deltaTime)
    {
        if (!IsGameRunning)
            return;
        _gameTime += deltaTime;

        UpdateDifficulty();

        HandleSpawning(deltaTime);

        UpdateObjects(deltaTime);
    }

    private void UpdateDifficulty()
    {
        _spawnInterval = Math.Max(0.35, 1.6 - (_gameTime / 60));

        foreach (var type in _objectTypes)
        {
            type.SpeedMultiplier = 1 + (_gameTime / 120);
        }
    }

    private void resetDifficulty()
    {
        _gameTime = 0;
        _spawnTimer = 0;
        _spawnInterval = 1.6;
        foreach (var type in _objectTypes)
        {
            type.SpeedMultiplier = 1;
        }
    }

    private void HandleSpawning(double deltaTime)
    {
        _spawnTimer += deltaTime;

        if (_spawnTimer < _spawnInterval)
            return;

        _spawnTimer = 0;

        SpawnObject();
    }

    private void SpawnObject()
    {
        ObjectType randomType;

        if (_random.NextDouble() < 0.05)
        {
            randomType = _objectTypes.First(x => x.Name == "GoldHeart");
        }
        else
        {
            randomType = _normalObjects[_random.Next(_normalObjects.Count)];
        }

        double spawnX;

        int attempts = 0;

        do
        {
            spawnX = _random.Next(0, 720);
            attempts++;
        }
        while (
            Items.Any(x => Math.Abs(x.X - spawnX) < 60 && x.Y < 120)
            && attempts < 10
        );

        Items.Add(new GameObject
        {
            X = spawnX,
            Y = -50,
            Type = randomType
            //FallDuration = TimeSpan.FromSeconds(
            //    700 / (randomType.Speed * randomType.SpeedMultiplier * 10)
            //)
        });
    }

    private void UpdateObjects(double deltaTime)
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var item = Items[i];

            double speed = item.Type.Speed * item.Type.SpeedMultiplier * 220;

            item.Y += speed * deltaTime;

            bool caught = item.Y > 430 && item.Y < 480 &&
                Math.Abs((item.X + 22) - (BasketX + 55)) < 60;

            if (caught)
                HandleCatch(item);
            else if (item.Y > 485)
                HandleMiss(item);
        }
    }

    private void HandleCatch(GameObject item)
    {
        if (item.Type.Name == "GoldHeart")
        {
            Lives++;
            _heartSound.Play();
        }
        else
        {
            Score += item.Type.Points;
            ItemCaught?.Invoke(item);
            _catchSound.Play();
        }

        Items.Remove(item);

        OnPropertyChanged(nameof(LivesCollection));
    }

    private void HandleMiss(GameObject item)
    {
        Items.Remove(item);

        if (item.Type.Name == "GoldHeart")
            return;

        Lives--;

        _missSound.Play();

        OnPropertyChanged(nameof(LivesCollection));

        ItemMissed?.Invoke(item);

        if (Lives <= 0)
        {
            GameOver();
        }
    }
    private void GameOver()
    {
        Lives = 0;
        _isGameOver = true;
        IsGameRunning = false;

        Title = "Game Over!";
        Description = $"Your final score: {Score}";

        resetDifficulty();

        _gameOverSound.Play();

        Items.Clear();
    }

    partial void OnBasketXChanged(double value)
    {
        const double minX = 0;
        const double maxX = 720;

        if (BasketX < minX)
            BasketX = minX;

        if (BasketX > maxX)
            BasketX = maxX;
    }

    [RelayCommand]
    private void StartGame()
    {
        Score = 0;
        Lives = 10;
        BasketX = 375;
        Items.Clear();
        IsGameRunning = true;
        StartMusic();

        OnPropertyChanged(nameof(LivesCollection));
    }

    public void StartMusic()
    {
        _bgMusic.Open(
            new Uri(
                AppDomain.CurrentDomain.BaseDirectory +
                "sounds/background.wav"
            )
        );

        _bgMusic.Volume = 0.3;

        _bgMusic.Play();
    }

    private void BgMusic_MediaEnded(object? sender, EventArgs e)
    {
        _bgMusic.Position = TimeSpan.Zero;
        _bgMusic.Play();
    }
}