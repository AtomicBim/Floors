using Autodesk.Revit.DB;
using System.Linq;

public class StairsHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Stairs;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        // Получаем экземпляры семейств лестниц
        var stairs = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category)
            .WhereElementIsNotElementType()
            .ToList();

        foreach (FamilyInstance stair in stairs)
        {
            // Используем параметр "Уровень"
            Parameter levelParam = stair.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                string levelValue = levelParam.AsValueString();
                string floorNumber = ExtractFloorNumber(levelValue);
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = stair.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}