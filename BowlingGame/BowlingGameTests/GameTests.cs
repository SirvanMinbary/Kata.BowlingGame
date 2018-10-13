using BowlingGame;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingGameTests
{
    [TestFixture]
    public class GameTests
    {
        private Game game = new Game();

        [TestCase(-1, "Can not knock down less than zero pins")]
        [TestCase(11, "Can not knock down more than 10 pins")]
        public void Game_RollIncorrectPins_ThrowsException(int pins, string expectedMessage)
        {
            var ex = Assert.Throws<ArgumentException>(() => game.Roll(pins));
            Assert.That(ex.Message == expectedMessage);
        }

        [Test]
        public void Game_InitCreatesCorrectNumberOfFrames()
        {
            var frameCount = game.Frames.Count + 1;
            Assert.AreEqual(frameCount, game.MaxFrames);
        }
    }
}
