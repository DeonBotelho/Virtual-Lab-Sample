using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resistor: DC_Component {

    private static int compNum = 1;
    private int id = compNum++;
    public Resistor()
    {
        internalResistance = 100;//100ohms
        
    }
    

    public override double getVoltageDrop(double incomingV, double incomingC)
    {
        double x = (int)((incomingV - incomingC * internalResistance) * 100);
        x = x / 100;

        return x;       
    }
 
    public override void setColor(string color)
    {
        double r = 100;
        try
        {
            r = double.Parse(color);
        }
        catch (Exception e) { }
        setResistace(r);
        
    }

    public void setResistace(double resistance)
    {
        if (resistance > 0)
        {
            this.internalResistance = resistance;
        }
        else
        {
            this.internalResistance = 100; //default 100 OHM
        }
        addVariance(internalResistance);
    }
    public override int componentNum()
    {
        return id;
    }


}
