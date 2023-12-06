using System;

namespace Saving
{
    [System.Serializable]
    public class SudokuGameData
    {
        // identifier
        public string id;
    
        public string name;
        public int difficulty;
            
        // grid
        public int[] numbers;
        public bool[] permanent;
            
        public SudokuGameData()
        {
            numbers = new int[81];
            permanent = new bool[81];
        }
            
        public SudokuGameData(string seed) //: base()
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