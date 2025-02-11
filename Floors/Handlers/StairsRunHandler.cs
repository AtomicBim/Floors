using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

public class StairRunHandler : BaseCategoryHandler
{
    public override BuiltInCategory Category => BuiltInCategory.OST_StairsRuns;

    private Level GetClosestLevel(Document doc, double elevation)
    {
        // Получаем все уровни, сортируем по высоте
        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(l => l.Elevation)
            .ToList();

        // Если уровней нет, возвращаем null
        if (!levels.Any()) return null;

        // Если elevation ниже первого уровня, возвращаем первый
        if (elevation <= levels.First().Elevation)
            return levels.First();

        // Если elevation выше последнего уровня, возвращаем последний
        if (elevation >= levels.Last().Elevation)
            return levels.Last();

        // Ищем два ближайших уровня
        Level lowerLevel = levels.First();
        foreach (var level in levels)
        {
            if (level.Elevation > elevation)
                break;
            lowerLevel = level;
        }

        // Возвращаем ближайший уровень
        return lowerLevel;
    }

    public override void ProcessElements(Document doc, Transaction trans)
    {
        var runs = GetElementsOfCategory(doc);

        foreach (Element run in runs)
        {
            try
            {
                // Получаем BoundingBox элемента
                BoundingBoxXYZ bbox = run.get_BoundingBox(null);
                if (bbox != null)
                {
                    // Получаем нижнюю точку
                    double bottomElevation = bbox.Min.Z;

                    // Находим ближайший уровень
                    Level closestLevel = GetClosestLevel(doc, bottomElevation);
                    if (closestLevel != null)
                    {
                        // Получаем имя уровня и обрабатываем его нашей функцией извлечения этажа
                        string levelName = closestLevel.Name;
                        string floorNumber = ExtractFloorNumber(levelName);

                        if (!string.IsNullOrEmpty(floorNumber))
                        {
                            Parameter targetParam = run.LookupParameter("Т_Этаж");
                            if (targetParam != null && !targetParam.IsReadOnly)
                            {
                                targetParam.Set(floorNumber);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но продолжаем обработку других элементов
                TaskDialog.Show("Ошибка", $"Ошибка при обработке марша: {ex.Message}");
            }
        }
    }
}