using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DC_Component : MonoBehaviour {



    protected Node nodeL, nodeR; //left and right node
    protected static Node supply, ground;
    protected Node positiveNode;
    protected double internalResistance;
    protected double internalCurrent;
    private int[] row, col;

    protected int RED = 0;
    protected int YELLOW = 1;
    protected int ORANGE = 2;
    protected int BLUE = 3;
    protected int GREEN = 4;
    protected int VIOLET = 5;
    protected int WHITE = 6;
    protected int BLACK = 7;



    public DC_Component()//Default constucter for unknown location of components 
    {
        internalResistance = 0.00001;
        RED = 0;
        YELLOW = 1;
        ORANGE = 2;
        BLUE = 3;
        GREEN = 4;
        VIOLET = 5;
        WHITE = 6;
        BLACK = 7;
    }

    protected DC_Component(double internalResistance, Node nodeL, Node nodeR) //Initialize left and right of a dc commonent
    {
        this.internalResistance = internalResistance;
        this.nodeL = nodeL;
        this.nodeR = nodeR;
    }

    public static void intializeSupplyGround(Node s, Node g)
    {
        supply = s;
        ground = g;
    }

    //Have left node change if component is moved
    public void changeNodeL(Node newNode)
    {
        nodeL = newNode;
    }
    //Have right node change if component is moved
    public void changeNodeR(Node newNode)
    {
        nodeR = newNode;        
    }
    public void setPositiveNode(Node positive)
    {
        positiveNode = positive;
    }

    public void swapNodes()
    {
        Node temp = nodeL;
        nodeL = nodeR;
        nodeR = temp;

        int TMP = row[0];
        row[0] = row[1];
        row[1] = TMP;
        TMP = col[0];
        col[0] = col[1];
        col[1] = TMP;

    }
    public void swapLocation()
    {
        int TMP = row[0];
        row[0] = row[1];
        row[1] = TMP;
        TMP = col[0];
        col[0] = col[1];
        col[1] = TMP;

    }
    public abstract int componentNum();

    //return left node
    public Node getNodeL()
    {
        return nodeL;
    }
    //return right node
    public Node getNodeR()
    {
        return nodeR;
    }

    public void setLocation(int[] row, int[] col)
    {
        this.row = new int[row.Length];
        this.col = new int[col.Length];
        for (int i = 0; i < row.Length; i++)
        {
            this.row[i] = row[i];
        }
        for (int i = 0; i < col.Length; i++)
        {
            this.col[i] = col[i];
        }
    }


    public int[] getRow()
    {
        return row;
    }
    public int[] getCol()
    {
        return col;
    }


    //get internalresistance
    public double getInternalResistance()
    {
        return internalResistance;
    }

    public double getInternalCurrent()
    {
        internalCurrent =(int)(((nodeL.getVoltage() - nodeR.getVoltage()) / internalResistance)*100);

        internalCurrent =  internalCurrent / 100;
        return internalCurrent;

    }

    public abstract double getVoltageDrop(double incomingV, double incomingC);
    
    public abstract void setColor(string color);

    public void turnComponentOff()
    {        
        internalCurrent = 0;
    }
    public void turnComponentOn()
    {     
        internalCurrent = getInternalCurrent();
    }


    protected double addVariance(double value)
    {
        System.Random r = new System.Random();//RANDOM VARIANCE IN COMPONENTS
        value = value + r.Next(0, (int)value / 10);
        return value;
    }

    public bool orientedRight()
    {
        if (nodeL.getNodeID() == positiveNode.getNodeID())
        {
            return true;
        }
      

        return false ;
    }



    public override string ToString()
    {
        return "DC_Component.ToString()";//(GetType().ToString()+":"+ internalResistance+"Ohms" + internalCurrent+"A" + "\n\t\t >> leftNode:".PadRight(2) + getNodeL().ToString().PadRight(3) + " , rightNode:".PadRight(2) + getNodeR().ToString().PadRight(3));
    }

}
