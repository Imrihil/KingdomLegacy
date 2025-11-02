using FluentAssertions;
using KingdomLegacy.Domain;

namespace KingdomLegacy.Tests;

public class GameTests
{
    private readonly Game _game = new();

    [Fact]
    public void ShouldImportGame()
    {
        _game.Load(SampleGameData);

        _game.IsInitialized.Should().BeTrue();
        _game.BoxCount.Should().Be(5);
        _game.DeckCount.Should().Be(8);
        _game.InPlay.Count.Should().Be(0);
        _game.Hand.Count.Should().Be(4);
        _game.DiscardedCount.Should().Be(0);
        _game.TrashCount.Should().Be(1);
        _game.Points.Should().Be(1);
    }

    private const string SampleGameData = @"Test
1
FeudalKingdom
0	1	7
1	1	2
2	1	4
3	1	2
4	1	2
5	2	4
6	2	2
7	1	2
8	1	2
9	1	4
10	2	2
11	1	2
12	1	4
13	1	0
14	1	0
15	1	0
16	1	0
17	1	0";
}