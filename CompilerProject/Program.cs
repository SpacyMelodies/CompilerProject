using System.Xml.XPath;
using L = Lexer;
using P = Parser;
using E = Emitter;
internal class Program
{
    // driver program for testing lexer
    private static void Main(string[] args)
    {
        //string source = "IF+-123 foo*THEN/";
        string source = File.ReadAllText("ParserText.Teeny"); // Test file only. will need to make dynamic for actual program (args)
        L.Lexer lexer = new L.Lexer(source);
        E.Emitter emitter = new E.Emitter("output.asm");
        P.Parser parser = new P.Parser(lexer, emitter);
        parser.Program();
        emitter.WriteFile();

        Console.WriteLine("Compilation complete");

    }
}