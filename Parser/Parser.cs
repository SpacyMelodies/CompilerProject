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
    public class Parser
    {
        public Lexer.Lexer Lexer { get; set; }
        private Lexer.Token CurrToken { get; set; }
        private Lexer.Token PeekToken { get; set; }
        private List<string> variables;
        private List<string> labelsDeclared;
        private List<string> labelsGotod;
        string emitterTestString = string.Empty;
        public Parser(Lexer.Lexer lexer)
        {
            this.Lexer = lexer;
            CurrToken = lexer.GetToken(); // initializes the first two tokens
            PeekToken = lexer.GetToken();
            variables = new List<string>();
            labelsDeclared = new List<string>();
            labelsGotod = new List<string>();
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
                Abort($"Unexpected Token in syntax tree. Expected {tokenType} but got {CurrToken.Type}");
            }
            else
            {
                NextToken();
            }
        }

        public void Abort(string message)
        {
            Console.WriteLine();
            Console.Write("Parser Error: " + message);
            Console.WriteLine();
            throw new Exception("Parser Error: " + message);

            //Environment.Exit(-1); Might include this in final
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
            string labelListComp = CompareLabelLists();
            if (labelListComp == string.Empty)
            {
                Console.WriteLine(emitterTestString);
            }
            else
            {
                Abort($"GOTO error: trying to GOTO label \"{labelListComp}\" that does not exist");
            }
        }

        private string CompareLabelLists()
        {
            foreach (var label in labelsGotod)
            {
                if (labelsDeclared.Contains(label))
                {
                    continue;
                }
                else
                {
                    return label;
                }
            }
            return string.Empty;
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
            emitterTestString += ";";
            NewLine();
        }

        private void ParseIdentifier()
        {
            Console.WriteLine("STATEMENT - " + CurrToken.Type.ToString());

            if (CheckToken(Token.TokenType.GOTO))
            {
                NextToken();
                labelsGotod.Add(CurrToken.TokenText);
            }
            else if (CheckToken(Token.TokenType.LABEL))
            {
                NextToken();
                if (labelsDeclared.Contains(CurrToken.TokenText))
                {
                    Abort($"{CurrToken.TokenText} has already been declared as a label");
                }
                labelsDeclared.Add(CurrToken.TokenText);
            }
            else
            {
                variables.Add(CurrToken.TokenText);
                NextToken();
            }
            MatchToken(Token.TokenType.IDENT);
        }

        private void ParseVariable()
        {
            Console.WriteLine("STATEMENT - LET");
            NextToken();
            if (!variables.Contains(CurrToken.TokenText))
            {
                variables.Add(CurrToken.TokenText);
            }
            MatchToken(Token.TokenType.IDENT);
            MatchToken(Token.TokenType.EQ);
            Expression();
        }

        private void ParseLoop()
        {
            Console.WriteLine("STATEMENT - WHILE");
            NextToken();
            Comparison();
            MatchToken(Token.TokenType.REPEAT);
            NewLine();
            while (!CheckToken(Token.TokenType.ENDWHILE))
            {
                Statement();
            }
            MatchToken(Token.TokenType.ENDWHILE);
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
            emitterTestString += "Console.WriteLine(";
            //Console.WriteLine("STATEMENT - PRINT");
            NextToken();
            if (CheckToken(Token.TokenType.STRING))
            {
                NextToken();
            }
            else
            {
                Expression();
            }
            emitterTestString += ")";
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
           // Console.WriteLine("COMPARISON");
            Expression();
            if ((int)CurrToken.Type >= 206 && (int)CurrToken.Type <= 211)
            {
                emitterTestString += CurrToken.TokenText;
                NextToken();
                Expression();
            }
            else
            {
                Abort($"Comparison error: expected comparator got \"{CurrToken.Type}\"");
            }
        }
        private void Expression()
        {
            //emitterTestString += CurrToken.TokenText;
            //Console.WriteLine("EXPRESSION");
            Term();
            while (CheckToken(Token.TokenType.PLUS) || CheckToken(Token.TokenType.MINUS))
            {
                emitterTestString += CurrToken.TokenText;
                NextToken();
                Term();
            }
        }
        private void Term()
        {
            //emitterTestString += CurrToken.TokenText;
            //Console.WriteLine("TERM");
            Unary();
            while (CheckToken(Token.TokenType.SLASH) || CheckToken(Token.TokenType.ASTERIK))
            {
                emitterTestString += CurrToken.TokenText;
                NextToken();
                Unary();
            }
        }
        private void Unary()
        {
            //emitterTestString += CurrToken.TokenText;
           // Console.WriteLine("UNARY");
            if (CheckToken(Token.TokenType.PLUS) || CheckToken(Token.TokenType.MINUS))
            {
                emitterTestString += CurrToken.TokenText;
                NextToken();
            }
            Primary();
        }
        private void Primary()
        {
            if (CheckToken(Token.TokenType.IDENT))
            {
                emitterTestString += CurrToken.TokenText;
                //Console.WriteLine($"PRIMARY - " + CurrToken.TokenText);
                MatchToken(Token.TokenType.IDENT);
            }
            else
            {
                emitterTestString += CurrToken.TokenText;
                //Console.WriteLine($"PRIMARY - " + CurrToken.TokenText);
                MatchToken(Token.TokenType.NUMBER);
            }
        }
    }
}
