using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class calculus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(stringyEvaluate("2*-3/-Sin(x)",.5235f,-.87f));
        //Debug.Log(stringyNumericDerivative("Sin(x*y)",.5f,-.87f,2,3));
        //Debug.Log(stringyDerivative("x*(5+y)","x"));
        //List<float> gradient = stringyNumericGradient("Sin(x)+Cos(y)",.566f,1.28956f);
        //Debug.Log(gradient[0].ToString()+" "+gradient[1].ToString());
        //Debug.Log(stringyDerivative("(x+y)","x"));
    }//End Start

    // Update is called once per frame
    void Update()
    {
        
    }
    
   /*
    Vector3 stringyVectorFieldCurl(string Fx, string Fy, string Fz, float xVal, float yVal) {
        stringyNumericDerivative(Fz, xVal, yVal, );

        return new Vector3();
    }//End stringyVectorFieldCurl method
    */

    int factorial(int s) {
        int acc = 1;
        for (int i = s; i > 0; i--) {
            acc *= i;
        }//End for loop
        return acc;
    }//End factorial function

    float stringyNumericDerivative(string s, float xVal, float yVal, float vx, float vy) {
        Vector2 v = stringyNumericGradient(s, xVal, yVal);
        return (vx*v.x + vy*v.y)/Mathf.Sqrt(Mathf.Pow(vx,2f) + Mathf.Pow(vy,2f));
    }//End stringyNumericDerivative

    public Vector2 stringyNumericGradient(string s, float xVal, float yVal) {
        float h = .001f;
        float xDeriv = (stringyEvaluate(s,xVal+h,yVal)-stringyEvaluate(s,xVal,yVal))/h;
        float yDeriv = (stringyEvaluate(s,xVal,yVal+h)-stringyEvaluate(s,xVal,yVal))/h;
        return new Vector2(xDeriv,yDeriv);
    }//End stringyNumericGradient

    public float stringyEvaluate(string s, float xVal, float yVal) {
        string myS = s.Replace(" ",""); //Get rid of the spaces
        while (myS.IndexOf("--") + myS.IndexOf("+-") > -2) {
            myS = myS.Replace("--","+").Replace("+-","-");
        }//End while loop to rectify negative sign confusions

        int lastAdd = -1;
        int lastSubtract = -1;
        int lastMultiply = -1;
        int lastDivision = -1;
        int lastExponent = -1;
        int lastFactorial = -1;

        int firstSin = myS.IndexOf("Sin(",0);
        int firstCos = myS.IndexOf("Cos(",0);
        int firstTan = myS.IndexOf("Tan(",0);
        int firstCot = myS.IndexOf("Cot(",0);
        int firstSec = myS.IndexOf("Sec(",0);
        int firstCsc = myS.IndexOf("Csc(",0);
        int firstExp = myS.IndexOf("Expr(",0);
        int firstLog = myS.IndexOf("Log(",0);
        int firstSqrt = myS.IndexOf("Sqrt(",0);

        int leftParen = 0;
        int firstLeftParen = -1;

        /* //Turn negative signs into 0-
        for (int i = 0; i < myS.Length; i++) {
            string character = myS.Substring(i,1);
            if (character == "-") { //If we found a negative or minus
                if (i == 0) { //If this was the first value
                    return stringyEvaluate("(0"+myS+")",xVal,yVal); //Just try again with the 0 lol
                } else { //If the minus sign was somewhere else
                    string prev = myS.Substring(i-1,1);
                    if (prev == "+" || prev == "-" || prev == "*" || prev == "/" || prev == "^") { //It's a negative sign
                        int end = -1;
                        for (int it = i+1; it < myS.Length; it++) {
                            string character2 = myS.Substring(it,1);
                            if (!(character2 == "0" || character2 == "1"|| character2 == "2"|| character2 == "3"|| character2 == "4"
                                || character2 == "5"|| character2 == "6"|| character2 == "7"|| character2 == "8"
                                || character2 == "9"|| character2 == "."|| character2 == "f"
                                || character2 == "x"|| character2 == "y")) {
                                    if (end == -1) {end = it-1; }
                            }//End if to find end of number
                            if (end == -1) { end = myS.Length - 1; }//End if for if we didn't find the end
                        }//End for loop to find end of number
                        if (end == myS.Length - 1) {
                            return stringyEvaluate(myS.Substring(0,i) + "(0"+myS.Substring(i,end-i+1) +")",xVal,yVal);
                        } else {
                            return stringyEvaluate(myS.Substring(0,i) + "(0"+myS.Substring(i,end-i+1) +")"+myS.Substring(end+1),xVal,yVal);
                        }//End if-else for if end is the end of myS
                    }//End if for if it's a negative sign
                }//End if-else for where the - is
            }//End if for if the character was a -
        }//End for loop
        */

        if (firstSin+firstCos+firstTan+firstCot+firstSec+firstCsc+firstExp+firstLog+firstExp > -9) { //If functions left
            if (firstSin > -1) {
                leftParen = 0;
                for (int i = firstSin; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            
                            return stringyEvaluate("" + myS.Substring(0,firstSin)
                                + round(Mathf.Sin(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstCos > -1) {
                leftParen = 0;
                for (int i = firstCos; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstCos)
                                + round(Mathf.Cos(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstTan > -1) {
                leftParen = 0;
                for (int i = firstTan; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstTan)
                                + round(Mathf.Tan(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstCot > -1) {
                leftParen = 0;
                for (int i = firstCot; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstCot)
                                + round((1/Mathf.Tan(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal)))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstSec > -1) {
                leftParen = 0;
                for (int i = firstSec; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstSec)
                                + round((1/Mathf.Cos(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal)))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstCsc > -1) {
                leftParen = 0;
                for (int i = firstCsc; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstCsc)
                                + round((1/Mathf.Sin(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal)))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstLog > -1) {
                leftParen = 0;
                for (int i = firstLog; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstLog)
                                + round(Mathf.Log(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal),Mathf.Exp(1))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstExp > -1) {
                leftParen = 0;
                for (int i = firstExp; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstExp)
                                + round(Mathf.Exp(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            } else if (firstSqrt > -1) {
                leftParen = 0;
                for (int i = firstSqrt; i < myS.Length; i++) {
                    string character = myS.Substring(i,1);
                    if (character == "(") {
                        leftParen++;
                        if (firstLeftParen == -1) {
                            firstLeftParen = i;
                        }//End if for firstLeftParen
                    } else if (character == ")") {
                        leftParen--;
                        if (leftParen == 0) { //If our parantheses are matched now
                            string end = "";
                            if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                            }//End if-else for end string
                            return stringyEvaluate("" + myS.Substring(0,firstSqrt)
                                + round(Mathf.Sqrt(stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal))).ToString()
                                + end ,xVal,yVal);
                        }//End if for matched
                    }//End if else for hit parantheses
                }//End for loop
            }//End if-else for which function
        }//End if for functions left

        //At this point there are no functions

        leftParen = 0;
        firstLeftParen = myS.IndexOf("(",0);
        if (firstLeftParen > -1) { //If parantheses still exist
             for (int i = 0; i < myS.Length; i++) {
                string character = myS.Substring(i,1);
                if (character == "(") { //If you reach a left parenatheses then increment leftParen
                    leftParen++;
                } else if (character == ")") { //If you reach a right parantheses then decrement leftParen
                    leftParen--;
                    if (leftParen == 0) { //If we've reached the right parantheses that matches the left one
                        string end = "";
                        if (i == myS.Length-1) {
                                end = "";
                            } else {
                                end = myS.Substring(i+1);
                        }//End if-else for end string

                        return stringyEvaluate("" + myS.Substring(0,firstLeftParen)
                            + stringyEvaluate(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal).ToString()
                            + end ,xVal,yVal);
                            
                    }//End if for reaching the matched right parantheses
                }//End if-else for hitting a parentheses
             }//End for loop
        }//End if for searching for a left parantheses
       
        //At this point there are no more parantheses

        if (myS.IndexOf("+",0) == -1) { lastAdd = -1; }
        else { lastAdd = myS.Length-1-Reverse(myS).IndexOf("+",0); }//End if-else for lastAdd

        if (myS.IndexOf("-",0) == -1) { lastSubtract = -1; }
        else { lastSubtract = myS.Length-1-Reverse(myS).IndexOf("-",0); }//End if-else for lastSubtract
        if (lastSubtract == 0) {lastSubtract = -1; }
        if (lastSubtract > 0) {
            string checkMe = myS.Substring(lastSubtract-1,1);
            if (checkMe == "*" || checkMe == "/")  {
                lastSubtract = -1; //Cause then that's not really a subtract
            }//End if for if the prev charatcer was bad
        }//End if else for lastSubtract check that it's not actually a minus sign

        if (myS.IndexOf("*",0) == -1) { lastMultiply = -1; }
        else { lastMultiply = myS.Length-1-Reverse(myS).IndexOf("*",0); }//End if-else for lastSubtract

        if (myS.IndexOf("/",0) == -1) { lastDivision = -1; }
        else { lastDivision = myS.Length-1-Reverse(myS).IndexOf("/",0); }//End if-else for lastSubtract

        if (myS.IndexOf("^",0) == -1) { lastExponent = -1; }
        else { lastExponent = myS.Length-1-Reverse(myS).IndexOf("^",0); }//End if-else for lastSubtract

        if (myS.IndexOf("!",0) == -1) { lastFactorial = -1; }
        else { lastFactorial = myS.Length-1-Reverse(myS).IndexOf("!",0); }//End if-else for lastSubtract

        if (lastAdd + lastSubtract > -1) { //There's a + symbol or a - symbol in there
            if (lastSubtract == -1 || lastAdd > lastSubtract) { //There is an addition rightmost
                return stringyEvaluate(myS.Substring(0,lastAdd),xVal,yVal) + stringyEvaluate(myS.Substring(lastAdd+1),xVal,yVal);
            } else { //There is a subtraction symbol rightmost
                return stringyEvaluate(myS.Substring(0,lastSubtract),xVal,yVal) - stringyEvaluate(myS.Substring(lastSubtract+1),xVal,yVal);
            }//End add/sub if-else
        } else { //No more +'s or -'s!
            if (lastMultiply + lastDivision > -1) {
                if (lastDivision == -1 || lastMultiply > lastDivision) { //There is a multiplication rightmost
                    return stringyEvaluate(myS.Substring(0,lastMultiply),xVal,yVal) * stringyEvaluate(myS.Substring(lastMultiply+1),xVal,yVal);
                } else { //There is a division symbol rightmost
                    return stringyEvaluate(myS.Substring(0,lastDivision),xVal,yVal) / stringyEvaluate(myS.Substring(lastDivision+1),xVal,yVal);
            }//End add/sub if-else
            } else { //No more *'s or /'s!
                if (lastExponent > -1) { //There's an exponent
                    return Mathf.Pow(stringyEvaluate(myS.Substring(0,lastExponent),xVal,yVal),stringyEvaluate(myS.Substring(lastExponent+1),xVal,yVal));
                } else { //No more ^'s!
                    if (lastFactorial > -1) {
                        return (float)factorial((int)stringyEvaluate(myS.Substring(0,myS.Length-1),xVal,yVal));
                    } else { //No more !'s!
                        myS = myS.Replace("x",xVal.ToString()).Replace("y",yVal.ToString()).Replace("π",3.14159265358979323f.ToString()).Replace("e",.5772156649.ToString()).Replace("G",.0000000006673f.ToString()).Replace("ke",8987551787.368f.ToString());
                        float finalVal = (float)System.Convert.ToDouble(myS);
                        return finalVal;
                    }//End ! if-else 
                }//End ^ if-else
            }//End mult/div if-else
        }//End if-else

    }//End stringyEvaluate

    public static string Reverse( string s )
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }//End Reverse function

    public float round (float num)
    {
        float num1 = num * 100000;
        num1 = Mathf.Round(num1);
        num1 /= 100000;
        return num1;
    }

}
