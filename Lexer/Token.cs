using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class Token
    {
        public string TokenText { get; set; }
        public TokenType Type { get; set; }
        public Token(string tokenText, TokenType type) 
        {
           TokenText = tokenText;
           Type = type;
        }
        public enum TokenType
        {
            EOF = -1,
            NEWLINE = 0,
            NUMBER = 2,
            IDENT = 3,
            STRING = 4,
            // Keywords
            LABEL = 101,
            GOTO = 102,
            PRINT = 103,
            INPUTNUM = 104,
            INPUTSTR = 105,
            LET = 106,
            IF = 107,
            THEN = 108,
            ENDIF = 109,
            WHILE = 110,
            REPEAT = 111,
            ENDWHILE = 112,
            // Operators
            EQ = 201,
            PLUS = 202, 
            MINUS = 203,
            ASTERIK = 204,
            SLASH = 205,
            // Comparators
            EQEQ = 206, 
            NOTEQ = 207,
            LT = 208,
            LTEQ = 209,
            GT = 210,
            GTEQ = 211
        }
    }
}
