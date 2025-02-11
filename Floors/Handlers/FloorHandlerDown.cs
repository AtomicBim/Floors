using Autodesk.Revit.DB;
using System.Linq;

[FloorHandler]
public class FloorHandlerDown : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Floors;

    public override void ProcessElements(Document doc, Transaction trans)
    {
        var floors = GetElementsOfCategory(doc);
        foreach (Element floor in floors)
        {
            Parameter levelParam = floor.get_Parameter(BuiltInParameter.LEVEL_PARAM);
            if (levelParam != null && levelParam.HasValue)
            {
                Level currentLevel = doc.GetElement(levelParam.AsElementId()) as Level;
                if (currentLevel != null)
                {
                    // Новая логика: уровень, к которому привязано перекрытие
                    string floorNumber = ExtractFloorNumber(currentLevel.Name);

                    if (!string.IsNullOrEmpty(floorNumber))
                    {
                        Parameter targetParam = floor.LookupParameter("Т_Этаж");
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