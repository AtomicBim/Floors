using Autodesk.Revit.DB;

public class WallHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Walls;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        var walls = GetElementsOfCategory(doc);

        foreach (Element wall in walls)
        {
            Parameter baseConstraint = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
            if (baseConstraint != null && baseConstraint.HasValue)
            {
                string floorNumber = ExtractFloorNumber(baseConstraint.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = wall.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}