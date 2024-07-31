using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emitter
{
    public class Emitter
    {
        private int varIndex = 0;
        public string FullPath { get; set; }
        private string Header { get; set; } 
        private string Text { get; set; }    
        public string FullC { get; set; }

        public Emitter(string fullPath)
        {
            FullPath = fullPath;
        }

        public void EmitText(string input)
        {
            Text += input;
        }
        
        public void EmitTextLine(string input)
        {
            Text += input + "\n";
        }

        public void EmitHeaderLine(string input)
        {
            Header += input + "\n";
        }

        public void CreateCFile()
        {
            FullC = Header + Text;
        }
        public void WriteFile()
        {
            File.WriteAllText(FullPath, FullC);
        }

    }
}
