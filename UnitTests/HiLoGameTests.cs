using FluentAssertions;
using Moq;
namespace Game;

public class HiLoGameTests
{
    private readonly Mock<IMessage> _mockMessage;
    private readonly HiLoGameOptions _hiLogGameOptions;
    private readonly HiLoGame _hiLoGame;

    public HiLoGameTests()
    {
        _hiLogGameOptions = new HiLoGameOptions
        {
            MaxNumberOfPlayers = 4,
            MaxNumberOfRange = 1000
        };

        _mockMessage = new Mock<IMessage>();

        _hiLoGame = new HiLoGame(_hiLogGameOptions, _mockMessage.Object, new HiLoGameInfo());
    }

    [Fact]
    public void HiLoGame_WithNullMessageDependecyInjection_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => new HiLoGame(_hiLogGameOptions, null!, new HiLoGameInfo());

        // Assert
        _ = action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(2, 100)]
    public void HiLoGame_WithNullMessageDependecyInjection_ShouldThrowArgumentNullException2(int numberOfPlayers, int numberOfRange)
    {
        // Arrange
        var gameInfo = new HiLoGameInfo();

        // Act
        var game = new HiLoGame(_hiLogGameOptions, null!, gameInfo);


    }
}
