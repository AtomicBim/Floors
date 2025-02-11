using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Transaction(TransactionMode.Manual)]
public class SetFloors : IExternalCommand
{
    private readonly List<ICategoryHandler> handlers = new List<ICategoryHandler>();

    public SetFloors()
    {
        handlers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       typeof(ICategoryHandler).IsAssignableFrom(t) &&
                       t.GetCustomAttribute<FloorHandlerAttribute>() == null) // Используем атрибут вместо интерфейса
            .Select(t => (ICategoryHandler)Activator.CreateInstance(t))
            .ToList();
    }

    private IFloorHandler ShowFloorHandlerSelectionDialog()
    {
        var floorHandlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       t.GetCustomAttribute<FloorHandlerAttribute>() != null)
            .ToList();

        if (!floorHandlerTypes.Any())
        {
            TaskDialog.Show("Предупреждение", "Не найдено доступных обработчиков полов");
            return null;
        }

        // Создаем TaskDialog для выбора
        TaskDialog td = new TaskDialog("Выбор обработчика полов");
        td.MainInstruction = "Выберите обработчик полов для добавления:";
        td.CommonButtons = TaskDialogCommonButtons.Cancel;

        // Добавляем каждый handler как CommandLink
        foreach (var handlerType in floorHandlerTypes)
        {
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1 + floorHandlerTypes.IndexOf(handlerType),
                            handlerType.Name,
                            "Добавить этот обработчик полов");
        }

        TaskDialogResult result = td.Show();

        if (result == TaskDialogResult.Cancel)
        {
            return null;
        }

        int selectedIndex = (int)result - (int)TaskDialogResult.CommandLink1;
        if (selectedIndex >= 0 && selectedIndex < floorHandlerTypes.Count)
        {
            return (IFloorHandler)Activator.CreateInstance(floorHandlerTypes[selectedIndex]);
        }

        return null;
    }

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        Document doc = uiapp.ActiveUIDocument.Document;

        try
        {
            var selectedFloorHandler = ShowFloorHandlerSelectionDialog();

            if (selectedFloorHandler == null)
            {
                return Result.Cancelled;
            }

            handlers.Add(selectedFloorHandler);

            using (Transaction trans = new Transaction(doc, "Update Category Parameters"))
            {
                trans.Start();

                foreach (var handler in handlers)
                {
                    handler.ProcessElements(doc, trans);
                }

                trans.Commit();
            }

            TaskDialog.Show("Успех", "Параметры элементов успешно обновлены");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            TaskDialog.Show("Ошибка", ex.Message);
            return Result.Failed;
        }
    }
}