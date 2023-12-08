using System;
using Saving;
using UnityEngine;

public class GridSaver : MonoBehaviour, IHasPuzzleData
{
    [SerializeField] private GridPort _gridPort;

    private void OnEnable()
    {
        AddListenerToSaveManager();
    }
    
    private void OnDisable()
    {
        RemoveListenerFromSaveManager();
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder, GridGenerationType gridGenerationType)
    {
        bool allPermanent = gridGenerationType == GridGenerationType.empty;
        
        int i = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                dataHolder.numbers[i] = _gridPort.tileBehaviours[row, col].number;

                if (!allPermanent)
                {
                    dataHolder.permanent.Add(_gridPort.tileBehaviours[row, col].Permanent);
                }
                
                // // on a self created puzzle, every tile is considered permanent
                // dataHolder.permanent[i] = allPermanent ? true : _gridPort.tileBehaviours[row, col].Permanent;
                
                i++;
            }
        }
    }

    public void LoadFromSaveData(PuzzleDataHolder dataHolder)
    {
        throw new NotImplementedException();
    }

    public void AddListenerToSaveManager()
    {
        SaveManager.AddPuzzleDataListener(this);
    }

    public void RemoveListenerFromSaveManager()
    {
        SaveManager.RemovePuzzleDataListener(this);
    }
}