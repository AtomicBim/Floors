using Autodesk.Revit.DB;

public class DoorHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Doors;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var doors = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category);

        foreach (FamilyInstance door in doors)
        {
            Parameter levelParam = door.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                string floorNumber = ExtractFloorNumber(levelParam.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = door.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}