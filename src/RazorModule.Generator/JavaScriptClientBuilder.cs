using System.Linq;
using System.Reflection;
using RazorModule.Areas.Test;

namespace JavaScriptClientGenerator
{
    public class JavaScriptClientBuilder
    {
        public JavaScriptClientBuilder()
        {
            var methods = typeof(TestController)
                .Assembly
                .GetTypes()
                .Where(x => x == typeof(TestController))
                .SelectMany(x => x.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
                .Select(MethodTemplate)
                .Aggregate((a, b) => $"{a},\n{b}");

            Client = BuildAxiosClient(methods);
            Module = BuildModule(Client);
        }

        public string Client { get; }
        public string Module { get; }

        static string MethodTemplate(MethodInfo controllerAction)
        {
            var methodName = controllerAction.Name;
            var url = $"/{controllerAction.DeclaringType.Namespace.Split('.').Last()}/{controllerAction.DeclaringType.Name.Replace("Controller", "")}/{controllerAction.Name}".ToLower();
            return @$"{CamelCaseMethodName(methodName)}: () => axios.get(""{url}"")";
        }

        static string CamelCaseMethodName(string methodName)
        {
            return methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
        }

        static string BuildAxiosClient(string methods)
        {
            return $@"const someClient = (axios) => ({{
    {methods}
}})";
        }

        static string BuildModule(string client)
        {
            return $@"{client}
module.exports = someClient
";
        }
    }
}