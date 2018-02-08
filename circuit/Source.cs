using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : DC_Component
{
    [SerializeField]
    private double voltage;

    //Default internal resistance of power source ~50ohms but ideally is 0
    public Source(double voltage, Node left, Node right) : base(0.0,left, right)
    {
        this.voltage = voltage;
    }

    public void changeVoltage(double newVoltage)
    {
        this.voltage = newVoltage;
    }

    public override int componentNum()
    {
        throw new NotImplementedException();
    }

    public override double getVoltageDrop(double incomingV, double incomingC)
    {
        throw new NotImplementedException();
    }

    public override void setColor(string color)
    {
        throw new NotImplementedException();
    }
}
