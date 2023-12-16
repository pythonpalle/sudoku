using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    // serialize fields
    [Header("Background")]
    [SerializeField] private Image border;
    //[SerializeField] private Image whitePart;

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
    [SerializeField] private TileColorFiller _colorFiller;
    
    [Header("Selection")]
    [SerializeField] private SelectionObject selectionObject;
    
    [Header("Selection")]
    [SerializeField] private ColorObject selectColor;
    [SerializeField] private ColorObject pencilMarkColor;
    [SerializeField] private ColorObject contradictionColor;
    

    // public fields
    public int row { get; private set; }
    public int col { get; private set; }
    public int number { get; private set; } 

    public bool HasDigit => number > 0;

    public bool Permanent { get; private set; } = false;
    public bool Contradicted { get; private set; } = false;

    public List<int> CenterMarks { get; private set; } = new List<int>();
    public List<int> CornerMarks  { get; private set; } = new List<int>();
    public List<int> ColorMarks { get; private set; } = new List<int>();

    private bool HasColors => ColorMarks.Count > 0;


    // private fields
    public bool isSelected { get; private set; } = false;

    private static float whitePartSelectScale = 0.8f;
    private Vector3 whitePartSelectScaleVector = Vector3.one * whitePartSelectScale;
    private Vector3 whitePartStartScale;
    
    private RectTransform tileAnimationParent;

    int centerMarkFontSize = 30;
    
    private void Start()
    {
        whitePartStartScale = _colorFiller.transform.localScale;
        SetAllTextColors();
    }

    private void SetAllTextColors()
    {
        numberText.color = pencilMarkColor.Color;
        centerText.color = pencilMarkColor.Color;

        foreach (var cornerText in cornerTextList)
        {
            cornerText.color = pencilMarkColor.Color;
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
        Assert.IsFalse(Permanent && enterType != EnterType.ColorMark);
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
        Assert.IsFalse(Permanent);
        
        //if (Permanent) return false;

        if (remove) number = 0;
        SetDigit(number);
        return true;
    }

    private bool TryUpdateCorner(int addedNumber, bool remove)
    {
        Assert.IsFalse((Permanent || HasDigit) );
        
        if (remove && CornerMarks.Contains(addedNumber))
        {
            CornerMarks.Remove(addedNumber);
        }
        else
        {
            Assert.IsFalse(CornerMarks.Contains(addedNumber));
            CornerMarks.Add(addedNumber);
        }

        SortCornerMarks();
        return true;
    }
    
    private bool TryUpdateCenter(int addedNumber, bool remove)
    {
        Assert.IsFalse((Permanent || HasDigit) );
        
        if (remove && CenterMarks.Contains(addedNumber))
        {
            CenterMarks.Remove(addedNumber);
        }
        else
        {
            Assert.IsFalse(CenterMarks.Contains(addedNumber));
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
            Assert.IsFalse(ColorMarks.Contains(colorNumber));
            ColorMarks.Add(colorNumber);
        }

        SortColorMarks();
        return true;
    }


    private void SortCornerMarks()
    {
        int numbersInCorner = CornerMarks.Count;
        List<int> indexOrder = MarkClass.GetCornerIndexOrder(numbersInCorner);

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

    private void SortCenterMarks()
    {
        CenterMarks.Sort();
        
        MarkClass.UpdateCenterString(CenterMarks, centerMarkFontSize, centerText);
    }
    
    private void SortColorMarks()
    {
        // // find how many colors the tile has
        // int colorCount = ColorMarks.Count;
        // int colorHolderIndex = colorCount - 1;
        //
        // SetColorHolderObjectsActive(colorHolderIndex);
        //
        // if (colorCount <= 0)
        //     return;
        
        ColorMarks.Sort();
        _colorFiller.SetTileColors(ColorMarks, Contradicted);
        
        // ColorMarkHolder colorMarkHolder = colorMarkHolders[colorHolderIndex];
        // colorMarkHolder.SetColors(ColorMarks);
    }

    // private void SetColorHolderObjectsActive(int colorHolderIndex)
    // {
    //     for (int i = 0; i < colorMarkHolders.Count; i++)
    //     {
    //         bool setActive = i == colorHolderIndex;
    //         colorMarkHolders[i].gameObject.SetActive(setActive);
    //     }
    //     
    //     UpdateWhitePartColor();
    // }

    private Color SetAlpha(Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }

    public void Select()
    {
        isSelected = true;
        border.color = selectColor.Color;
        _colorFiller.transform.localScale = whitePartSelectScaleVector;
        //spriteMask.transform.localScale = whitePartSelectScaleVector;
        EventManager.SelectTile(this);
    }

    public void Deselect()
    {
        isSelected = false;
        border.color = Color.black;
        _colorFiller.transform.localScale = whitePartStartScale;
        //spriteMask.transform.localScale = whitePartStartScale;
        EventManager.DeselectTile(this);
    }

    public void SetContradiction()
    {
        if (Contradicted)
        {
            return;
        }
        
        Contradicted = true;
        UpdateWhitePartColor();
    }

    public void RemoveContradiction()
    {
        if (!Contradicted)
        {
            return;
        }
        
        Contradicted = false;
        UpdateWhitePartColor();
    }

    void UpdateWhitePartColor()
    {
        _colorFiller.SetTileColors(ColorMarks, Contradicted);
        return; 
        
        // if (Contradicted)
        // {
        //     whitePart.color = contradictionColor.Color;
        //
        //     if (HasColors)
        //     {
        //         whitePart.color = SetAlpha(whitePart.color, 0.75f);
        //     }
        // }
        // else
        // {
        //     whitePart.color = Color.white;
        //     
        //     if (HasColors)
        //     {
        //         whitePart.color = SetAlpha(whitePart.color, 0f);
        //     }
        //     else
        //     {
        //         whitePart.color = SetAlpha(whitePart.color, 1f);
        //     }
        // }
    }

    public bool HasSameNumber(int otherNumber, EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                return this.number == otherNumber;
            
            case EnterType.CenterMark:
                return !HasDigit && CenterMarks.Contains(otherNumber);
            
            case EnterType.CornerMark:
                return !HasDigit && CornerMarks.Contains(otherNumber);
            
            case EnterType.ColorMark:
                return ColorMarks.Contains(otherNumber);
        }

        return false;
    }

    public bool TryRemoveAllOfType(EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                return TryUpdateNumber(0, EnterType.DigitMark, true);
            
            case EnterType.CenterMark:
                return TryRemoveAllCenterMarks();
            
            case EnterType.CornerMark:
                return TryRemoveAllCornerMarks();
            
            case EnterType.ColorMark:
                return TryRemoveAllColorMarks();
            
            default:
                return false;
        }
    }

    private bool TryRemoveAllCenterMarks()
    {
        centerText.text = String.Empty;

        if (CenterMarks.Count > 0)
        {
            CenterMarks.Clear();
            return true;
        }

        return false;
    }

    private bool TryRemoveAllCornerMarks()
    {
        foreach (var text in cornerTextList)
        {
            text.text = string.Empty;
        }
        
        if (CornerMarks.Count > 0)
        {
            CornerMarks.Clear();
            return true;
        }

        return false;
    }
    
    private bool TryRemoveAllColorMarks()
    {
        bool hadColors = ColorMarks.Count > 0;
        
        ColorMarks.Clear();
        
        // // passing in -1 to inactivate all color holder
        // SetColorHolderObjectsActive(-1);
        
        _colorFiller.SetTileColors(ColorMarks, Contradicted);

        return hadColors;
    }

    public void Hint()
    {
        if (!isHinting)
            StartCoroutine(PlayHintAnimation());
    }

    private bool isHinting;
    private IEnumerator PlayHintAnimation()
    {
        isHinting = true;

        // switching parent to make tile render on top of every other tile
        var defaultParent = transform.parent;
        transform.SetParent(tileAnimationParent, true);
        
        var startScale = transform.localScale;
        float upscaledFactor = 1.3f;
        var upscaleScale = startScale * upscaledFactor;
        
        float timeForUpscale = 0.3f;
        
        int scaleShiftCount = 0;
        int numberOfScaleShifts = 6;
        
        bool scaleUp = true;
        float t = 0;

        while (scaleShiftCount < numberOfScaleShifts)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime / timeForUpscale;

            transform.localScale = scaleUp ? 
                Vector3.Lerp(startScale, upscaleScale, t) 
                : Vector3.Lerp(upscaleScale, startScale, t);
            
            if (t > 1)
            {
                scaleUp = !scaleUp;
                t = 0;
                scaleShiftCount++;
            }
        }

        transform.SetParent(defaultParent, true);
        isHinting = false;
    }

    public void SetAnimationParent(RectTransform animationParent)
    {
        tileAnimationParent = animationParent;
    }


    public bool IsEffectedByEntry(int number, EnterType enterType, RemovalType remove)
    {
        // maybe can be skipped
        if (Permanent && enterType != EnterType.ColorMark) return false;

        switch (enterType)
        {
            case EnterType.DigitMark:
                return IsEffectedByDigit(number, remove);
            
            case EnterType.CornerMark:
                return IsEffectedByCorner(number, remove);
            
            case EnterType.CenterMark:
                return IsEffectedByCenter(number, remove);
            
            case EnterType.ColorMark:
                return IsEffectedByColor(number, remove);
        }

        return false;
    }
    
    private bool IsEffectedByColor(int nbr, RemovalType remove)
    {
        return IsEffectedByListType(nbr, remove, ColorMarks);
    }

    private bool IsEffectedByCorner(int nbr, RemovalType remove)
    {
        if (HasDigit)
            return false;

        return IsEffectedByListType(nbr, remove, CornerMarks);
    }
    
    private bool IsEffectedByCenter(int nbr, RemovalType remove)
    {
        if (HasDigit)
            return false;

        return IsEffectedByListType(nbr, remove, CenterMarks);
    }

    private bool IsEffectedByListType(int nbr, RemovalType remove, List<int> list)
    {
        switch (remove)
        {
            case RemovalType.All:
                return list.Count > 0;
            case RemovalType.Single:
                return list.Contains(nbr);;
        }

        return !list.Contains(nbr);
    }

    private bool IsEffectedByDigit(int nbr, RemovalType remove)
    {
        if (Permanent)
            return false;

        switch (remove)
        {
            case RemovalType.All:
            case RemovalType.Single:
                return HasDigit;
        }

        return nbr != number;
    }

    private int indexInt = -1;
    public int IndexInt
    {
        get
        {
            if (indexInt < 0)
            {
                indexInt = row * 9 + col;
            }

            return indexInt;
        }
    }
}