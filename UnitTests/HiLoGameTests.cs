using FluentAssertions;

namespace Game;

public class HiLoGameTests
{
    private readonly TestableMessage _mockMessage;
    private readonly HiLoGameOptions _gameOptions;

    public HiLoGameTests()
    {
        _gameOptions = new HiLoGameOptions
        {
            MaxNumberOfPlayers = 4,
            MaxNumberOfRange = 1000
        };

        _mockMessage = new TestableMessage();       
    }

    [Fact]
    public void HiLoGame_WithNullMessageDependecyInjection_ShouldThrowArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new HiLoGame(_gameOptions, null!, new HiLoGameInfo()));
    }

    [Theory]
    [InlineData("1", "100")]
    [InlineData("2", "50")]
    [InlineData("3", "30")]
    public void HiLoGame_WhenUserSelectValidNumberOfPlayersAndRange_ShouldSetThoseValuesInGameInfo(string numberOfPlayers, string numberOfRange)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = numberOfPlayers;
        _mockMessage.MysteryNumberRangeInput = numberOfRange;
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo);
        game.Start();

        game.GameInfo.NumberOfPlayers.Should().Be(int.Parse(numberOfPlayers));
        game.GameInfo.MaxNumberToGuess.Should().Be(int.Parse(numberOfRange));
        game.GameInfo.Players.Count.Should().Be(int.Parse(numberOfPlayers));
        game.GameInfo.Players.All(player => player.PlayerName == _mockMessage.PlayerNameInput);
        game.Round.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("5")]
    public void HiLoGame_WhenInvalidNumberOfPlayersInput_ShouldReturnValidationMessage(string numberOfPlayers)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = numberOfPlayers;
        _mockMessage.MysteryNumberRangeInput = "100";
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo)
        {
            FinishGameWithInvalidInput = true
        };
        game.Start();

        game.GameInfo.NumberOfPlayers.Should().Be(0);
        _mockMessage.WriteMessage.Should().Be($"Invalid input: number of players must be between 1 and {_gameOptions.MaxNumberOfPlayers}\n");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1005")]
    public void HiLoGame_WhenInvalidMysteryNumberRangeInput_ShouldReturnValidationMessage(string numberOfRange)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = "2";
        _mockMessage.MysteryNumberRangeInput = numberOfRange;
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo)
        {
            FinishGameWithInvalidInput = true
        };
        game.Start();

        game.GameInfo.MaxNumberToGuess.Should().Be(0);
        _mockMessage.WriteMessage.Should().Be($"Invalid input: mistery number range must be between 1 and {_gameOptions.MaxNumberOfRange}\n");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789012345678901")]
    public void HiLoGame_WhenInvalidPlayerNameInput_ShouldReturnValidationMessage(string playerName)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = "2";
        _mockMessage.MysteryNumberRangeInput = "30";
        _mockMessage.PlayerNameInput = playerName;
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo)
        {
            FinishGameWithInvalidInput = true
        };
        game.Start();

        game.GameInfo.Players.Count().Should().Be(0);
        _mockMessage.WriteMessage.Should().Be($"Invalid name: player name characters must be between 1 and 20");
    }

    [Theory]
    [InlineData("0", "10")]
    [InlineData("50", "10")]
    [InlineData("1500", "1000")]
    public void HiLoGame_WhenInvalidGuessInput_ShouldReturnValidationMessage(string guess, string mysteryNumberRange)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = "2";
        _mockMessage.MysteryNumberRangeInput = mysteryNumberRange;
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.GuessInput = guess;
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo)
        {
            FinishGameWithInvalidInput = true
        };
        game.Start();

        _mockMessage.WriteMessage.Should().Be($"Invalid input: guess must be number between 1 and  {mysteryNumberRange}\n");
    }

    [Theory]
    [InlineData("aaaa")]
    [InlineData("yessss")]
    public void HiLoGame_WhenInvalidRestartTheGameInput_ShouldReturnValidationMessage(string restartTheGame)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = "2";
        _mockMessage.MysteryNumberRangeInput = "30";
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.RestartTheGameInput = restartTheGame;

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo)
        {
            FinishGameWithInvalidInput = true
        };
        game.Start();

        _mockMessage.WriteMessage.Should().Be("Invalid option: should choose y or n");
    }

    [Fact]
    public void HiLoGame_WhenValidInput_ShouldReturnMessageAfterWinTheGame()
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();
        _mockMessage.NumberOfPlayersInput = "1";
        _mockMessage.MysteryNumberRangeInput = "30";
        _mockMessage.PlayerNameInput = "test";
        _mockMessage.RestartTheGameInput = "n";

        // Act
        var game = new HiLoGame(_gameOptions, _mockMessage, gameInfo);
        game.Start();
        _mockMessage.WonGameMessage.Should().Be($"{_mockMessage.PlayerNameInput}: you won the game after {game.Round} rounds\nCheck the guess numbers for each round: {string.Join(",", game.GameInfo.Players.First().GuessNumbers!)}");
    }
}
public class TestableMessage : IMessage
{
    public string? NumberOfPlayersInput { get; set; }

    public string? MysteryNumberRangeInput { get; set; }

    public string? PlayerNameInput { get; set; }

    public string? GuessInput { get; set; }

    public string? RestartTheGameInput { get; set; }

    public string? WriteMessage { get; set; }

    public string? WonGameMessage { get; set; }

    public string? ReadNumberOfPlayers() => NumberOfPlayersInput;

    public string? ReadMysteryNumberRange() => MysteryNumberRangeInput;

    public string? ReadPlayerName() => PlayerNameInput;

    public string? ReadGuess()
    {
        if (GuessInput == null)
        {
            Random random = new();
            return random.Next(1, int.Parse(MysteryNumberRangeInput!)).ToString();
        }
        else { return GuessInput; }
    }

    public string? ReadRestartTheGame() => RestartTheGameInput;

    public void Write(string message) => WriteMessage = message;

    public void WonGame(string message) => WonGameMessage = message;
}
