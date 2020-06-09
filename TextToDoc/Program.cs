using Appserver.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TextToDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            if( args.Length < 2)
            {
                Console.WriteLine("Usage: text2doc [image|json|blur] <filename>");
                return;
            }

            switch ( args[0])
            {
                case "image":
                case "json":
                    TextractProcessing.Run(args);
                    break;
                case "blur":
                    Images.Run(args);
                    break;
            }
        }
    }
}
