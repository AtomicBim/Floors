using Autodesk.Revit.DB;
using System;
using System.Text.RegularExpressions;

public abstract class BaseCategoryHandler : ICategoryHandler, IFloorHandler
{
    public abstract BuiltInCategory Category { get; }

    protected FilteredElementCollector GetElementsOfCategory(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfCategory(Category)
            .WhereElementIsNotElementType();
    }

    public abstract void ProcessElements(Document doc, Transaction trans);

    protected string ExtractFloorNumber(string baseConstraintValue)
    {
        if (string.IsNullOrWhiteSpace(baseConstraintValue))
            return string.Empty;

        Match floorMatch = Regex.Match(
            baseConstraintValue,
            @"Этаж\s*-?\d+|Этаж\s*-?\d{2,3}",
            RegexOptions.IgnoreCase
        );
        if (floorMatch.Success)
        {
            // Extract the numeric part and normalize it
            var numericPart = Regex.Match(floorMatch.Value, @"-?\d+").Value;
            var floorNum = int.Parse(numericPart);
            return $"Этаж {(floorNum < 0 ? "-" : "")}{Math.Abs(floorNum):D2}";
        }

        Match roofMatch = Regex.Match(
            baseConstraintValue,
            @"(Выход\s+на\s+)?Кровл[яюи].*",
            RegexOptions.IgnoreCase
        );
        if (roofMatch.Success)
        {
            return roofMatch.Value.TrimEnd();
        }

        return string.Empty;
    }
}