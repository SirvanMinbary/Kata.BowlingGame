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
        int currentFrameCount = 0;
        public GameState CurrentState
        {
            get
            {
                if (currentFrameCount == MaxFrames - 1)
                {
                    return GameState.LastFrame;
                }

                if (currentFrameCount < MaxFrames)
                {
                    return GameState.InProgress;
                }

                if (currentFrameCount >= MaxFrames)
                {
                    return GameState.Finished;
                }

                throw new InvalidOperationException("Unknown game state");
            }
        }

        public List<Frame> Frames { get; set; } = new List<Frame>();
        public LastFrame LastFrame { get; set; } = new LastFrame();

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

            if (CurrentState == GameState.Finished)
            {
                throw new InvalidOperationException("Maximum amount of frames has been reached");
            }

            if (CurrentState == GameState.InProgress)
            {
                var currentFrame = Frames[currentFrameCount];
                currentFrame.AddRoll(pins);

                if (currentFrame.AdvanceNextFrame)
                {
                    currentFrameCount++;
                }
            }

            if (CurrentState == GameState.LastFrame)
            {
                LastFrame.AddRoll(pins);
                if (LastFrame.FinishGame)
                {
                    currentFrameCount++;
                }
            }

        }

        public int Score()
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
        public bool AdvanceNextFrame
        {
            get
            {
                if (Modifier == ScoreModifier.Strike || Modifier == ScoreModifier.Spare)
                {
                    return true;
                }
                else if (FirstRoll.HasValue && SecondRoll.HasValue)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual void AddRoll(int pins)
        {
            if (!FirstRoll.HasValue)
            {
                FirstRoll = pins;

                if (pins == 10)
                {
                    Modifier = ScoreModifier.Strike;
                }
            }
            else
            {
                SecondRoll = pins;

                if (FirstRoll.Value + SecondRoll.Value == 10)
                {
                    Modifier = ScoreModifier.Spare;
                }
            }
        }
    }

    public class LastFrame : Frame
    {
        public int? ThirdRoll { get; set; }
        public bool FinishGame
        {
            get
            {
                return ThirdRoll.HasValue;
            }
        }

        public override void AddRoll(int pins)
        {
            if (!FirstRoll.HasValue)
            {
                FirstRoll = pins;
                if (pins == 10)
                {
                    Modifier = ScoreModifier.Strike;
                }
            }
            else if (!SecondRoll.HasValue && Modifier != ScoreModifier.Strike)
            {
                SecondRoll = pins;
                if (FirstRoll.Value + SecondRoll.Value == 10)
                {
                    Modifier = ScoreModifier.Spare;
                }
            }
            else
            {
                ThirdRoll = pins;
            }
        }
    }

    public enum ScoreModifier
    {
        None,
        Spare,
        Strike
    }

    public enum GameState
    {
        Started,
        InProgress,
        LastFrame,
        Finished
    }
}
