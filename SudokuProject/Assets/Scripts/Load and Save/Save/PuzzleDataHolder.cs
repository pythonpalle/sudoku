using System;

namespace Saving
{
    [System.Serializable]
    public class PuzzleDataHolder
    {
        // identifier
        public string id;
    
        public string name;
        public int difficulty;
        public bool selfCreated;
            
        // grid
        public int[] numbers;
        public bool[] permanent;
        
        // status
        public bool completed;
            
        public PuzzleDataHolder()
        {
            numbers = new int[81];
            permanent = new bool[81];
        }
            
        public PuzzleDataHolder(string seed) //: base()
        {
            numbers = new int[81];
            permanent = new bool[81];
                
            for (int i = 0; i < seed.Length; i++)
            {
                var digitChar = seed[i];
                
                if (digitChar == '0' || digitChar == ' ')
                    continue;
                
                int row = i / 9;
                int col = i % 9;
    
                int digitNumber = (int)Char.GetNumericValue(digitChar);
                numbers[i] = digitNumber;
            }
        }
    }
}