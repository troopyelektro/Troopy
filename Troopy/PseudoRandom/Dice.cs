using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Troopy.PseudoRandom
{
    public class Dice
    {
        private int wallCount;
        public int WallCount {
            get {
                return this.wallCount;
            }
            set {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("walls");
                }
                wallCount = value;
            }
        }
        
        
        private Random random;

        public Dice(int walls)
        {
            if (walls < 1) {
                throw new ArgumentOutOfRangeException("walls");
            }
            wallCount = walls;
            random = new Random();
        }

        public int roll()
        {
            return random.Next(1, wallCount + 1);
        }

        public override string ToString()
        {
            return $"Rolling Dice with {wallCount} walls";
        }
    }
}
