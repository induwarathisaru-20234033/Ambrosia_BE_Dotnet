using HandlebarsDotNet;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Common
{
    public static class NotificationTemplateEngine
    {
        public static string Render(string templateContent, object data)
        {
            var template = Handlebars.Compile(templateContent);
            return template(data);
        }
    }
}
