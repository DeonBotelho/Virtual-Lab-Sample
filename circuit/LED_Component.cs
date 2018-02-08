using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED_Component : DC_Component
{

    private static int compNum = 1;
    private int id = compNum++;

    [SerializeField]
    private double resistance;// = 0.013; //internal resistance of 13ohmns
    private double currentReq = 20; //20ma


    private double current = 0;



    protected double voltageDrop = 0.7; //voltage drop across LED_Component ~0.7V
    protected double voltageReq = 0;
    protected Material glassMaterial;
    [SerializeField]
    private GameObject ledModel;
    [SerializeField]
    private GameObject[] LightBulb;
    [SerializeField]
    private Material defaultGlass;
    [SerializeField]
    private Material burntGlass;
    [SerializeField]                           
    private Material[] glass;                // R     Y      O      B     G      V      W  
    protected static double[] resistances =  { 12.1, 16.3, 15.65, 19.81, 14.5, 22.55, 27.12};//ohms @15v
    protected static double[] voltageDrops = { 1.63, 2.10, 2.030, 2.480, 1.90, 2.760, 3.200};
    protected static double[] voltagesReq =  { 2.00, 2.00, 2.000, 3.500, 3.70, 4.500, 3.500};
    protected static double[] currentsReq =  { 30.0, 25.0, 26.00, 20.00, 23.0, 18.00, 15.00}; //mA
    protected int color;

    //LED VoltageDrops 
    /*http://www.talkingelectronics.com/projects/30%20LED%20Projects/30%20LED%20Projects.html  
        http://www.talkingelectronics.com/projects/30%20LED%20Projects/images/LED-Colour.gif*/





    private void Update()
    {

        Node nodeHigh = nodeL;
        Node nodeLow = nodeR;
        /*              
         *     Probably useful information even if not used at the current moment         
         *     
         *     <<<<<<<<<<<<<<<<<<<<<<<<<check in series to if oriented right>>>>>>>>>>>>>>>>>>>>>
         *     
        if ((this.transform.rotation.y) == 0)  //+down -up
        {
        
        }
        else if ((this.transform.rotation.y) > 0 && (this.transform.rotation.y) < 90) //+LeftDown -RightUp
        {

        }
        else if ((this.transform.rotation.y) == 90) //+left - right
        {
            
        }
        else if ((this.transform.rotation.y) > 90 && (this.transform.rotation.y) < 180) //+LeftUP -RightDown
        {

        }
        else if ((this.transform.rotation.y) == 180) //+up -down
        {
            
        }
        else if ((this.transform.rotation.y) > 180 && (this.transform.rotation.y) < 270) //+RightUp -LeftDown
        {

        }
        else if ((this.transform.rotation.y) == -90) //+right -left
        {

        }
        else if ((this.transform.rotation.y) < 0 && (this.transform.rotation.y) > -90) //+RightDown -Leftup
        {

        }*/


        // if ((nodeHigh.getVoltage() - nodeLow.getVoltage()) > voltageReq && CircuitStation.power == true)


        //Debug.Log(internalCurrent + " <> " + currentReq);
        if (internalCurrent > currentReq && CircuitStation.power == true)
        {
         
         //   Debug.Log(positiveNode.getVoltage() + " <**> " + voltageReq);
            if (positiveNode.getVoltage() >= voltageReq)
            {
         
                ledOn(true);
            }
        }
        else
        {
            ledOn(false);
        }
    }

    public LED_Component() : base()
    {
        glassMaterial = defaultGlass;
        internalResistance = 13;//13ohms
        voltageDrop = 1.5;
        voltageReq = 2;
        currentReq = 20;
        

    }
    public override void setColor(string color)
    { 
                
        if (color.ToUpper().Equals("RED")) { setLed(RED); }
        else if (color.ToUpper().Equals("YELLOW")) { setLed(YELLOW); }
        else if (color.ToUpper().Equals("ORANGE")) { setLed(ORANGE); }
        else if (color.ToUpper().Equals("BLUE")) { setLed(BLUE); }
        else if (color.ToUpper().Equals("GREEN")) { setLed(GREEN); }
        else if (color.ToUpper().Equals("VIOLET")) { setLed(VIOLET); }
        else if (color.ToUpper().Equals("WHITE")) { setLed(WHITE); }
        else//Defaultt
        {
            System.Random r = new System.Random();
            setLed(r.Next(0, 6));
        }


    }
    private void setLed(int led_Color)
    {
        color = led_Color;   
        voltageDrop =addVariance(voltageDrops[led_Color]);
        voltageReq = addVariance(voltagesReq[led_Color]);
        currentReq = addVariance(currentsReq[led_Color])/1000;
        glassMaterial = glass[led_Color];
        internalResistance = addVariance(resistances[led_Color]);
    }

    public void ledOn(bool on)
    {
        // Debug.Log(internalCurrent + "  " + currentReq);
        if (internalCurrent > (currentReq * 10) && CircuitStation.power == true)
        {
            glassMaterial = burntGlass;
        }
        else
        {
            glassMaterial = glass[color];
        }

            if (on)
        {
            //glassmaterial
            foreach (GameObject x in LightBulb)
            {
                x.GetComponent<Renderer>().material = glassMaterial;
            }
        }
        else
        {
            foreach (GameObject x in LightBulb)
            {
                x.GetComponent<Renderer>().material = defaultGlass;
            }

        }
    }

    public double getVoltageDrop()
    {
        return voltageDrop;
    }

    public void updateResistance(double voltage)
    {
        internalResistance = addVariance(resistances[color]);

    }


    public override double getVoltageDrop(double incomingV, double incomingC)
    {
        //double x = (int) ( (incomingV - voltageDrop) * 100);

        double r = internalResistance * 15 / incomingV;
        double x = (int)((incomingV - (r*incomingC)) * 100);//+voltagedrop /2
        x = x / 100;
       
        return x;       
    }

    public override int componentNum()
    {
        return id;
    }
}


  