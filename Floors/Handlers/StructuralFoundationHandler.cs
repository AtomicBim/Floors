using Autodesk.Revit.DB;

public class StructuralFoundationHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_StructuralFoundation;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        var foundations = GetElementsOfCategory(doc);

        foreach (Element foundation in foundations)
        {
            string floorNumber = string.Empty;

            // Сначала пробуем получить значение из параметра "Уровень"
            Parameter levelParam = foundation.get_Parameter(BuiltInParameter.LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                string levelValue = levelParam.AsValueString();
                floorNumber = ExtractFloorNumber(levelValue);
            }

            // Если не получилось, пробуем "Уровень спецификации"
            if (string.IsNullOrEmpty(floorNumber))
            {
                Parameter scheduleLevelParam = foundation.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM);
                if (scheduleLevelParam != null && scheduleLevelParam.HasValue)
                {
                    string scheduleLevelValue = scheduleLevelParam.AsValueString();
                    floorNumber = ExtractFloorNumber(scheduleLevelValue);
                }
            }

            // Если получили номер этажа из любого параметра, записываем его
            if (!string.IsNullOrEmpty(floorNumber))
            {
                Parameter targetParam = foundation.LookupParameter("Т_Этаж");
                if (targetParam != null && !targetParam.IsReadOnly)
                {
                    targetParam.Set(floorNumber);
                }
            }
        }
    }
}