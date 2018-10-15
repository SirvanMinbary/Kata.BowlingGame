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

                if (currentFrameCount > MaxFrames)
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
            for (int i = 0; i < MaxFrames - 1; i++)
            {
                Frames.Add(new Frame());
            }
        }

        public void Roll(int pins)
        {
            CheckExceptions(pins);

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

        private void CheckExceptions(int pins)
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
        }

        public int Score()
        {
            int score = 0;
            for (int i = 0; i < Frames.Count; i++)
            {
                var frame = Frames[i];
                if (!frame.AdvanceNextFrame)
                {
                    continue;
                }

                switch (frame.Modifier)
                {
                    case ScoreModifier.None:
                        score += CalculateNormalPoints(frame);
                        break;
                    case ScoreModifier.Spare:
                        score += CalculateSparePoints(i);
                        break;
                    case ScoreModifier.Strike:
                        score += CalculateStrikePoints(i);
                        break;
                    default:
                        break;
                }
            }

            score += LastFrame.Score;

            return score;
        }

        private int CalculateNormalPoints(Frame frame)
        {
            return frame.FirstRoll.Value + frame.SecondRoll.Value;
        }

        private int CalculateSparePoints(int frameIndex)
        {
            var nextIndex = frameIndex++;
            if (nextIndex == 10)
            {
                return 10 + LastFrame.FirstRoll.Value;
            }

            var nextFrame = Frames[nextIndex];

            return 10 + nextFrame.FirstRoll.Value;
        }

        private int CalculateStrikePoints(int frameIndex)
        {
            int score = 0;

            var strikeFrame = Frames[frameIndex];
            score += strikeFrame.FirstRoll.Value;

            var nextIndex = frameIndex + 1;
            if (nextIndex == 9)
            {
                switch (LastFrame.Modifier)
                {
                    case ScoreModifier.None:
                    case ScoreModifier.Spare:
                        score += LastFrame.FirstRoll.Value + LastFrame.SecondRoll.Value;
                        break;
                    case ScoreModifier.Strike:
                        score += LastFrame.FirstRoll.Value + LastFrame.SecondRoll.Value + LastFrame.ThirdRoll.Value;
                        break;
                }
            }
            else
            {
                var nextRoll = Frames[nextIndex];
                score += nextRoll.FirstRoll.Value;
                if (nextRoll.Modifier == ScoreModifier.Strike)
                {
                    score += nextRoll.FirstRoll.Value;
                    if (nextIndex + 1 == 10)
                    {
                        score += LastFrame.FirstRoll.Value;
                    }
                    else
                    {
                        if (nextIndex + 1 == 9)
                        {
                            score += LastFrame.FirstRoll.Value;
                        }
                        else
                        {
                            nextRoll = Frames[nextIndex + 1];
                            score += nextRoll.FirstRoll.Value;
                        }
                    }
                }
                else
                {
                    score += nextRoll.SecondRoll.Value;
                }
            }


            return score;
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

        public int Score
        {
            get
            {
                switch (Modifier)
                {
                    case ScoreModifier.None:
                    case ScoreModifier.Spare:
                        return FirstRoll.Value + SecondRoll.Value + ThirdRoll.Value;
                    case ScoreModifier.Strike:
                        return FirstRoll.Value + ThirdRoll.Value;
                    default:
                        throw new InvalidOperationException("Unknown last frame value");
                }
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
            else if (!SecondRoll.HasValue)
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
