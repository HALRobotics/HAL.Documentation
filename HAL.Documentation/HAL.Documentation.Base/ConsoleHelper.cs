using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
    public class ConsoleHelper
    {
        public void GetInputsFromConsole()
        {
            var path = GetPathFromConsole();
            
        }

        private string GetPathFromConsole()
        {
            string path= null;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Write the path of the cell you want to import.");
                Console.Write("Path:");
                var inputPath = Console.ReadLine();
                var fileName = Path.GetFileName(inputPath);
                if (fileName is null) Console.WriteLine("No document found for this path.");
                else
                {
                    Console.WriteLine($"{fileName} found. Do you want to continue with this file? (y/n)");
                    if (Console.ReadKey().KeyChar.ToString() == "y")
                    {
                        path = inputPath;
                        break;
                    }
                }
            }
            if (path is null) throw new Exception("Path not found");
            else return path;
        }

       
    }
}
