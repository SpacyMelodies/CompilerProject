using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emitter
{
    internal class Emitter
    {
        public string FullPath { get; set; }
        public string Code { get; set; }
        public string  Header { get; set; }

        public Emitter(string fullPath)
        {
            FullPath = fullPath;
            Code = string.Empty;
            Header = string.Empty;
        }

        private void Emit(string input)
        {
            Code += input;
        }
        
        private void EmitLine(string input)
        {
            Code += input + "\n";
        }

        private void AddHeader(string input)
        {
            Header = input;
        }
        private void WriteFile()
        {
            File.WriteAllText(FullPath, Code);
        }

    }
}
