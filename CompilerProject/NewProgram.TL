// example program in TinyLang
// repeats asking user to input number greater than 44

PRINT "Enter a number above 44"
INPUTNUM myNum

WHILE myNum <= 44 REPEAT
IF myNum <= 44 THEN
PRINT "Thats under 44, try again"
INPUTNUM myNum
ENDIF
ENDWHILE
PRINT myNum 
PRINT "is greater than 44"