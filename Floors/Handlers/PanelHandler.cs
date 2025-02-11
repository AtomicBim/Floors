using Autodesk.Revit.DB;

public class PanelHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_CurtainWallPanels;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var panels = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category);

        foreach (FamilyInstance panel in panels)
        {
            Wall hostWall = panel.Host as Wall;
            if (hostWall != null)
            {
                Parameter baseConstraint = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                if (baseConstraint != null && baseConstraint.HasValue)
                {
                    string floorNumber = ExtractFloorNumber(baseConstraint.AsValueString());
                    if (!string.IsNullOrEmpty(floorNumber))
                    {
                        Parameter targetParam = panel.LookupParameter("Т_Этаж");
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