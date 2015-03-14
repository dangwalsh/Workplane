using System;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Workplane
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;

                if (IsWorkplaneParallel(uidoc))
                	throw new  WorkplaneException("Workplane is not oriented correctly");

                return Result.Succeeded;
            }
            catch (WorkplaneException ex)
            {
                TaskDialog.Show("Workplane Error", ex.Message);
                return Result.Failed;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }
        bool IsWorkplaneParallel(UIDocument uidoc)
        {
            SketchPlane sketchPlane = uidoc.ActiveView.SketchPlane;

            if (sketchPlane == null)
                throw new NullReferenceException();

            XYZ sketchNormal = sketchPlane.GetPlane().Normal;
            XYZ viewNormal = uidoc.ActiveView.ViewDirection;
            XYZ vec = sketchNormal.Subtract(viewNormal);

            return vec.IsZeroLength();
        }
    }

    public class WorkplaneException : Exception
    {
        public WorkplaneException(string message)
            : base(message)
        {
        }
    }
}