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
        public Emitter.Emitter Emitter { get; set; }
        private Lexer.Token CurrToken { get; set; }
        private Lexer.Token PeekToken { get; set; }

        private List<string> stringVars = new List<string>();
        private List<string> numberVars = new List<string>();
        private List<string> labelsDeclared = new List<string>();
        private List<string> labelsGotod = new List<string>();
        public Parser(Lexer.Lexer lexer, Emitter.Emitter emitter)
        {
            this.Lexer = lexer;
            this.Emitter = emitter;

            CurrToken = lexer.GetToken(); // initializes the first two tokens
            PeekToken = lexer.GetToken();
        }

        public bool CheckToken(Lexer.Token.TokenType tokenType)
        {
            return tokenType == CurrToken.Type;
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
            Emitter.EmitTextLine("#include <stdio.h>");
            Emitter.EmitTextLine("int main(void){");

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
                Emitter.EmitTextLine("return 0;");
                Emitter.EmitTextLine("}");
                Emitter.CreateCFile();
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
                case Token.TokenType.INPUTNUM:
                case Token.TokenType.INPUTSTR:
                    ParseIdentifier();
                    break;
                default:
                    Abort($"Statement error: currToken is {CurrToken.Type}, need a valid statement");
                    break;
            }
            Emitter.EmitTextLine(""); // emits a new line for the asm file
            NewLine();
        }

        private void ParseIdentifier()
        {         
            
            if (CheckToken(Token.TokenType.GOTO))
            {
                NextToken();
                labelsGotod.Add(CurrToken.TokenText);
                Emitter.EmitTextLine($"goto {CurrToken.TokenText};");
            }
            else if (CheckToken(Token.TokenType.LABEL))
            {
                NextToken();
                if (labelsDeclared.Contains(CurrToken.TokenText))
                {
                    Abort($"{CurrToken.TokenText} has already been declared as a label");
                }
                labelsDeclared.Add(CurrToken.TokenText);
                Emitter.EmitTextLine($"{CurrToken.TokenText}:");
            }
            else if (CheckToken(Token.TokenType.INPUTNUM)) // Input number from user
            {
                NextToken();
                if (!numberVars.Contains(CurrToken.TokenText)) 
                {
                    numberVars.Add(CurrToken.TokenText);
                    Emitter.EmitHeaderLine($"float {CurrToken.TokenText};");
                }
                // edit for error checking!!!!
                Emitter.EmitTextLine($"scanf(\"%f\",&{CurrToken.TokenText});");
                Emitter.EmitTextLine($"while ((getchar()) != '\\n');"); // clears input buffer 

            }
            else
            {
                NextToken();
                if (!stringVars.Contains(CurrToken.TokenText))
                {
                    stringVars.Add(CurrToken.TokenText);
                    Emitter.EmitHeaderLine($"char {CurrToken.TokenText}[512];");
                } 
                Emitter.EmitTextLine($"fgets({CurrToken.TokenText}, 511, stdin);");
            }
            MatchToken(Token.TokenType.IDENT);
        }

        private void ParseVariable()
        {
            NextToken();

            string identString = CurrToken.TokenText;
            //Emitter.EmitText($"char {CurrToken.TokenText}[] = ");

            MatchToken(Token.TokenType.IDENT);
            MatchToken(Token.TokenType.EQ);
            if (!stringVars.Contains(identString) && !numberVars.Contains(identString))
            {
                if (CheckToken(Token.TokenType.NUMBER))
                {
                    numberVars.Add(identString);
                    Emitter.EmitText($"{identString} = ");
                    Emitter.EmitHeaderLine($"float {identString};");
                }
                else
                {
                    Emitter.EmitText($"char {identString}[] = ");
                    stringVars.Add(identString);
                }
            }
            else
            {
                Emitter.EmitText($"{identString} = ");
            }
            Expression();
            Emitter.EmitTextLine(";");   
        }

        private void ParseLoop()
        {
            NextToken();
            Emitter.EmitText("while(");
            Comparison();
            MatchToken(Token.TokenType.REPEAT);
            NewLine();
            Emitter.EmitText("){");
            while (!CheckToken(Token.TokenType.ENDWHILE))
            {
                Statement();
            }
            MatchToken(Token.TokenType.ENDWHILE);
            Emitter.EmitTextLine("}");
            // when we return to statement, we get the newline check
        }

        private void ParseIfThen()
        {
            NextToken();
            Emitter.EmitText("if(");
            Comparison();
            MatchToken(Token.TokenType.THEN);
            NewLine();
            Emitter.EmitTextLine("){");
            while (!CheckToken(Token.TokenType.ENDIF))
            {
                Statement();
            }
            MatchToken(Token.TokenType.ENDIF);
            Emitter.EmitTextLine("}");
        }

        private void ParsePrint()
        {         
            NextToken();
            if (CheckToken(Token.TokenType.STRING))
            {
                Emitter.EmitText($"printf(\"%s\\n\",{CurrToken.TokenText});");
                NextToken();
            }
            else if (CheckToken(Token.TokenType.IDENT))
            {
                if(numberVars.Contains(CurrToken.TokenText)) // if the variable is a number, set up that print statement
                {
                    Emitter.EmitText($"printf(\"%.2f\\n\", (float)(");
                    Expression();
                    Emitter.EmitTextLine("));");
                }
                else
                {
                    Emitter.EmitText($"printf(\"%s\\n\",{CurrToken.TokenText});");
                    NextToken();
                }               
            }
            else // if its a constant number or expression
            {
                Emitter.EmitText("printf(\"%.2f\\n\", (float)(");
                Expression();
                Emitter.EmitTextLine("));");
            }
            
        }

        private void NewLine()
        {
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
                Emitter.EmitText(CurrToken.TokenText);
                NextToken();
                Expression();
            }
            else
            {
                Abort($"Comparison error: expected comparator got \"{CurrToken.Type}\"");
            }
            while((int)CurrToken.Type >= 206 && (int)CurrToken.Type <= 211)
            {
                Emitter.EmitText(CurrToken.TokenText);
                NextToken();
                Expression();
            }
        }
        private void Expression()
        {
            //emitterTestString += CurrToken.TokenText;
            //Console.WriteLine("EXPRESSION");
            Term();
            while (CheckToken(Token.TokenType.PLUS) || CheckToken(Token.TokenType.MINUS))
            {
                Emitter.EmitText(CurrToken.TokenText);
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
                Emitter.EmitText(CurrToken.TokenText);
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
                Emitter.EmitText(CurrToken.TokenText);
                NextToken();
            }
            Primary();
        }
        private void Primary() 
        {
            if (CheckToken(Token.TokenType.IDENT))
            {
                if(!numberVars.Contains(CurrToken.TokenText) && !stringVars.Contains(CurrToken.TokenText))
                {
                    Abort("Error: referencing variable before assignment");
                }
                Emitter.EmitText(CurrToken.TokenText);
                MatchToken(Token.TokenType.IDENT);               
            }
            else if (CheckToken(Token.TokenType.NUMBER))
            {
                Emitter.EmitText(CurrToken.TokenText);
                MatchToken(Token.TokenType.NUMBER);
            }
            else
            {
                Emitter.EmitText(CurrToken.TokenText);
                MatchToken(Token.TokenType.STRING);
            }
        }
    }
}
