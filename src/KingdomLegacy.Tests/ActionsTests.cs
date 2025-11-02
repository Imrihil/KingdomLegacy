using FluentAssertions;
using KingdomLegacy.Domain;
using KingdomLegacy.Domain.Logic;

namespace KingdomLegacy.Tests;
public class ActionsTests
{
    [Fact]
    public void Shoud_AllowDiscard()
    {
        var actions = new Actions(new Game());
        var cardActions = actions.GetCardActions(new Card()
        {
            State = State.Hand
        });
        cardActions.Should().Contain(action => typeof(FlipAndDiscard).IsInstanceOfType(action));
    }
}
