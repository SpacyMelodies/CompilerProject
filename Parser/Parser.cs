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
        public static int LineNumber { get; set; } = 1;
        public Lexer.Lexer Lexer { get; set; }
        public Emitter.Emitter Emitter { get; set; }
        private Lexer.Token CurrToken { get; set; }
        private Lexer.Token PeekToken { get; set; }

        private List<string> stringVars;
        private List<string> numberVars;
        private List<string> labelsDeclared;
        private List<string> labelsGotod;

        string emitterTestString = string.Empty;

        public Parser(Lexer.Lexer lexer, Emitter.Emitter emitter)
        {
            this.Lexer = lexer;
            this.Emitter = emitter;

            CurrToken = lexer.GetToken(); // initializes the first two tokens
            PeekToken = lexer.GetToken();

            stringVars = new List<string>();
            numberVars = new List<string>();
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
            Console.Write("Parser Error: " + message + $" at line {LineNumber}");
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
                Emitter.EmitTextLine("return 0");
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
            NextToken();
            if (CheckToken(Token.TokenType.GOTO))
            {                
                labelsGotod.Add(CurrToken.TokenText);
                Emitter.EmitTextLine($"goto {CurrToken.TokenText};");
            }
            else if (CheckToken(Token.TokenType.LABEL))
            {
                if (labelsDeclared.Contains(CurrToken.TokenText))
                {
                    Abort($"{CurrToken.TokenText} has already been declared as a label");
                }
                labelsDeclared.Add(CurrToken.TokenText);
                Emitter.EmitTextLine($"{CurrToken.TokenText}:");
            }
            else if (CheckToken(Token.TokenType.INPUTNUM)) // Input from user
            {
                numberVars.Add(CurrToken.TokenText);
              
            }
            else
            {
                stringVars.Add(CurrToken.TokenText);
                Emitter.EmitBssLine(CurrToken.TokenText + " resb 512");
                Emitter.EmitBssLine("chars resb 4");
                Emitter.EmitTextLine($"sub rsp, 40\r\nmov rcx, -10 ;-10 = stdinputhandle\r\ncall GetStdHandle\r\nmov rcx, rax\r\n xor rdx, rdx\r\nmov rdx, {CurrToken.TokenText}\r\nmov r8, 511\r\nmov r9, chars\r\nmov rax, qword 0\r\nmov qword [rsp+0x20], rax\r\ncall ReadConsoleA\r\nadd rsp, 40");
            }
            MatchToken(Token.TokenType.IDENT);
        }

        private void ParseVariable()
        {
            NextToken();
            if (!stringVars.Contains(CurrToken.TokenText) || !numberVars.Contains(CurrToken.TokenText))
            {
                if (CheckPeek(Token.TokenType.NUMBER))
                {
                    numberVars.Add(CurrToken.TokenText);
                    Emitter.EmitHeaderLine($"float {CurrToken.TokenText};");
                }
                else
                {
                    stringVars.Add(CurrToken.TokenText);
                    Emitter.EmitHeaderLine($"string {CurrToken.TokenText};");
                }
            }
            Emitter.EmitText($"{CurrToken.TokenText} = ");
            MatchToken(Token.TokenType.IDENT);
            MatchToken(Token.TokenType.EQ);

            // stuff will need to be added to parse strings

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
                Emitter.EmitText("printf(\"" + CurrToken.TokenText + "\\n\");");
                NextToken();
            }
            else if (CheckToken(Token.TokenType.IDENT))
            {
                if(numberVars.Contains(CurrToken.TokenText)) // if the variable is a number, set up that print statement
                {
                    Emitter.EmitTextLine($"THIS NEEDS WORK - PARSEPRINT IDENT NUMBER");
                }
                else
                {
                    Emitter.EmitTextLine($"THIS NEEDS WORK - PARSEPRINT IDENT STRING");
                }               
            }
            else // if its a constant number or expression
            {
                Emitter.EmitText("printf(\"%" + ".2f\\n\", (float)(");
                Expression();
                Emitter.EmitTextLine("));");
            }

        }// need to fix parse print

        private void NewLine()
        {
            LineNumber++;
            Console.WriteLine("NEW LINE");
            MatchToken(Token.TokenType.NEWLINE);
            while (CheckToken(Token.TokenType.NEWLINE))
            {
                LineNumber++;
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
            Emitter.EmitText($"{CurrToken.TokenText}");
            if (CheckToken(Token.TokenType.IDENT))
            {
                MatchToken(Token.TokenType.IDENT);               
            }
            else
            {
                MatchToken(Token.TokenType.NUMBER);
            }
        }
    }
}
