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
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game();
        }

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

        [Test]
        public void Game_Strike_IsRegistered()
        {
            game.Roll(10);
            var firstFrameModifier = game.Frames[0].Modifier;
            Assert.That(firstFrameModifier == ScoreModifier.Strike, "Strike was not registered");
        }

        [Test]
        public void Game_AllStrikesReached()
        {
            for (int i = 0; i < 10; i++)
            {
                game.Roll(10);
            }

            Assert.IsTrue(game.Frames.TrueForAll(x => x.FirstRoll.Value == 10 && !x.SecondRoll.HasValue));
            Assert.IsTrue(game.LastFrame.FirstRoll.Value == 10);
            Assert.IsTrue(!game.LastFrame.SecondRoll.HasValue);
            Assert.IsTrue(game.LastFrame.ThirdRoll.Value == 10);
        }

        [Test]
        public void Game_Spare_IsRegistered()
        {
            game.Roll(5);
            game.Roll(5);
            var firstFrameModifier = game.Frames[0].Modifier;
            Assert.That(firstFrameModifier == ScoreModifier.Spare, "Spare was not registered");
        }

        [Test]
        public void Game_Frame_Advances()
        {
            for (int i = 0; i < 3; i++)
            {
                game.Roll(0);
            }
            var secondFrame = game.Frames[1];
            Assert.That(secondFrame.FirstRoll.HasValue);
        }

        [Test]
        public void Game_LastFrame_Registered()
        {
            for (int i = 0; i < 18; i++)
            {
                game.Roll(0);
            }

            Assert.That(game.CurrentState == GameState.LastFrame);
        }

        [Test]
        public void Game_LastFrameThirdRollReached()
        {
            for (int i = 0; i < 20; i++)
            {
                game.Roll(0);
            }

            Assert.That(game.LastFrame.ThirdRoll.HasValue);
        }

        [Test]
        public void Game_AllFramesReached()
        {
            for (int i = 0; i < 20; i++)
            {
                game.Roll(0);
            }

            var frames = game.Frames;
            var lastFrame = game.LastFrame;
            Assert.IsTrue(frames.TrueForAll(x => x.FirstRoll.HasValue && x.SecondRoll.HasValue));
            Assert.IsTrue(lastFrame.FirstRoll.HasValue && lastFrame.SecondRoll.HasValue && lastFrame.ThirdRoll.HasValue);
        }
    }
}
