using Autodesk.Revit.DB;

public class ParkingHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Parking;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        // Получаем экземпляры парковочных мест
        var parkingSpaces = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(Category);

        foreach (FamilyInstance parking in parkingSpaces)
        {
            // Используем параметр ОСНОВА (INSTANCE_FREE_HOST_PARAM)
            Parameter baseParam = parking.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_PARAM);

            if (baseParam != null && baseParam.HasValue)
            {
                string floorNumber = ExtractFloorNumber(baseParam.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = parking.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}