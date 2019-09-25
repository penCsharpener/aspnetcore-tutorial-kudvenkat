using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Utils {
    public static class ControllerNameOutput {
        public static string ToString(string controllerName) {
            return controllerName.Replace("Controller", string.Empty);
        }
    }

}
