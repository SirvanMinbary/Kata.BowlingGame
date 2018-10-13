using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
        }
    }

    public class Game
    {
        public readonly int MaxFrames = 10;
        int currentFrame = 0;
        int previousKnockedDownPins = 0;
        int currentOpportunity = 0;

        public List<Frame> Frames { get; set; } = new List<Frame>();
        public LastFrame LastFrame { get; set; }

        public Game()
        {
            InitFrames();
        }

        public void Roll(int pins)
        {
            if (pins < 0)
            {
                throw new ArgumentException("Can not knock down less than zero pins");
            }

            if (pins > 10)
            {
                throw new ArgumentException("Can not knock down more than 10 pins");
            }
        }

        int Score()
        {
            throw new NotImplementedException();
        }

        private void InitFrames()
        {
            for (int i = 0; i < MaxFrames - 1; i++)
            {
                Frames.Add(new Frame());
            }
        }
    }

    public class Frame
    {
        public int? FirstRoll { get; set; }
        public int? SecondRoll { get; set; }
        public ScoreModifier Modifier { get; set; } = ScoreModifier.None;
    }

    public class LastFrame : Frame
    {
        public int? ThirdRoll { get; set; }
    }

    public enum ScoreModifier
    {
        None,
        Spare,
        Strike
    }
}
