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
        //string source = "IF+-123 foo*THEN/";
        Console.Write("Enter the Tiny file path you wish to compile: ");
        string path = Console.ReadLine();
        string source = File.ReadAllText(path);
        L.Lexer lexer = new L.Lexer(source);
        E.Emitter emitter = new E.Emitter($"output.c");
        P.Parser parser = new P.Parser(lexer, emitter);
        parser.Program();
        emitter.WriteFile();

        Process p = new Process();
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        processStartInfo.CreateNoWindow = true;
        processStartInfo.FileName = "CMD.exe";
        processStartInfo.Arguments = $"/C build.bat";
        processStartInfo.Arguments += $"/C cl output.c";
        p.StartInfo = processStartInfo;
        p.Start();
        p.WaitForExit();
        Console.WriteLine("Compilation complete");
        Console.WriteLine("Do you want to run the file? y/n");
        if(Console.ReadLine().ToLower() == "y")
        {
            Process z = new Process();
            z.StartInfo.FileName = "CMD.exe";
            z.StartInfo.Arguments = $"/C output.exe";
            z.Start();
            z.Close();
        }
        else
        {
            Console.WriteLine("file saved as output.exe");
            Console.ReadLine();
        }



    }
}