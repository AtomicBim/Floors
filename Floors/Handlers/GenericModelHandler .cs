using Autodesk.Revit.DB;

public class GenericModelHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_GenericModel;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        // Получаем экземпляры семейств обобщенных моделей
        var genericModels = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category)
            .WhereElementIsNotElementType();

        foreach (FamilyInstance model in genericModels)
        {
            // Используем параметр "Уровень"
            Parameter levelParam = model.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                string levelValue = levelParam.AsValueString();
                string floorNumber = ExtractFloorNumber(levelValue);
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = model.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}