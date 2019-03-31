using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Controllers
{
    public class ControllerHandler
    {
        public delegate void AddedControllerDelegate(IController controller);
        public AddedControllerDelegate addedController;

        public delegate void RemovedControllerDelegate(IController controller);
        public RemovedControllerDelegate removedController;

        public List<IController> controllers = new List<IController>();

        public void AddController(IController controller)
        {
            controllers.Add(controller);
            addedController?.Invoke(controller);
        }
        public void RemoveController(IController controller) {
            controllers.Remove(controller);
            removedController?.Invoke(controller);
        }

        public List<IController> GetControllers()
        {
            return controllers;
        }
    }
}
