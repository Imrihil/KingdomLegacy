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

    private const string SampleGameData = @"1
FeudalKingdom
0	1	5
1	1	1
2	1	2
3	1	1
4	1	1
5	2	2
6	2	1
7	1	1
8	1	1
9	1	2
10	2	1
11	1	1
12	1	2
13	1	0
14	1	0
15	1	0
16	1	0
17	1	0";
}