using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Lexer;

namespace Parser
{
    // Need to implement non-implemented functions
    // parse if/then  if token -> comparator -> then token -> statement(s) -> endif token -> NL token
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

        public void MatchToken(Lexer.Token.TokenType tokenType)
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

        public void Program()
        {
            Console.WriteLine("PROGRAM");

            while (CheckToken(Token.TokenType.NEWLINE))
            {
                NextToken();
            }
            
            while (!CheckToken(Token.TokenType.EOF))
            {
                Statement();
            }
            Console.WriteLine("Parsing Completed");
        }

        public void Statement()
        {
            switch (CurrToken.Type)
            {
                case Token.TokenType.PRINT:
                    ParsePrint();
                    break;
                case Token.TokenType.IF:
                    ParseIfThen();
                    break;
                case Token.TokenType.WHILE:
                    ParseLoop();
                    break;
                case Token.TokenType.LET:
                    ParseVariable();
                    break;
                case Token.TokenType.LABEL:
                case Token.TokenType.GOTO:
                case Token.TokenType.INPUT:
                    ParseIdentifier();
                    break;
                default:
                    Abort($"Statement error: currToken is {CurrToken.Type}, need a valid statement");
                    break;
            }
            NewLine();
        }

        private void ParseIdentifier()
        {
            Console.WriteLine("STATEMENT - " + CurrToken.Type.ToString());
            NextToken();
            MatchToken(Token.TokenType.IDENT);
        }

        private void ParseVariable()
        {
            Console.WriteLine("STATEMENT - LET");
            NextToken();
            MatchToken(Token.TokenType.IDENT);
            MatchToken(Token.TokenType.EQ);
            Expression();
        }

        private void ParseLoop()
        {
            Console.WriteLine("STATEMENT - WHILE");
            NextToken();
            MatchToken(Token.TokenType.REPEAT);
            NewLine();
            while (!CheckToken(Token.TokenType.ENDIF))
            {
                Statement();
            }
            MatchToken(Token.TokenType.ENDIF);
            // when we return to statement, we get the newline check
        }

        private void ParseIfThen()
        {
            Console.WriteLine("STATEMENT - IF");
            NextToken();
            Comparison();
            MatchToken(Token.TokenType.THEN);
            NewLine();
            while (!CheckToken(Token.TokenType.ENDIF))
            {
                Statement();
            }
            MatchToken(Token.TokenType.ENDIF);
        }

        private void ParsePrint()
        {
            Console.WriteLine("STATEMENT - PRINT");
            NextToken();
            if (CheckToken(Token.TokenType.STRING))
            {
                NextToken();
            }
            else
            {
                Expression();
            }
        }

        private void NewLine()
        {
            Console.WriteLine("NEW LINE");
            MatchToken(Token.TokenType.NEWLINE);
            while (CheckToken(Token.TokenType.NEWLINE))
            {
                NextToken();
            }
        }
        private void Comparison()
        {
            Console.WriteLine("C");
            Expression();
            if((int)CurrToken.Type >= 206 || (int)CurrToken.Type <= 211)
            {
                NextToken();
                Expression();
            }
            else
            {
                Abort($"Comparison error: expected comparator got {CurrToken.Type}");
            }
        }
        private void Expression()
        {
            Console.WriteLine("Expression");
            Term();
            while(CheckToken(Token.TokenType.PLUS) || CheckToken(Token.TokenType.MINUS))
            {
                NextToken();
                Term();
            }
        }
        private void Term()
        {
            Console.WriteLine("Term");
            Unary();
            while(CheckToken(Token.TokenType.SLASH) || CheckToken(Token.TokenType.ASTERIK))
            {
                NextToken();
                Unary();
            }
        }
        private void Unary()
        {
            Console.WriteLine("Unary");
            if (CheckToken(Token.TokenType.PLUS) || CheckToken(Token.TokenType.MINUS))
            {
                NextToken();
            }
            Primary();
        }
        private void Primary()
        {
            
            if (CheckToken(Token.TokenType.IDENT))
            {
                Console.WriteLine($"PRIMARY - " + CurrToken.TokenText);
                MatchToken(Token.TokenType.IDENT);
            }
            else
            {
                Console.WriteLine($"PRIMARY - " + CurrToken.TokenText);
                MatchToken(Token.TokenType.NUMBER);
            }
        }
    }
}
