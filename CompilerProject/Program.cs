using Lexer;
internal class Program
{ 
    private static void Main(string[] args)
    {
        string source = "+- // This is a comment!\n */";
        Lexer.Lexer lexer = new Lexer.Lexer(source); 

        Lexer.Token token = lexer.GetToken();
        
        while (token.Type != Token.TokenType.EOF)
        {
            Console.WriteLine(token.Type);
            token = lexer.GetToken();
        }
    }
}