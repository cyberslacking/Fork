using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using System.Configuration;

namespace APP.Nancy
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            //程序根目录 需要绝对路径
            string _rootPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\" + ConfigurationManager.AppSettings["WebRootRelative"];
            if (ConfigurationManager.AppSettings["WebRootAbsolute"] != null)
                _rootPath = ConfigurationManager.AppSettings["WebRootAbsolute"];
            return _rootPath;
        }
    }
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }
        /// <summary>
        /// 配置静态文件访问权限
        /// </summary>
        /// <param name="conventions"></param>
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            ///静态文件夹访问 设置 css,js,image
            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.Clear();
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("content", "content"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(ConfigurationManager.AppSettings["WebStaticResource"], ConfigurationManager.AppSettings["WebStaticResource"]));
        }
    }
}
