using Lexer;
internal class Program
{ 
    // driver program for testing lexer
    private static void Main(string[] args)
    {
        string source = "IF+-123 foo*THEN/";
        Lexer.Lexer lexer = new Lexer.Lexer(source); 

        Lexer.Token token = lexer.GetToken();
        
        while (token.Type != Token.TokenType.EOF)
        {
            Console.WriteLine(token.Type);
            token = lexer.GetToken();
        }
    }
}