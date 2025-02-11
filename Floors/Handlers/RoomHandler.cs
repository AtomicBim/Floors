using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;

public class RoomHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Rooms;
    public override void ProcessElements(Document doc, Transaction trans)
    {
        var rooms = new FilteredElementCollector(doc)
            .OfCategory(Category)
            .WhereElementIsNotElementType();

        foreach (Room room in rooms)
        {
            Parameter levelParam = room.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID);
            if (levelParam != null && levelParam.HasValue)
            {
                string floorNumber = ExtractFloorNumber(levelParam.AsValueString());
                if (!string.IsNullOrEmpty(floorNumber))
                {
                    Parameter targetParam = room.LookupParameter("Т_Этаж");
                    if (targetParam != null && !targetParam.IsReadOnly)
                    {
                        targetParam.Set(floorNumber);
                    }
                }
            }
        }
    }
}