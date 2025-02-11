using Autodesk.Revit.DB;

public class MullionHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_CurtainWallMullions;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var mullions = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category);

        foreach (FamilyInstance mullion in mullions)
        {
            Wall hostWall = mullion.Host as Wall;
            if (hostWall != null)
            {
                Parameter baseConstraint = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                if (baseConstraint != null && baseConstraint.HasValue)
                {
                    string floorNumber = ExtractFloorNumber(baseConstraint.AsValueString());
                    if (!string.IsNullOrEmpty(floorNumber))
                    {
                        Parameter targetParam = mullion.LookupParameter("Т_Этаж");
                        if (targetParam != null && !targetParam.IsReadOnly)
                        {
                            targetParam.Set(floorNumber);
                        }
                    }
                }
            }
        }
    }
}