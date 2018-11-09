using System;
using System.IO;
using NLog;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace IotDataServer.HttpServer
{
    public static class TemplateManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static IRazorEngineService Service { get; set; }
        static TemplateServiceConfiguration Configuration { get; set; }
        static readonly object _lockObject = new object();

        static TemplateManager()
        {
            Cleanup();
            Configuration = new TemplateServiceConfiguration();
            Service = RazorEngineService.Create(Configuration);
            Engine.Razor = Service;
    }

        /// <summary>
        /// Resets the cache.
        /// </summary>
        public static void ResetCache()
        {
            //이것도 동작 안함
            Configuration.CachingProvider = new DefaultCachingProvider();
            Cleanup();
        }
        public static void Cleanup()
        {
            string tempPath = Path.GetTempPath();
            DirectoryInfo di = new DirectoryInfo(Path.GetTempPath());

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                if (dir.Name.StartsWith("RazorEngine_"))
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Compiles, caches and parses a template using RazorEngine.
        /// </summary>
        /// <param name="templateKey">Type of the template.</param>
        /// <param name="anonymousType">Type of the anonymous object.</param>
        /// <returns></returns>
        public static string Rander<T>(string templateKey, T anonymousType)
        {
            try
            {
                lock (_lockObject)
                {
                    if (!Service.IsTemplateCached(templateKey, anonymousType.GetType()))
                    {
                        string file = $"Templates/{templateKey}.cshtml";
                        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                        Engine.Razor.Compile(new LoadedTemplateSource(System.IO.File.ReadAllText(fullPath), fullPath), templateKey, anonymousType.GetType());
                    }
                }
                return Engine.Razor.Run(templateKey, anonymousType.GetType(), anonymousType);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot load template!");
            }
            return "";
        }

        internal static string Rander(string templateKey)
        {
            try
            {
                lock (_lockObject)
                {
                    if (!Service.IsTemplateCached(templateKey, null))
                    {
                        string file = $"Templates/{templateKey}.cshtml";
                        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                        Engine.Razor.Compile(new LoadedTemplateSource(System.IO.File.ReadAllText(fullPath), fullPath), templateKey, null);
                    }
                }
                return Engine.Razor.Run(templateKey);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Cannot load template!");
            }
            return "";
        }
    }
}
