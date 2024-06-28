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
