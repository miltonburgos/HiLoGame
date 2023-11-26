namespace Game;

public class HiLoGame
{
    private readonly HiLoGameOptions _hiLoGameOptions;
    private readonly IMessage _message;
    private HiLoGameInfo _gameInfo;

    private readonly Random _random = new();
    private readonly int _maxNumberOfPlayers;
    private readonly int _maxNumberOfRange;
    private const int MAX_PLAYER_NAME_CHARACTERS = 20;

    public HiLoGame(HiLoGameOptions hiLoGameOptions, IMessage message, HiLoGameInfo gameInfo)
    {
        _hiLoGameOptions = hiLoGameOptions;
        _message = message;
        _gameInfo = gameInfo;

        _maxNumberOfPlayers = _hiLoGameOptions.MaxNumberOfPlayers;
        _maxNumberOfRange = _hiLoGameOptions.MaxNumberOfRange;

        var continueTheGame = true;
        while (continueTheGame)
        {
            Start();           
            continueTheGame = Restart(continueTheGame);
        }
    }

    private void Start()
    {
        _message.Write("\nHI-LO game: you need to find the mystery number to win the game.\n");

        _gameInfo = new(SetupNumberOfPlayers(), SetupMysteryNumberRange());

        SetupPlayers(_gameInfo);

        _message.Write($"\nRandom mystery numbers generated for each player between 1 and {_gameInfo.MaxNumberToGuess}");

        Guess(_gameInfo);
    }

    private void Guess(HiLoGameInfo gameInfo)
    {
        int round = 1;
        bool gameFinished = false;
        while (!gameFinished)
        {
            gameFinished = PlayRound(gameInfo, round, gameFinished);
            round++;
        }
    }

    private static bool IsValidNumberInput(string input, out int number) => int.TryParse(input, out number);

    private bool IsValidNumberOfPlayers(int numberOfPlayers) => numberOfPlayers >= 1 && numberOfPlayers <= _maxNumberOfPlayers;

    private bool IsValidMysteryNumberRange(int numberOfRange) => numberOfRange >= 1 && numberOfRange <= _maxNumberOfRange;

    private static bool IsValidGuess(int guess, int maxNumberToGuess) => guess >= 1 && guess <= maxNumberToGuess;

    private static bool IsValidPlayerName(string playerName) => playerName.Trim() != string.Empty && playerName.Trim().Length <= MAX_PLAYER_NAME_CHARACTERS;

    private static bool IsValidRestartOption(string option) => option!.ToLower() == "y" || option.ToLower() == "n";


    private int SetupNumberOfPlayers()
    {
        var numberOfPlayers = 0;
        while (!IsValidNumberOfPlayers(numberOfPlayers))
        {
            _message.Write($"Choose the number of players between 1 and {_maxNumberOfPlayers}");
            if (!IsValidNumberInput(_message.Read()!, out numberOfPlayers) || !IsValidNumberOfPlayers(numberOfPlayers))
            {
                _message.Write($"Invalid input: number of players must be between 1 and {_maxNumberOfPlayers}\n");
            }
        }
        return numberOfPlayers;
    }

    private int SetupMysteryNumberRange()
    {
        var numberOfRange = 0;
        while (!IsValidMysteryNumberRange(numberOfRange))
        {
            _message.Write($"Choose the max range for the mystery number until {_maxNumberOfRange}");
            if (!IsValidNumberInput(_message.Read()!, out numberOfRange) || !IsValidMysteryNumberRange(numberOfRange))
            {
                _message.Write($"Invalid input: mistery number range must be between 1 and {_maxNumberOfRange}\n");
            }
        }
        return numberOfRange;
    }

    private void SetupPlayers(HiLoGameInfo gameInfo)
    {
        for (int i = 1; i <= gameInfo.NumberOfPlayers; i++)
        {
            var playerName = string.Empty;
            while (!IsValidPlayerName(playerName!))
            {
                _message.Write($"\nType the name of Player {i}");
                playerName = _message.Read();
                if (playerName == null || !IsValidPlayerName(playerName))
                {
                    _message.Write($"Invalid name: player name characters must be between 1 and {MAX_PLAYER_NAME_CHARACTERS}");
                }
            }            
            HiLoPlayerInfo player = new()
            {
                PlayerName = playerName,
                MysteryNumber = _random.Next(1, gameInfo.MaxNumberToGuess)
            };
            _gameInfo.Players?.Add(player);
        }
    }

    private bool PlayRound(HiLoGameInfo gameInfo, int round, bool gameFinished)
    {
        _message.Write("\n### Round " + round + " starting now ###");
        foreach (HiLoPlayerInfo player in gameInfo.Players!)
        {
            int guess = Guess(gameInfo, player);

            if (guess == player.MysteryNumber)
            {
                _message.Write($"{player.PlayerName}: you won the game after {round} rounds");
                _message.Write($"Check the guess numbers for each round: {string.Join(",", player.GuessNumbers!)}");
                gameFinished = true;
            }
            else
            {
                _message.Write($"{player.PlayerName}: you need to guess {(guess > player.MysteryNumber ? Enum.GetName(GuessOptions.LO) : Enum.GetName(GuessOptions.HI))} than {guess}");
            }
        }
        return gameFinished;
    }

    private int Guess(HiLoGameInfo gameInfo, HiLoPlayerInfo player)
    {
        int guess = 0;
        while (!IsValidGuess(guess, gameInfo.MaxNumberToGuess))
        {
            _message.Write($"\nNow it's your turn {player.PlayerName} to guess the mystery number");
            if (!IsValidNumberInput(_message.Read()!, out guess) || !IsValidGuess(guess, gameInfo.MaxNumberToGuess))
            {
                _message.Write($"Invalid input: guess must be number between 1 and  {gameInfo.MaxNumberToGuess}\n");
            }
        }
        player.GuessNumbers!.Add(guess);
        return guess;
    }

    private bool Restart(bool restart)
    {
        string? option = string.Empty;
        while (!IsValidRestartOption(option))
        {
            _message.Write("\nRestart the game (y/n): ");
            option = _message.Read()!;
            if (!IsValidRestartOption(option))
            {
                _message.Write("Invalid option: should choose y or n");
            }
            else if (option == "n")
            {
                restart = false;
                _message.Write("Thanks to play the game.");
            }
        }
        return restart;
    }    
}

public class HiLoGameOptions
{
    public int MaxNumberOfPlayers { get; set; }

    public int MaxNumberOfRange { get; set; }
}

public class HiLoGameInfo
{
    public int NumberOfPlayers { get; init; }

    public int MaxNumberToGuess { get; init; }

    public List<HiLoPlayerInfo> Players { get; init; } = new();

    public HiLoGameInfo() { }

    public HiLoGameInfo(int numberOfPlayers, int maxNumberToGuess)
    {
        NumberOfPlayers = numberOfPlayers;
        MaxNumberToGuess = maxNumberToGuess;
    }
}

public class HiLoPlayerInfo
{
    public string? PlayerName { get; set; }

    public int MysteryNumber { get; set; }

    public List<int>? GuessNumbers { get; set; } = new();
}

public enum GuessOptions
{
    HI,
    LO    
}

public interface IMessage
{
    void Write(string message);

    string? Read();
}

public class ConsoleMessage : IMessage
{
    public string? Read() => Console.ReadLine();

    public void Write(string message) => Console.WriteLine(message);
}
