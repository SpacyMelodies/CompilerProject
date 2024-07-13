using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emitter
{
    public class Emitter
    {
        private int varIndex = 0;
        public string FullPath { get; set; }
        private string Header { get; set; } = "bits 64\r\ndefault rel\n"; //Standard NASM header for x64 comp using relative addressing
        private string Bss { get; set; } = "section .Bss\n";
        private string Data { get; set; } = "section .Data\nformat db \"%d\",0xd, 0xa, 0\n"; //instatiates a format string to print numbers to console and a new line feed
        private string Text { get; set; } = "section .text\r\nglobal main\r\nextern ExitProcess\r\nextern scanf\r\nextern printf\r\n\r\nmain:\n    ; shadow space for windowss\r\n    push    rbp\r\n    mov     rbp, rsp\r\n    sub     rsp, 32";
        private string Footer { get; set; } = "FINAL:\r\n   xor rax, rax\r\n    xor rcx, rcx\r\n    xor rdx, rdx\r\n    ; Exit process\r\n    call    ExitProcess\r\n \r\n    ; Epilogue\r\n    mov     rsp, rbp\r\n    pop     rbp\r\n    ret";

        public string FullAsm { get; set; }

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

        private void AddHeader(string input)
        {
            Header = input;
        }

        public void CreateASMFile()
        {
            FullAsm = Header + Bss + Data + Text + Footer;
        }
        public void WriteFile()
        {
            File.WriteAllText(FullPath, FullAsm);
        }

        public string CreateData(string tokenText)
        {
            string index = $"msg{varIndex}";
            Data += $"{index} db {tokenText}, 0xd, 0xa, 0\n"; 
            varIndex++;
            return index;
           
        }
    }
}
