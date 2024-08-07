using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lexer
{
    public class Lexer
    {
        private char currChar{ get; set; }
        private int currPos { get; set; }
        public string source { get; set; }
        public Lexer(string source)
        {
            this.source = source + "\n";
            this.currChar = ' ';
            this.currPos = -1;
            this.NextChar();
        }

        // shifts the char pointer to the next char in the token
        public void NextChar()
        {
            this.currPos++;
            if (currPos >= source.Length)
            {
                currChar = '\0'; // EOF
            }
            else
            {
                currChar = source[currPos];
            }
        }

        // looks at the next char value in the source file/string
        public char Peek()
        {
            if (currPos + 1 >= source.Length)
            {
                return '\0';
            }
            return source[currPos + 1];
        }

        // returns token containing token type and content(string)
        public Token GetToken()
        {
            Token token = null;
            switch (currChar)
            {
                //operators
                case '+':
                    token = new Token(currChar.ToString(), Token.TokenType.PLUS);
                    break;
                case '-':
                    token = new Token(currChar.ToString(), Token.TokenType.MINUS);
                    break;
                case '*':
                    token = new Token(currChar.ToString(), Token.TokenType.ASTERIK);
                    break;
                case '/':
                    if (Peek() == '/')
                    {
                        NextChar();
                        SkipComments();
                        token = new Token(Peek().ToString(), Token.TokenType.NEWLINE);
                        NextChar();
                        break;
                    }
                    else
                    {
                        token = new Token(currChar.ToString(), Token.TokenType.SLASH);
                        break;
                    }
                case '=':
                    if (Peek() == '=') // if we are looking at a = and next is an = return ==
                    {
                        token = new Token(currChar + Peek().ToString(), Token.TokenType.EQEQ);
                        NextChar();
                        break;
                    }
                    else
                    {
                        token = new Token(currChar.ToString(), Token.TokenType.EQ);
                        break;
                    }
                case '>':
                    if (Peek() == '=')
                    {
                        token = new Token(currChar + Peek().ToString(), Token.TokenType.GTEQ);
                        NextChar();
                        break;
                    }
                    else
                    {
                        token = new Token(currChar.ToString(), Token.TokenType.GT);
                        break;
                    }
                case '<':
                    if (Peek() == '=')
                    {
                        token = new Token(currChar + Peek().ToString(), Token.TokenType.LTEQ);
                        NextChar();
                        break;
                    }
                    else
                    {
                        token = new Token(currChar.ToString(), Token.TokenType.LT);
                        break;
                    }
                case '!':
                    if (Peek() == '=')
                    {
                        token = new Token(currChar + Peek().ToString(), Token.TokenType.NOTEQ);
                        NextChar(); 
                        break;
                    }
                    else
                    {
                        Abort("Excpected '!=', but got '!'");
                        break;
                    }
                //strings
                case '"':
                    token = new Token(GetString(), Token.TokenType.STRING);
                    break;
                // Numbers and Lexemes
                case var _check when (currChar >= '0' && currChar <= '9'): // _check doesnt do anything, this handles numeric values for lexing
                    token = new Token(GetNumber(), Token.TokenType.NUMBER);
                    break;
                case var _check when (char.IsLetter(currChar)):
                    string lexeme = GetLexeme();
                    if (Enum.TryParse(lexeme, false, out Token.TokenType result)) // chagned ignore case 07/22 from T -> false. 
                    {
                        token = new Token(lexeme, result);
                    }
                    else
                    {
                        token = new Token(lexeme, Token.TokenType.IDENT);
                    }
                    break;  
                // NewLines & whitespace
                case '\n':
                    token = new Token(currChar.ToString(), Token.TokenType.NEWLINE);
                    break;
                case '\0':
                    token = new Token(currChar.ToString(), Token.TokenType.EOF);
                    break;
                case ' ': // handles whitespace skips
                case '\t':
                case '\r':
                    NextChar();
                    return GetToken();
                default:
                    Abort("Unknown Token \"" + currChar.ToString() + "\" not recognized");
                    break;
            }
            NextChar();
            return token;
        }
        
        // returns a lexeme string to the caller, with checks defined for lexems
        private string GetLexeme()
        {
            string valueString = "";
            while(char.IsLetterOrDigit(currChar))
            {
                valueString += currChar;
                if (!char.IsLetterOrDigit(Peek()))
                {
                    break;
                }
                else
                {
                    NextChar();
                }
            }
            return valueString;
        }

        // returns a number string to the caller, with checks defined for numbers
        private string GetNumber() // NOTE: see if I can get this a bit more concise
        {
            string valueString = "";
            int decimalCount = 0;
            while ((currChar >= '0' && currChar <= '9') || currChar == '.')
            {
                if (currChar == '.')
                {

                    if (decimalCount == 0)
                    {
                        decimalCount++;
                    }
                    else
                    {
                        Abort("Error in Number: number had more than 1 decmial point");
                    }
                    if (Peek() < '0' || Peek() > '9')
                    {
                        Abort("Error in Number: no digit after decimal point");
                    }
                }
                valueString += currChar;
                if ((Peek() >= '0' && Peek() <= '9') || Peek() == '.')
                {
                    NextChar();
                }
                else
                {
                    break;
                }
            }
            return valueString;
        }

        // returns user string , with checks defined for checking string bounds 
        private string GetString()
        {
            string valueString = string.Empty;
            while(Peek() != '"')
            {
                if (currChar == '\n' || currChar == '\r' || currChar == '\t' || currChar == '\\' || currChar == '%')
                {
                    Abort("Error in string: Special char used in string");
                }
                else
                {
                    valueString += currChar;
                    NextChar();
                }
            }
            valueString += currChar; // adds the last char to the string
            NextChar();
            valueString += currChar; // adds the last "
            return valueString;
        }

        // skips '//' comments until a new line
        private void SkipComments()
        {
            while (Peek() != '\n')
            {
                NextChar();
            }
            return;
        }

        // prints error message to console then throws exception
        public void Abort(string message)
        {
            Console.Error.WriteLine ("Lexing Error: " + message);
            throw new Exception ("Lexing Error: " + message );            
        }
    }
}
