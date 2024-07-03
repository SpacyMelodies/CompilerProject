using L = Lexer;
using P = Parser;
internal class Program
{ 
    // driver program for testing lexer
    private static void Main(string[] args)
    {
        //string source = "IF+-123 foo*THEN/";
        string source = File.ReadAllText("ParserTest.txt"); // Test file only. will need to make dynamic for actual program (args)
        L.Lexer lexer = new L.Lexer(source);
        P.Parser parser = new P.Parser(lexer);
        parser.Program();
        Console.ReadLine();

    }
}