namespace Saving
{
    public interface IPopulatePuzzleData
    {
        void PopulateSaveData(PuzzleDataHolder dataHolder, bool newSelfCreate);
    }
    
    public interface ILoadPuzzleData
    {
        void LoadFromSaveData(PuzzleDataHolder dataHolder);
    }
}