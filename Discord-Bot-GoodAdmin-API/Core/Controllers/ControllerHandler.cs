using Discord;
using System.Collections.Generic;
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
        public async Task AddController(IController controller, IGuild guild, params object[] args)
        {
            AddController(controller);

            object[] kwargs = new object[args.Length + 1];
            kwargs[0] = controller.GetType();
            for (var i = 0; i < args.Length; i++)
                kwargs[i + 1] = args[i];

            var cfg = await Configuration.LoadOrCreateGuildConfig(guild);
            cfg.controllers.Add(kwargs);
            await cfg.Save(guild);
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
