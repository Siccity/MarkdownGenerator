using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownWikiGenerator
{
    class Program
    {
        // 0 = dll src path, 1 = dest root
        static void Main(string[] args)
        {
            string target = null; // target .dll file
            string destination = "md"; // where to store generated md files
            bool namespaces = true; // include namespaces in filename
            List<string> whitelist = null; // Whitelist namespaces

            //Parse args
            foreach (string arg in args) {
                if (arg.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)) {
                    if (File.Exists(arg)) target = arg;
                    else Console.WriteLine("Target .dll not found at " + arg);
                } else if (Directory.Exists(arg)) destination = arg;
                else if (arg == "-nonamespace") namespaces = false;
                else if (arg.Contains('=')) {
                    string[] splitArgs = arg.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitArgs.Length == 2) {
                        if (splitArgs[0] == "-whitelist") {
                            if (whitelist == null) whitelist = new List<string>();
                            whitelist.Add(splitArgs[1]);
                        }
                    }
                }
            }

            if (target == null) {
                Console.WriteLine("No target .dll");
                Console.ReadLine();
            }

            MarkdownableType[] types = MarkdownGenerator.Load(target, whitelist);
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            //BuildHome(types, dest);
            BuildClasses(types, destination, namespaces);
            Console.ReadLine();
        }
    
        /// <summary> Build Home.md </summary>
        /*static void BuildHome (MarkdownableType[] types, string destination) {
            MarkdownBuilder homeBuilder = new MarkdownBuilder();
            homeBuilder.Header(1, "References");
            homeBuilder.AppendLine();

            foreach (var g in types.GroupBy(x => x.Namespace).OrderBy(x => x.Key)) {
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

                homeBuilder.HeaderWithLink(2, g.Key, g.Key);
                homeBuilder.AppendLine();

                var sb = new StringBuilder();
                foreach (var item in g.OrderBy(x => x.Name)) {
                    homeBuilder.ListLink(MarkdownBuilder.MarkdownCodeQuote(item.BeautifyName), g.Key + "#" + item.BeautifyName.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "-").ToLower());

                    sb.Append(item.ToString());
                }

                //File.WriteAllText(Path.Combine(destination, g.Key + ".md"), sb.ToString());
                homeBuilder.AppendLine();
            }

            // Write Home.md
            File.WriteAllText(Path.Combine(destination, "Home.md"), homeBuilder.ToString());
        }*/

        static void BuildClasses(MarkdownableType[] types, string destination, bool namespaces) {
            foreach(MarkdownableType type in types) {
                string filename = (namespaces ? type.Namespace + "." : "") + type.BeautifyName + ".md";
                File.WriteAllText(Path.Combine(destination, filename), type.ToString());
                Console.WriteLine(type.Namespace + " " + type.Name + " " + type.BeautifyName);
            }
        }
    }
}
