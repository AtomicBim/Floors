using Autodesk.Revit.DB;

public class StructuralColumnHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_StructuralColumns;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        var columns = GetElementsOfCategory(doc);

        foreach (Element column in columns)
        {
            // Используем параметр "Зависимость снизу"
            Parameter baseConstraint = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
            if (baseConstraint != null && baseConstraint.HasValue)
            {
                string baseConstraintValue = baseConstraint.AsValueString();
                string floorNumber = ExtractFloorNumber(baseConstraintValue);
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = column.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}