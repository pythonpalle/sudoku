namespace Saving
{
    public interface IHasPuzzleData
    {
        void PopulateSaveData(PuzzleDataHolder dataHolder, GridGenerationType gridGenerationType);
        void LoadFromSaveData(PuzzleDataHolder dataHolder);

        void AddListenerToSaveManager();
        void RemoveListenerFromSaveManager();
    }

    public interface ISavePuzzleData
    {
        
    }
    
    public interface ILoadPuzzleData
    {
        
    }
}