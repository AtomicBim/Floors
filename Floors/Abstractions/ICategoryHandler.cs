using Autodesk.Revit.DB;

public interface ICategoryHandler
{
    void ProcessElements(Document doc, Transaction trans);
    BuiltInCategory Category { get; }
}