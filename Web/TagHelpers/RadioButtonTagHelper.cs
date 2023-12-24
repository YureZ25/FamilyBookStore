using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "[type='radio']", TagStructure = TagStructure.WithoutEndTag)]
    public class RadioButtonTagHelper(IHtmlGenerator generator) : InputTagHelper(generator)
    {
        [HtmlAttributeName("asp-checked")]
        public bool Checked { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (Checked)
            {
                output.Attributes.Add(new TagHelperAttribute("checked"));
            }
        }
    }
}
