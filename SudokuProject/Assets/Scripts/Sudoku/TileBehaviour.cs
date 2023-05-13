using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
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
    
    [Header("Color")]
    [SerializeField] private List<ColorMarkHolder> colorMarkHolders;
    [SerializeField] private RectTransform spriteMask;
    
    [Header("Scriptable objects")]
    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private ColorObject selectColor;

    
    // public fields
    public int row { get; private set; }
    public int col { get; private set; }
    public int number { get; private set; } 

    public bool HasDigit => number > 0;

    public bool Permanent { get; private set; } = false;
    public bool Contradicted { get; private set; } = false;

    public List<int> CenterMarks { get; private set; } = new List<int>();
    public List<int> CornerMarks  { get; private set; } = new List<int>();
    public List<int> ColorMarks; // { get; private set; }


    // private fields
    public bool isSelected { get; private set; } = false;

    private static float whitePartSelectScale = 0.8f;
    private Vector3 whitePartSelectScaleVector = Vector3.one * whitePartSelectScale;
    private Vector3 whitePartStartScale;
    
    int centerMarkFontSize = 30;

    private void Start()
    {
        whitePartStartScale = whitePart.transform.localScale;
        SetAllTextColors();
    }

    private void SetAllTextColors()
    {
        numberText.color = selectColor.Color;
        centerText.color = selectColor.Color;

        foreach (var cornerText in cornerTextList)
        {
            cornerText.color = selectColor.Color;
        }

    }

    private void OnEnable()
    {
        selectionObject.OnDeselectAllTiles += OnDeselectAllTiles;
    }

    private void OnDisable()
    {
        selectionObject.OnDeselectAllTiles -= OnDeselectAllTiles;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        EventManager.TilePointerDown(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager.TilePointerUp(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.TilePointerEnter(this);
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
            numberText.color = Color.black;
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
        if (Permanent && enterType != EnterType.ColorMark) return false;

        switch (enterType)
        {
            case EnterType.DigitMark:
                return TryUpdateDigit(number, remove);
            
            case EnterType.CornerMark:
                return TryUpdateCorner(number, remove);
            
            case EnterType.CenterMark:
                return TryUpdateCenter(number, remove);
            
            case EnterType.ColorMark:
                return TryUpdateColor(number, remove);
        }

        return false;
    }

    private bool TryUpdateDigit(int number, bool remove)
    {
        if (Permanent) return false;

        if (remove) number = 0;
        SetDigit(number);
        // numberText.color = Color.blue;
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
        
        if (remove && CenterMarks.Contains(addedNumber))
        {
            CenterMarks.Remove(addedNumber);
        }
        else
        {
            if (CenterMarks.Contains(addedNumber))
                return false;
            
            CenterMarks.Add(addedNumber);
        }

        SortCenterMarks();
        return true;
    }
    
    private bool TryUpdateColor(int colorNumber, bool remove)
    {
        if (colorNumber <= 0)
            remove = true;
        
        if (remove && ColorMarks.Contains(colorNumber))
        {
            ColorMarks.Remove(colorNumber);
        }
        else
        {
            if (ColorMarks.Contains(colorNumber))
                return false;
            
            ColorMarks.Add(colorNumber);
        }

        SortColorMarks();
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

        // indexes have different positions depending on how many numbers are in corners
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
        CenterMarks.Sort();

        int centerMarkCount = CenterMarks.Count;
        float centerStringSize = centerMarkFontSize;
        int maximumMarksForDefaultSize = 5;

        // decreasing font size to make all numbers fit in size
        if (centerMarkCount > maximumMarksForDefaultSize)
        {
            int difference = centerMarkCount - maximumMarksForDefaultSize;
            float powBase = 0.87f;

            centerStringSize *= Mathf.Pow(powBase, difference);
        }

        string centerMarkString = string.Empty;
        foreach (var mark in CenterMarks)
        {
            centerMarkString += mark.ToString();
        }

        centerText.text = centerMarkString;
        centerText.fontSize = centerStringSize;
    }
    
    private void SortColorMarks()
    {
        // find how many colors the tile has
        int colorCount = ColorMarks.Count;
        int colorHolderIndex = colorCount - 1;

        SetColorHolderObjectsActive(colorHolderIndex);

        if (colorCount <= 0)
            return;
        
        ColorMarks.Sort();
        ColorMarkHolder colorMarkHolder = colorMarkHolders[colorHolderIndex];
        colorMarkHolder.SetColors(ColorMarks);
    }

    private void SetColorHolderObjectsActive(int colorHolderIndex)
    {
        for (int i = 0; i < colorMarkHolders.Count; i++)
        {
            bool setActive = i == colorHolderIndex;
            colorMarkHolders[i].gameObject.SetActive(setActive);
        }

        bool activateWhitePart = ColorMarks.Count <= 0;
        whitePart.gameObject.SetActive(activateWhitePart);
    }

    public void Select()
    {
        isSelected = true;
        border.color = selectColor.Color;
        whitePart.transform.localScale = whitePartSelectScaleVector;
        spriteMask.transform.localScale = whitePartSelectScaleVector;
        EventManager.SelectTile(this);
    }

    public void Deselect()
    {
        isSelected = false;
        border.color = Color.black;
        whitePart.transform.localScale = whitePartStartScale;
        spriteMask.transform.localScale = whitePartStartScale;
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
                return !HasDigit && CenterMarks.Contains(number);
            
            case EnterType.CornerMark:
                return !HasDigit && CornerMarks.Contains(number);
            
            case EnterType.ColorMark:
                return ColorMarks.Contains(number);
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
            
            case EnterType.ColorMark:
                RemoveAllColorMarks();
                break;
        }
    }

    private void RemoveAllCenterMarks()
    {
        CenterMarks.Clear();
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
    
    private void RemoveAllColorMarks()
    {
        ColorMarks.Clear();
        
        // passing in -1 to inactivate all color holder
        SetColorHolderObjectsActive(-1);
    }
}