using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
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

    public bool HasDigit => number > 0;

    public bool Permanent { get; private set; } = false;
    public bool Contradicted { get; private set; } = false;

    public List<int> centerMarks { get; set; } = new List<int>();
    public List<int> CornerMarks;

    // private fields
    private bool isSelected { get; set; } = false;

    private float timeOfLastClick;
    private const float maxTimeForDoubleClick = 0.2f;
    
    private Vector3 whitePartSelectScale = Vector3.one * 0.9f;
    private Vector3 whitePartStartScale;
    
    int centerMarkFontSize = 30;

    private void Start()
    {
        whitePartStartScale = whitePart.transform.localScale;
    }

    private void OnEnable()
    {
        selectionObject.OnDeselectAllTiles += OnDeselectAllTiles;
    }

    private void OnDisable()
    {
        selectionObject.OnDeselectAllTiles -= OnDeselectAllTiles;
    }

    private void OnDeselectAllTiles()
    {
        Deselect();
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
    
    private bool TryUpdateDigit(int number, bool remove)
    {
        if (Permanent) return false;

        if (remove) number = 0;
        SetDigit(number);
        numberText.color = Color.blue;
        return true;
    }
    

    private bool TryUpdateCorner(int addedNumber, bool remove)
    {
        if (Permanent || HasDigit) return false;
        
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
    
    private bool TryUpdateCenter(int addedNumber, bool remove)
    {
        if (Permanent || HasDigit) return false;
        
        if (remove && centerMarks.Contains(addedNumber))
        {
            centerMarks.Remove(addedNumber);
        }
        else
        {
            if (centerMarks.Contains(addedNumber))
                return false;
            
            centerMarks.Add(addedNumber);
        }

        SortCenterMarks();
        return true;
    }
    

    private void SortCornerMarks()
    {
        int numbersInCorner = CornerMarks.Count;
        List<int> indexOrder = GetCornerIndexOrder(numbersInCorner);

        CornerMarks.Sort();
        
        // reset string
        for (int i = 0; i < cornerTextList.Count; i++)
        {
            cornerTextList[i].text = string.Empty;
        }

        // set number in appropriate order
        for (int i = 0; i < numbersInCorner; i++)
        {
            cornerTextList[indexOrder[i]].text = CornerMarks[i].ToString();
        }
    }

    private List<int> GetCornerIndexOrder(int numbersInCorner)
    {
        List<int> indexes = new List<int>();

        switch (numbersInCorner)
        {
            case < 5:
                for (int i = 0; i < numbersInCorner; i++)
                    indexes.Add(i);
                break;
            
            case 5:
                indexes = new List<int> { 0, 4, 1, 2, 3 };
                break;
            
            case 6:
                indexes = new List<int> { 0, 4, 5, 1, 2, 3 };
                break;
            
            case 7:
                indexes = new List<int> { 0, 4, 5, 1, 2, 6, 3 };
                break;
            
            case 8:
                indexes = new List<int> { 0, 4, 5, 1, 2, 6, 7, 3 };
                break;
            
            case 9:
                indexes = new List<int> { 0, 4, 8, 5, 1, 2, 6, 7, 3 };
                break;
        }

        return indexes;
    }
    
    private void SortCenterMarks()
    {
        centerMarks.Sort();

        int centerMarkCount = centerMarks.Count;
        float centerStringSize = centerMarkFontSize;
        int maximumMarksForDefaultSize = 5;

        if (centerMarkCount > maximumMarksForDefaultSize)
        {
            int difference = centerMarkCount - maximumMarksForDefaultSize;

            centerStringSize *= Mathf.Pow(0.87f, difference);
        }

        string centerMarkString = string.Empty;
        foreach (var mark in centerMarks)
        {
            centerMarkString += mark.ToString();
        }

        centerText.text = centerMarkString;
        centerText.fontSize = centerStringSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleDragSelection();
        EventManager.UIElementHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("On Pointer Click");
        //OnClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer Down");
        OnClick();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected)
        {
            selectionObject.SetSelectionMode(SelectionMode.None);
        }
    }
    
    private void OnClick()
    {
        bool doubleClick = Time.time < timeOfLastClick + maxTimeForDoubleClick;
        if (doubleClick)
        {
            EventManager.SelectAllTilesWithNumber(this);
        }
        else
        {
            if (selectionObject.multiSelectKeyIsPressed && isSelected)
            {
                Debug.Log("Set Deselect!");
                selectionObject.SetSelectionMode(SelectionMode.Deselecting);
                Deselect();
            }
            else
            {
                if (!selectionObject.multiSelectKeyIsPressed)
                    selectionObject.DeselectAllTiles();
                
                selectionObject.SetSelectionMode(SelectionMode.Selecting);
                HandleSelect();
            }
        }

        timeOfLastClick = Time.time;
    }

    private bool HandleDragSelection()
    {
        if (selectionObject.IsSelecting)
        {
            HandleSelect();
            return true;
        } 
        else if (selectionObject.IsDeselecting && selectionObject.SelectionKeyDown)
        {
            Deselect();
            return true;
        }

        return false;
    }
    
    // temp function, change later
    public void HandleSelectPublic()
    {
        HandleSelect();
    }

    private void HandleSelect()
    {
        isSelected = true;
        border.color = Color.blue;
        whitePart.transform.localScale = whitePartSelectScale;
        EventManager.SelectTile(this);
    }

    private void Deselect()
    {
        isSelected = false;
        border.color = Color.black;
        whitePart.transform.localScale = whitePartStartScale;
        EventManager.DeselectTile(this);
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

    public void RemoveAllOfType(EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                TryUpdateNumber(0, EnterType.DigitMark, true);
                break;
            
            case EnterType.CenterMark:
                RemoveAllCenterMarks();
                break;
            
            case EnterType.CornerMark:
                RemoveAllCornerMarks();
                break;
        }
    }

    private void RemoveAllCenterMarks()
    {
        centerMarks.Clear();
        centerText.text = String.Empty;
    }

    private void RemoveAllCornerMarks()
    {
        CornerMarks.Clear();
        foreach (var text in cornerTextList)
        {
            text.text = string.Empty;
        }
    }
}