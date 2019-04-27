using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class calculus3D : MonoBehaviour
{
    public string[] constantFlow = new string[3];
    public List<string> constantDiagonalFlow = new List<string>();
    public List<string> cubeRepulsion = new List<string>();
    public List<string> cubeAttraction = new List<string>();
    public List<string> inverseRepulsion = new List<string>();
    public List<string> negativeCharge = new List<string>();
    public List<string> positiveCharge = new List<string>();
    public List<string> cyclone = new List<string>();
    public List<string> tornado = new List<string>();
    public List<string> attractiveShell = new List<string>();
    public List<string> repulsiveShell = new List<string>();
    public List<string> sourceAndSink = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(stringyEvaluate3D("x*-y^z",1f,2f,3f));

        string[] constantFlow = new string[]
        {
            "1",
            "0",
            "0"
        };

        List<string> constantDiagonalFlow = new List<string>();
            constantDiagonalFlow.Add("1/Sqrt(3)"); constantDiagonalFlow.Add("1/Sqrt(3)"); constantDiagonalFlow.Add("1/Sqrt(3)");
        List<string> cubeRepulsion = new List<string>();
            cubeRepulsion.Add("x"); cubeRepulsion.Add("y"); cubeRepulsion.Add("z");
        List<string> cubeAttraction = new List<string>();
            cubeAttraction.Add("-x"); cubeAttraction.Add("-y"); cubeAttraction.Add("-z");
        List<string> inverseRepulsion = new List<string>();
            inverseRepulsion.Add("1/x"); inverseRepulsion.Add("1/y"); inverseRepulsion.Add("1/z");
        List<string> inverseAttraction = new List<string>();
            inverseAttraction.Add("-1/x"); inverseAttraction.Add("-1/y"); inverseAttraction.Add("-1/z");
        List<string> negativeCharge = new List<string>();
            negativeCharge.Add("x/(x^2+y^2+z^2)"); negativeCharge.Add("y/(x^2+y^2+z^2)"); negativeCharge.Add("z/(x^2+y^2+z^2)");
        List<string> positiveCharge = new List<string>();
            positiveCharge.Add("-x/(x^2+y^2+z^2)"); positiveCharge.Add("-y/(x^2+y^2+z^2)"); positiveCharge.Add("-z/(x^2+y^2+z^2)");
        List<string> cyclone = new List<string>();
            cyclone.Add("-y"); cyclone.Add("x"); cyclone.Add("0");
        List<string> tornado = new List<string>();
            tornado.Add("z*-y"); tornado.Add("z*x"); tornado.Add("0");
        List<string> attractiveShell = new List<string>();
            attractiveShell.Add("x/(2-Sqrt(Sqrt(x^2+y^2+z^2)))"); attractiveShell.Add("y/(2-Sqrt(Sqrt(x^2+y^2+z^2)))"); attractiveShell.Add("z/(2-Sqrt(Sqrt(x^2+y^2+z^2)))");
        List<string> repulsiveShell = new List<string>();
            repulsiveShell.Add("-x/(2-Sqrt(Sqrt(x^2+y^2+z^2)))"); repulsiveShell.Add("-y/(2-Sqrt(Sqrt(x^2+y^2+z^2)))"); repulsiveShell.Add("-z/(2-Sqrt(Sqrt(x^2+y^2+z^2)))");
        List<string> sourceAndSink = new List<string>();
            sourceAndSink.Add("6/Sqrt((x-6)^2+(y-6)^2+(z-3)^2)-(-2)/Sqrt((x+2)^2+(y+2)^2+(z+1)^2)");
            sourceAndSink.Add("6/Sqrt((x-6)^2+(y-6)^2+(z-3)^2)-(-2)/Sqrt((x+2)^2+(y+2)^2+(z+1)^2)");
            sourceAndSink.Add("3/Sqrt((x-6)^2+(y-6)^2+(z-3)^2)-(-1)/Sqrt((x+2)^2+(y+2)^2+(z+1)^2)");


    }//End Start

    // Update is called once per frame
    void Update()
    {
        
    }

    int factorial(int s) {
        int acc = 1;
        for (int i = s; i > 0; i--) {
            acc *= i;
        }//End for loop
        return acc;
    }//End factorial function

    Vector3 stringyNumericGradient3D(string s, float xVal, float yVal, float zVal) {
        float h = .001f;
        float xDeriv = (stringyEvaluate3D(s,xVal+h,yVal,zVal)-stringyEvaluate3D(s,xVal,yVal,zVal))/h;
        float yDeriv = (stringyEvaluate3D(s,xVal,yVal+h,zVal)-stringyEvaluate3D(s,xVal,yVal,zVal))/h;
        float zDeriv = (stringyEvaluate3D(s,xVal,yVal,zVal+h)-stringyEvaluate3D(s,xVal,yVal,zVal))/h;
        return new Vector3(xDeriv,yDeriv,zDeriv);
    }//End stringyNumericGradient


    float stringyEvaluate3D(string s, float xVal, float yVal, float zVal) {
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstSin)
                                + Mathf.Sin(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal)).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstCos)
                                + Mathf.Cos(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal)).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstTan)
                                + Mathf.Tan(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal)).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstCot)
                                + (1/Mathf.Tan(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal))).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstSec)
                                + (1/Mathf.Cos(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal))).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstCsc)
                                + (1/Mathf.Sin(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal))).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstLog)
                                + Mathf.Log(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal),Mathf.Exp(1)).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstExp)
                                + Mathf.Exp(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal)).ToString()
                                + end ,xVal,yVal,zVal);
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
                            return stringyEvaluate3D("" + myS.Substring(0,firstSqrt)
                                + Mathf.Sqrt(stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal)).ToString()
                                + end ,xVal,yVal,zVal);
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

                        return stringyEvaluate3D("" + myS.Substring(0,firstLeftParen)
                            + stringyEvaluate3D(myS.Substring(firstLeftParen+1,i-firstLeftParen-1),xVal,yVal,zVal).ToString()
                            + end ,xVal,yVal,zVal);
                            
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
                return stringyEvaluate3D(myS.Substring(0,lastAdd),xVal,yVal,zVal) + stringyEvaluate3D(myS.Substring(lastAdd+1),xVal,yVal,zVal);
            } else { //There is a subtraction symbol rightmost
                return stringyEvaluate3D(myS.Substring(0,lastSubtract),xVal,yVal,zVal) - stringyEvaluate3D(myS.Substring(lastSubtract+1),xVal,yVal,zVal);
            }//End add/sub if-else
        } else { //No more +'s or -'s!
            if (lastMultiply + lastDivision > -1) {
                if (lastDivision == -1 || lastMultiply > lastDivision) { //There is a multiplication rightmost
                    return stringyEvaluate3D(myS.Substring(0,lastMultiply),xVal,yVal,zVal) * stringyEvaluate3D(myS.Substring(lastMultiply+1),xVal,yVal,zVal);
                } else { //There is a division symbol rightmost
                    return stringyEvaluate3D(myS.Substring(0,lastDivision),xVal,yVal,zVal) / stringyEvaluate3D(myS.Substring(lastDivision+1),xVal,yVal,zVal);
            }//End add/sub if-else
            } else { //No more *'s or /'s!
                if (lastExponent > -1) { //There's an exponent
                    return Mathf.Pow(stringyEvaluate3D(myS.Substring(0,lastExponent),xVal,yVal,zVal),stringyEvaluate3D(myS.Substring(lastExponent+1),xVal,yVal,zVal));
                } else { //No more ^'s!
                    if (lastFactorial > -1) {
                        return (float)factorial((int)stringyEvaluate3D(myS.Substring(0,myS.Length-1),xVal,yVal,zVal));
                    } else { //No more !'s!
                        myS = myS.Replace("x",xVal.ToString()).Replace("y",yVal.ToString()).Replace("z",zVal.ToString()).Replace("π",3.14159265358979323f.ToString()).Replace("e",.5772156649.ToString()).Replace("G",.0000000006673f.ToString()).Replace("ke",8987551787.368f.ToString());
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

    //You feed this function a vector field in the form of a list, that is, one of the ones above like cyclone
    //It will then return to you a list of 1000 lists, each list containing 2 Vector3's
    //The first Vector3 is the position to draw the arrow
    //The second Vector3 is the (unnormalized) arrow, so where it should point, originating from that point of course
    //The float spacer is the space between points, you may need to adjust this to work with Unity
    //The bounds for the for loops are the domain, you may need to adjust this accordingly
    public List<List<Vector3>> rectTestPoints(string[] vectorField) {
        float spacer = 1; //This is how far apart the points are on the lattice
        List<List<Vector3>> positionDirectionPairs = new List<List<Vector3>>();
        for (int i = -5; i <= 5; i++) { //You may need to change this domain
            for (int j = -5; j <= 5; j++) { //You may need to change this domain
                for (int k = -5; k <= 5; k++) { //You may need to change this domain
                    Vector3 position = new Vector3(spacer*i,spacer*j,spacer*k);
                    float Fx = stringyEvaluate3D("z*(0-y)", i*spacer,j*spacer,k*spacer);
                    float Fy = stringyEvaluate3D("z*x", i*spacer,j*spacer,k*spacer);
                    float Fz = stringyEvaluate3D("0", i*spacer,j*spacer,k*spacer);
                    Vector3 direction = new Vector3(Fx,Fy,Fz);
                    List<Vector3> positionAndDirection = new List<Vector3>();
                    positionAndDirection.Add(position); positionAndDirection.Add(direction);
                    positionDirectionPairs.Add(positionAndDirection);
                }//End z for loop
            }//End y for loop
        }//End x for loop
        return positionDirectionPairs;
     }//End rectTestPoints function

}//End calculus3D class
