using System;
using System.Collections.Generic;
using Command;

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
        public bool[] permanent;
        
        // marks
        public List<int>[] cornerMarks;
        public List<int>[] centerMarks;
        public List<int>[] colorMarks;

        // public List<SerializedCommandData> commands = new List<SerializedCommandData>();
        // public int commandCounter;

        // commands
        public List<SudokuCommand> undoCommands = new List<SudokuCommand>();
        public List<SudokuCommand> redoCommands = new List<SudokuCommand>();
        //public int commandCounter;


        // status
        public bool completed;

        public PuzzleDataHolder()
        {
            InstantiateArrays();
        }

        private void InstantiateArrays()
        {
            numbers = new int[81];
            permanent = new bool[81];

            cornerMarks = new List<int>[81];
            centerMarks = new List<int>[81];
            colorMarks = new List<int>[81];
        }

        public PuzzleDataHolder(string seed) //: base()
        {
            InstantiateArrays();

            for (int i = 0; i < seed.Length; i++)
            {
                var digitChar = seed[i];
                
                if (digitChar == '0' || digitChar == ' ')
                    continue;
                
                int digitNumber = (int)Char.GetNumericValue(digitChar);
                numbers[i] = digitNumber;
            }
        }

        public void Reset()
        {
            undoCommands.Clear();
            redoCommands.Clear();
            //commandCounter = 0;

            foreach (var mark in colorMarks)
            {
                mark.Clear();
            }
            
            foreach (var mark in cornerMarks)
            {
                mark.Clear();
            }
            
            foreach (var mark in centerMarks)
            {
                mark.Clear();
            }
            
            for (int i = 0; i < 81; i++)
            {
                if (permanent[i])
                    continue;

                numbers[i] = 0;
            }
        }
    }

    [System.Serializable]
    public class SerializedCommandData
    {
        public List<int> tiles;
        public int enterType;
        public int number;
        public bool removal;
        public bool colorRemoval;
    
        public SerializedCommandData(List<int> tiles, int enterType, int number, bool removal, bool colorRemoval)
        {
            this.tiles = tiles;
            this.enterType = enterType;
            this.number = number;
            this.removal = removal;
            this.colorRemoval = colorRemoval;
        }
    }
}