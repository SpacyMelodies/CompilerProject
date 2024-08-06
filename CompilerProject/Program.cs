using System.Xml.XPath;
using L = Lexer;
using P = Parser;
using E = Emitter;
using System.Diagnostics;
internal class Program
{
    // driver program for testing lexer
    private static void Main(string[] args)
    {

        Console.Write("Enter the Tiny file path you wish to compile: ");
        string path = Console.ReadLine();
        string source = File.ReadAllText(path);
        L.Lexer lexer = new L.Lexer(source);
        E.Emitter emitter = new E.Emitter($"output1.c");
        P.Parser parser = new P.Parser(lexer, emitter);
        parser.Program();
        emitter.WriteFile();

        Process p = new Process();
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        processStartInfo.CreateNoWindow = true;
        processStartInfo.FileName = "CMD.exe";
        processStartInfo.Arguments = $"/C build.bat";
        //processStartInfo.Arguments += $"/C cl output1.c";
        p.StartInfo = processStartInfo;
        p.Start();
        p.WaitForExit();
        Console.WriteLine("Compilation complete");
        Console.WriteLine("Do you want to run the file? y/n");
        if(Console.ReadLine().ToLower() == "y")
        {
            Process z = new Process();
            z.StartInfo.FileName = "CMD.exe";
            z.StartInfo.Arguments = $"/C output1.exe";
            z.Start();
            z.Close();
        }
        else
        {
            Console.WriteLine("file saved as output1.exe");
            Console.ReadLine();
        }



    }
}