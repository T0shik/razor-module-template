using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RazorModule.Areas.Test;

namespace JavaScriptClientGenerator
{
    internal class Program
    {
        private const string ClientOutputFlag = "client-output";
        private const string ModuleOutputFlag = "module-output";

        public static void Main(string[] args)
        {
            var builder = new JavaScriptClientBuilder();
            foreach (var arg in args)
            {
                var parts = arg.Trim().Split('=');
                var command = parts[0].ToLower();
                var value = parts[1];

                if (command == ClientOutputFlag)
                {
                    File.WriteAllText(value, builder.Client);
                }
                else if (command == ModuleOutputFlag)
                {
                    File.WriteAllText(value, builder.Module);
                }
            }
        }
    }
}