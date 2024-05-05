using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;

namespace Web.TagHelpers
{
    [HtmlTargetElement("script", Attributes = targetAttribute)]
    public class LoadScriptTagHelper : TagHelper
    {
        private const string targetAttribute = "on-content-loaded";

        private readonly IHostEnvironment _hostEnvironment;

        public LoadScriptTagHelper(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Execute script only once document is loaded.
        /// </summary>
        [HtmlAttributeName(targetAttribute)]
        public bool OnContentLoaded { get; set; } = false;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!OnContentLoaded)
            {
                output.SuppressOutput();
                return;
            }

            var dir = Path.GetDirectoryName(ViewContext.ExecutingFilePath);
            var viewName = Path.GetFileName(ViewContext.ExecutingFilePath);

            var dirContents = _hostEnvironment.ContentRootFileProvider.GetDirectoryContents(dir);
            var scriptFile = dirContents.FirstOrDefault(f => !f.IsDirectory && f.Exists && f.Name == $"{viewName}.js");
            if (scriptFile == null)
            {
                output.SuppressOutput();
                return;
            }

            using var scriptStream = scriptFile.CreateReadStream();
            var sr = new StreamReader(scriptStream);
            var script = await sr.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(script)) 
            {
                output.SuppressOutput();
                return;
            }

            var content = $$"""
                document.addEventListener('DOMContentLoaded', function() {
                    {{script}}
                });
                """;
            output.Content.SetHtmlContent(content);

            // TODO: Try implement is as compile time event
            //var assemblyName = this.GetType().Assembly.GetName().Name;
            //var scriptsFile = _hostEnvironment.ContentRootFileProvider.GetFileInfo($"{assemblyName}.scripts.js");
            //using var stream = File.AppendText(scriptFile.PhysicalPath);
            //await stream.WriteLineAsync(script);
        }
    }
}
