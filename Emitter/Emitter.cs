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
        public string FullPath { get; set; }
        private string Header { get; set; } 
        private string Text { get; set; }    
        public string FullC { get; set; }

        public Emitter(string fullPath)
        {
            FullPath = fullPath;
        }
        //Adds a line of C to the Text property string
        public void EmitText(string input)
        {
            Text += input;
        }
        //Adds a line of C to the Text property string, with a new line terminator
        public void EmitTextLine(string input)
        {
            Text += input + "\n";
        }
        //Adds a line of C to the Header property string, with a new line terminator
        public void EmitHeaderLine(string input)
        {
            Header += input + "\n";
        }
        // combines the Header and Text strings into the FullC property String
        public void CreateCFile()
        {
            FullC = Header + Text;
        }
        // Creates the C file with the FullPath as the file name and the FullC string as the content
        public void WriteFile()
        {
            File.WriteAllText(FullPath, FullC);
        }

    }
}
