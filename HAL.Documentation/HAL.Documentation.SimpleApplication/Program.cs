using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.SimpleApplication
{
    class Program
    {
        private static async Task Main(string[] args) 
        {
            //await FunctioningCellFromCatalogs.Run();
            await FunctioningCellFromSerialization.Run();
        }
    }
}
