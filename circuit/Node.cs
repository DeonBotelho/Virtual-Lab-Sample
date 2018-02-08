using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: MonoBehaviour {

    static int nodeCount = 0;
    private int nodeId;
    private double voltage;
    

    public Node()//Signular representaion of one side of an electrical component
    {        
        voltage = 0;        
        nodeId = nodeCount++;
    }

    public int getNodeID()
    {
        return nodeId;
    }

    public void setVoltage(double v)
    {
        v = (int)(v * 100);
        v = v / 100;
       
        voltage = v;
    }
    

    public double getVoltage()
    {
        return voltage;
    }

    public override string ToString()
    {
        return nodeId + "";
    }
}
