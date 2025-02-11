using Autodesk.Revit.DB;

public class WindowHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Windows;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var windows = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category);

        foreach (FamilyInstance window in windows)
        {
            Parameter levelParam = window.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                string floorNumber = ExtractFloorNumber(levelParam.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = window.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}