using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class Lexer
    {
        public char currChar{ get; set; }
        public int currPos { get; set; }
        public string source { get; set; }
        public Lexer(string source)
        {
            this.source = source + "\n";
            this.currChar = ' ';
            this.currPos = -1;
            this.NextChar();
        }

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

        public char Peek()
        {
            if (currPos + 1 >= source.Length)
            {
                return '\0';
            }
            return source[currPos + 1];
        }

        public Token GetToken()
        {
            Token token = null;
            switch (currChar)
            {
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
                    if (Peek() == '=')
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
                case '"':
                    token = new Token(GetString(), Token.TokenType.STRING);
                    break;
                case var _check when (currChar >= '0' && currChar <= '9'): // _check doesnt do anything, this handles numeric values for lexing
                    token = new Token(GetNumber(), Token.TokenType.NUMBER);
                    break;
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

        private string GetNumber() // NOTE: see if I can get this a bit more concise
        {
            string returnString = "";
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
                returnString += currChar;
                if ((Peek() >= '0' && Peek() <= '9') || Peek() == '.')
                {
                    NextChar();
                }
                else
                {
                    break;
                }
            }
            return returnString;
        }

        // need to edit this to not allow special chars
        private string GetString()
        {
            string returnString = "";
            while(Peek() != '"')
            {
                if (currChar == '\n' || currChar == '\r' || currChar == '\t' || currChar == '\\' || currChar == '%')
                {
                    NextChar();
                    continue;
                }
                else
                {
                    returnString += currChar;
                    NextChar();
                }
            }
            NextChar();
            return returnString;
        }

        private void SkipComments()
        {
            while (Peek() != '\n')
            {
                NextChar();
            }
            return;
        }

        public void Abort(string message)
        {
            Console.Error.WriteLine ("Lexing Error: " + message);
            throw new Exception ("Lexing Error: " + message );            
        }
    }
}
