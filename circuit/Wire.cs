using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : DC_Component
{

    private static int compNum = 1;
    private int id = compNum++;
    [SerializeField]
    private double voltageDrop = 0; //voltage drop across Wire  assumed to be 0
    [SerializeField]
    GameObject cyl;
    [SerializeField]
    Material[] colors;


    public Wire() : base()
    {
        internalResistance = 0.00000001;
    }
    //Wire should have no internal resistance ideally
    public Wire(Node left, Node right) : base(0.0, left, right)
    {
    }

    public override void setColor(string color)
    {
        int i = GREEN;
        if (color.ToUpper().Equals("RED")) { i = (RED); }
        else if (color.ToUpper().Equals("YELLOW")) { i=  (YELLOW); }
        else if (color.ToUpper().Equals("ORANGE")) { i=  (ORANGE); }
        else if (color.ToUpper().Equals("BLUE")) { i=  (BLUE); }
        else if (color.ToUpper().Equals("GREEN")) { i=  (GREEN); }
        else if (color.ToUpper().Equals("VIOLET")) { i=  (VIOLET); }
        else if (color.ToUpper().Equals("WHITE"))  { i= (WHITE); }
        else if (color.ToUpper().Equals("BLACK")) { i = (BLACK); }
        else//Defaultt
        {
            System.Random r = new System.Random();
            i = r.Next(0, 7);
        }


        cyl.GetComponent<Renderer>().material = colors[i];
    }

    
    public override double getVoltageDrop(double incomingV, double incomingC)
    {
        if (getNodeR().getVoltage() == 0)
        {
            return 0;
        }
        return incomingV;
    }

    public override int componentNum()
    {
        return id;
    }
}
