namespace Saving
{
    public interface IHasPuzzleData
    {
        void PopulateSaveData(PuzzleDataHolder dataHolder);
        void LoadFromSaveData(PuzzleDataHolder dataHolder);
    }
}