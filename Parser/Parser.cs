using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexer;

namespace Parser
{
    public class Parser
    {
        public Lexer.Lexer Lexer { get; set; }
        private Lexer.Token CurrToken { get; set; }
        private Lexer.Token PeekToken { get; set; }
        public Parser(Lexer.Lexer lexer)
        {
            this.Lexer = lexer;
            CurrToken = lexer.GetToken(); // initializes the first two tokens
            PeekToken = lexer.GetToken();
        }

        public bool CheckToken(Lexer.Token.TokenType tokenType)
        {
            return tokenType == CurrToken.Type;
        }

        public bool CheckPeek(Lexer.Token.TokenType tokenType)
        {
            return tokenType == PeekToken.Type;
        }

        public void NextToken()
        {
            CurrToken = PeekToken;
            PeekToken = Lexer.GetToken();
        }

        public void MatchTokens(Lexer.Token.TokenType tokenType)
        {
            if (!CheckToken(tokenType))
            {
                Abort($"Unexpected Token in syntax tree. Expected {tokenType} but got {CurrToken}");
            }
            else
            {
                NextToken();
            }
        }

        public void Abort(string message)
        {
            Console.Write("Parser Error: " + message);
            throw new Exception("Parser Error: " + message);
        }
    }
}
