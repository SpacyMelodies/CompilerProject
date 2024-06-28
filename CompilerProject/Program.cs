using Lexer;
internal class Program
{ 
    // NOTE: next work is identifiers / key words section
    private static void Main(string[] args)
    {
        string source = "+-123 9.8654 */";
        Lexer.Lexer lexer = new Lexer.Lexer(source); 

        Lexer.Token token = lexer.GetToken();
        
        while (token.Type != Token.TokenType.EOF)
        {
            Console.WriteLine(token.Type);
            token = lexer.GetToken();
        }
    }
}