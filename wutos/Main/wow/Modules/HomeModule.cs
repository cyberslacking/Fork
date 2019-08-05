using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.Configuration;

namespace APP.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            string HomePage = "/Index";
            if (ConfigurationManager.AppSettings["HomePage"] != null)
                HomePage = ConfigurationManager.AppSettings["HomePage"];
            Get["/"] = p =>
            {
                return View[HomePage];
            };

            Get["/Home"] = p =>
            {
                return View[HomePage];
            };

            Get["/Index"] = p =>
            {
                return View[HomePage];
            };
        }
    }
}
