using System.Collections.Generic;

namespace Saving
{
    [System.Serializable]
    public class UserSaveData : SaveData
    {
        public UserSaveData(UserIdentifier identifier)
        {
            this.identifier = identifier;
            puzzles = new List<PuzzleDataHolder>();
        }
        
        public UserSaveData()
        {
            puzzles = new List<PuzzleDataHolder>();
        }

        public UserIdentifier identifier;// { get; private set; }
        public List<PuzzleDataHolder> puzzles;// { get; private set; }
    }
}

