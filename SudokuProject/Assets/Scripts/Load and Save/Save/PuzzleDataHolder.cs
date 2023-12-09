using System;
using System.Collections.Generic;

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
        
        // // TODO: add creation time for sorting purposes
        //public float creationTime;
            
        // grid
        public int[] numbers;
        public List<bool> permanent = new List<bool>();
        
        
        // status
        public bool completed;
            
        public PuzzleDataHolder()
        {
            numbers = new int[81];
            List<bool> permanent = new List<bool>();
        }
            
        public PuzzleDataHolder(string seed) //: base()
        {
            numbers = new int[81];
            permanent = new List<bool>();
                
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