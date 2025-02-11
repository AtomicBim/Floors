using Autodesk.Revit.DB;
using System.Linq;

[FloorHandler]
public class FloorHandlerUp : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_Floors;

    private Level GetNextLowerLevel(Document doc, Level currentLevel)
    {
        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderByDescending(l => l.Elevation)  // Сортируем по убыванию
            .ToList();

        return levels.FirstOrDefault(l =>
            l.Elevation < currentLevel.Elevation);
    }

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
                    // Находим ближайший нижний уровень
                    Level lowerLevel = GetNextLowerLevel(doc, currentLevel);

                    string floorNumber;
                    if (lowerLevel != null)
                    {
                        // Если есть уровень ниже, берем его номер
                        floorNumber = ExtractFloorNumber(lowerLevel.Name);
                    }
                    else
                    {
                        // Если уровня ниже нет, используем номер текущего уровня
                        floorNumber = ExtractFloorNumber(currentLevel.Name);
                    }

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