using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;

public class StairsRailing : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_StairsRailing;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var railings = new FilteredElementCollector(doc)
            .OfClass(typeof(Railing))
            .OfCategory(Category);

        foreach (Railing railing in railings)
        {
            Parameter railingBaseLevelParam = railing.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM);
            if (railingBaseLevelParam != null && railingBaseLevelParam.HasValue)
            {
                string floorNumber = ExtractFloorNumber(railingBaseLevelParam.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = railing.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}