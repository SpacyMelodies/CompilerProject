using System.Xml.XPath;
using L = Lexer;
using P = Parser;
using E = Emitter;
using System.Diagnostics;
using Lexer;

/// <summary>
/// COSC3506 Final Project
/// Mitchell Plunkett - Student ID #239499150
/// 
/// TinyLang to C compiler
/// 
/// Program compiles a user program written in custom programming language TinyLang
/// and returns a valid C file. Then calls the microsoft C compiler to assemble link
/// and return an executable to the user
/// 
/// 
/// </summary>

internal class Program
{
    // Compilation driver
    private static void Main(string[] args)
    {
        // Prompts user for file path and output file names
       Console.Write("Enter the Tiny file path you wish to compile: ");

        string? path = Console.ReadLine();
        string source = File.ReadAllText("ParserText.TL");
        Console.WriteLine("Enter a name for the output files");
        string? fileName = Console.ReadLine();
        L.Lexer lexer = new L.Lexer(source);
        E.Emitter emitter = new E.Emitter($"{fileName}.c");
        P.Parser parser = new P.Parser(lexer, emitter);

        //starts compilation process and returns a valid C file
        parser.Program();
        emitter.WriteFile();


        // command script that compiles C file to executable
        Process p = new Process();
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "CMD.exe",
            Arguments = $"/C build.bat && cl {fileName}.c",  
            CreateNoWindow = true,  // Uncomment this line if you want to hide the compilation window
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        p.StartInfo = processStartInfo;
        p.Start();
        p.WaitForExit();
       if ( p.ExitCode != 0 ) // delivers errors if C did not compile
        {
            Console.WriteLine("C compilation Error. Please check your TinyLang program");
            Environment.Exit(p.ExitCode);
        }

       // allows user to run program immediatley, or simply save EXE
        Console.WriteLine("Compilation complete");
       Console.WriteLine("Do you want to run the file? y/n");
        if(Console.ReadLine().ToLower() == "y")
        {
            Process z = new Process();
            z.StartInfo.FileName = "CMD.exe";
            z.StartInfo.Arguments = $"/C {fileName}.exe";
            z.Start();
            z.Close();
        }
        else
        {
            Console.WriteLine($"file saved as {fileName}.exe");
            Console.ReadLine();
        }
    }
}