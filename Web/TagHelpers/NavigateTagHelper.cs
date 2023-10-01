using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.TagHelpers
{
    [HtmlTargetElement(Attributes = "navigate")]
    public class NavigateTagHelper : AnchorTagHelper
    {
        public NavigateTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = Generator.GenerateActionLink(
                   ViewContext,
                   linkText: string.Empty,
                   actionName: Action,
                   controllerName: Controller,
                   protocol: Protocol,
                   hostname: Host,
                   fragment: Fragment,
                   routeValues: new RouteValueDictionary(RouteValues),
                   htmlAttributes: null);

            var href = tagBuilder.Attributes.GetValueOrDefault("href");

            output.Attributes.Add("onclick", $"window.location.href = '{href}';");
        }
    }
}
