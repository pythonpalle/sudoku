
using System;
using System.Collections.Generic;

public static class MethodContainer
{
    private static readonly List<DigitMethod> DigitMethods = new ()
    {
        // Naked Single First because its fastest
        new NakedSingle(),
        
        // Hidden box single second because it's much easier for humans to find than in rows/cols,
        // good for giving hints
        new HiddenSingleBox(),

        new HiddenSingleColumn(),
        new HiddenSingleInRow(),
    };

    private static readonly List<CandidateMethod> CandidateMethods = new ()
    {
        // Pointing Pairs
        new PointingPairRowToBox(),
        new PointingPairColToBox(),
        
        new PointingPairBoxToRow(),
        new PointingPairBoxToCol(),
        
        // Pointing Triples
        new PointingTripleRowToBox(),
        new PointingTripleColToBox(),
        new PointingTripleBoxToRow(),
        new PointingTripleBoxToCol(),
        
        // Naked Pairs
        new NakedPairInCol(),
        new NakedPairInRow(),
        new NakedPairInBox(),
        
        // Hidden pairs
        new HiddenPairInBox(),
        new HiddenPairInRow(),
        new HiddenPairInCol(),
        
        // Naked Triples
        new NakedTripleInRow(),
        new NakedTripleInCol(),
        new NakedTripleInBox(),
        
        // Hidden triples 
        new HiddenTripleInBox(),
        new HiddenTripleInRow(),
        new HiddenTripleInCol(),
        
        // Naked Quad (includes row, col and box)
        new NakedQuad(),
        
        // Hidden Quad (includes row, col and box)
        new HiddenQuad(),
        
        // Uniqueness
        new UniquenessRectangle(),
        
        // XWings
        new XWingRow(),
        new XWingCol(),

        // Extended Wings
        new XYWing(),
        new XYZWing(),
        
        // Swordfish
        new SwordFishRow(),
        new SwordFishCol(),
        
        // JellyFish
        new JellyFishRow(),
        new JellyFishCol(),
    };

    private static List<DigitMethod> easyDigitMethods = new List<DigitMethod>();
    private static List<DigitMethod> mediumDigitMethods = new List<DigitMethod>();
    private static List<DigitMethod> hardDigitMethods = new List<DigitMethod>();
    private static List<DigitMethod> extremeDigitMethods = new List<DigitMethod>();
    
    private static List<CandidateMethod> easyCandidateMethods = new List<CandidateMethod>();
    private static List<CandidateMethod> mediumCandidateMethods = new List<CandidateMethod>();
    private static List<CandidateMethod> hardCandidateMethods = new List<CandidateMethod>();
    private static List<CandidateMethod> extremeCandidateMethods = new List<CandidateMethod>();

    public static List<DigitMethod> GetDigitMethodsOfDifficulty(PuzzleDifficulty difficulty)
    {
        SetUpAllDigitMethodDifficulties();
        switch (difficulty)
        {
            case PuzzleDifficulty.Simple:
            case PuzzleDifficulty.Easy:
                return easyDigitMethods;
            
            case PuzzleDifficulty.Medium:
                return mediumDigitMethods;
            
            case PuzzleDifficulty.Hard:
                return hardDigitMethods;
            
            case PuzzleDifficulty.Extreme:
                return extremeDigitMethods;
        }
        
        return null;
    }

    private static void SetUpAllDigitMethodDifficulties()
    {
        TrySetUpDigitMethod(easyDigitMethods, PuzzleDifficulty.Easy);
        TrySetUpDigitMethod(mediumDigitMethods, PuzzleDifficulty.Medium);
        TrySetUpDigitMethod(hardDigitMethods, PuzzleDifficulty.Hard);
        TrySetUpDigitMethod(extremeDigitMethods, PuzzleDifficulty.Extreme);
    }

    private static void TrySetUpDigitMethod(List<DigitMethod> digitMethods, PuzzleDifficulty difficulty)
    {
        if (digitMethods.Count <= 0)
            SetupDigitMethods(digitMethods, difficulty);
    }

    private static void SetupDigitMethods(List<DigitMethod> digitMethods, PuzzleDifficulty difficulty)
    {
        foreach (var method in DigitMethods)
        {
            if ((int)method.Difficulty <= (int)difficulty)
            {
                digitMethods.Add(method);
            }
        }
    }
    
    public static List<CandidateMethod> GetCandidatesMethodsOfDifficulty(PuzzleDifficulty difficulty)
    {
        SetUpAllCandidateMethodDifficulties();
        switch (difficulty)
        {
            case PuzzleDifficulty.Simple:
            case PuzzleDifficulty.Easy:
                return easyCandidateMethods;
            
            case PuzzleDifficulty.Medium:
                return mediumCandidateMethods;
            
            case PuzzleDifficulty.Hard:
                return hardCandidateMethods;
            
            case PuzzleDifficulty.Extreme:
                return extremeCandidateMethods;
        }
        
        return null;
    }
    
    private static void SetUpAllCandidateMethodDifficulties()
    {
        TrySetUpCandidateMethod(easyCandidateMethods, PuzzleDifficulty.Easy);
        TrySetUpCandidateMethod(mediumCandidateMethods, PuzzleDifficulty.Medium);
        TrySetUpCandidateMethod(hardCandidateMethods, PuzzleDifficulty.Hard);
        TrySetUpCandidateMethod(extremeCandidateMethods, PuzzleDifficulty.Extreme);
    }

    private static void TrySetUpCandidateMethod(List<CandidateMethod> candidateMethods, PuzzleDifficulty difficulty)
    {
        foreach (var method in CandidateMethods)
        {
            if ((int)method.Difficulty <= (int)difficulty)
            {
                candidateMethods.Add(method);
            }
        }
    }
}