using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // serialize fields
    [Header("Background")]
    [SerializeField] private Image border;
    [SerializeField] private Image whitePart;
    
    [Header("Digit")]
    [SerializeField] private TextMeshProUGUI numberText;

    [Header("Corner")]
    [SerializeField] private List<TextMeshProUGUI> cornerTextList;
    [SerializeField] private GameObject cornerTextsParent;
    
    [Header("Center")]
    [SerializeField] private TextMeshProUGUI centerText;
    
    [Header("Scriptable objects")]
    [SerializeField] private SelectionObject selectionObject;
    
    // public fields
    public int row { get; private set; }
    public int col { get; private set; }
    public int number { get; private set; }

    private bool HasDigit => number > 0;

    public bool Permanent { get; private set; } = false;
    public bool Contradicted { get; private set; } = false;

    private List<int> centerMarks { get; set; } = new List<int>();
    //private List<int> CornerMarks { get; set; } = new List<int>();
    public List<int> CornerMarks;

    // private fields
    private bool isSelected { get; set; } = false;

    private float timeOfLastClick;
    private const float maxTimeForDoubleClick = 0.2f;
    
    private Vector3 whitePartSelectScale = Vector3.one * 0.9f;
    private Vector3 whitePartStartScale;
    
    private void Start()
    {
        whitePartStartScale = whitePart.transform.localScale;
    }

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
        
        EventManager.SetTileIndex(row, col, this);
    }

    public void SetStartNumber(int number)
    {
        SetDigit(number);
        
        if (number > 0)
        {
            Permanent = true;
        }
    }

    private void SetDigit(int number)
    {
        this.number = number;
        numberText.text = number > 0 ? number.ToString() : string.Empty;
        if (number == 0)
        {
            RemoveContradiction();
            cornerTextsParent.SetActive(true);
            centerText.gameObject.SetActive(true);
        }
        else
        {
            cornerTextsParent.SetActive(false);
            centerText.gameObject.SetActive(false);
        }
    }

    private bool TryUpdateDigit(int number, bool remove)
    {
        if (Permanent) return false;

        if (remove) number = 0;
        SetDigit(number);
        numberText.color = Color.blue;
        return true;
    }
    
    public bool TryUpdateNumber(int number, EnterType enterType, bool remove)
    {
        if (Permanent) return false;

        switch (enterType)
        {
            case EnterType.DigitMark:
                return TryUpdateDigit(number, remove);
            
            case EnterType.CornerMark:
                return TryUpdateCorner(number, remove);
            
            case EnterType.CenterMark:
                return TryUpdateCenter(number, remove);
        }

        return false;
    }
    

    private bool TryUpdateCorner(int addedNumber, bool remove)
    {
        if (Permanent || HasDigit) return false;
        
        Debug.Log("Added number: " + addedNumber);

        //numberText.gameObject.SetActive(false);
        // cornerTextsParent.SetActive(true);
        // centerText.gameObject.SetActive(true);

        if (remove && CornerMarks.Contains(addedNumber))
        {
            CornerMarks.Remove(addedNumber);
        }
        else
        {
            if (CornerMarks.Contains(addedNumber))
                return false;
            
            CornerMarks.Add(addedNumber);
        }

        SortCornerMarks();
        return true;
    }

    private void SortCornerMarks()
    {
        CornerMarks.Sort();
        int numbersInCorner = CornerMarks.Count;
        for (int i = 0; i < cornerTextList.Count; i++)
        {
            bool outOfIndex = i >= numbersInCorner;
            if (outOfIndex)
            {
                cornerTextList[i].text = string.Empty;
            }
            else
            {
                cornerTextList[i].text = CornerMarks[i].ToString();
            }
        }
    }
    
    private bool TryUpdateCenter(int i, bool remove)
    {
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleTrySelect();
        EventManager.UIElementHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool doubleClick = Time.time < timeOfLastClick + maxTimeForDoubleClick;
        if (doubleClick && number > 0)
        {
            EventManager.SelectAllTilesWithNumber(number);
        }
        else
        {
            HandleTrySelect();
        }

        timeOfLastClick = Time.time;
    }

    private bool HandleTrySelect()
    {
        if (!selectionObject.IsSelecting || isSelected)
        {
            return false;
        }

        HandleSelect();
        return true;
    }

    public void HandleSelect()
    {
        isSelected = true;
        border.color = Color.blue;
        whitePart.transform.localScale = whitePartSelectScale;
        EventManager.SelectTile(this);
    }

    public void Deselect()
    {
        isSelected = false;
        border.color = Color.black;
        whitePart.transform.localScale = whitePartStartScale;
    }

    public void SetContradiction()
    {
        Contradicted = true;
        whitePart.color = Color.red;
    }

    public void RemoveContradiction()
    {
        Contradicted = false;
        whitePart.color = Color.white;
    }

    public bool HasSameNumber(int number, EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                return this.number == number;
            
            case EnterType.CenterMark:
                return !HasDigit && centerMarks.Contains(number);
               
            
            case EnterType.CornerMark:
                return !HasDigit && CornerMarks.Contains(number);
        }

        return false;
    }
}