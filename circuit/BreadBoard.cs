using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadBoard : MonoBehaviour
{
    /*
     * Visual representation of breadboard
     
       //Nodes on Bread board
            A      B         C     D     E     F     G         H     I     J     K      L        M     N  
            
            ^      ^                                                                             ^      ^
       1    L      L        <     Node:[0][0]        >          <     Node:[1][0]        >       R      R
       2    E      E        <     Node:[0][1]        >          <     Node:[1][1]        >       I      I
       3    F      F        <     Node:[0][2]        >          <     Node:[1][2]        >       G      G
       4    T      F        <     Node:[0][3]        >          <     Node:[1][3]        >       H      H 
       5    P      N        <     Node:[0][4]        >          <     Node:[1][4]        >       T      T
       6    O      E        <     Node:[0][5]        >          <     Node:[1][5]        >       P      N
       7    S      G        <     Node:[0][6]        >          <     Node:[1][6]        >       O      E
       8    .      .        <     Node:[0][7]        >          <     Node:[1][7]        >       S      G
       9    .      .        <     Node:[0][8]        >          <     Node:[1][8]        >       .      .
      10    .      .        <     Node:[0][9]        >          <     Node:[1][9]        >       .      .
            v      v                                                                             v      v
       
        //Spaces :occupied boolean array

             A      B         C     D     E     F     G         H     I     J     K      L        M     N
            
       1   [0,0]  [0,1]     [0,2] [0,3] [0,4] [0,5] [0,6]   [0,7] [0,8] [0,9] [0,10] [0,11]     [0,12] [0,13] 
       2   [1,0]  [1,1]     [1,2] [1,3] [1,4] [1,5] [1,6]   [1,7] [1,8] [1,9] [1,10] [1,11]     [1,12] [1,13] 
       3   [2,0]  [2,1]     [2,2] [2,3] [2,4] [2,5] [2,6]   [2,7] [2,8] [2,9] [2,10] [2,11]     [2,12] [2,13] 
       4   [3,0]  [3,1]     [3,2] [3,3] [3,4] [3,5] [3,6]   [3,7] [3,8] [3,9] [3,10] [3,11]     [3,12] [3,13] 
       5   [4,0]  [4,1]     [4,2] [4,3] [4,4] [4,5] [4,6]   [4,7] [4,8] [4,9] [4,10] [4,11]     [4,12] [4,13] 
       6   [5,0]  [5,1]     [5,2] [5,3] [5,4] [5,5] [5,6]   [5,7] [5,8] [5,9] [5,10] [5,11]     [5,12] [5,13] 
       7   [6,0]  [6,1]     [6,2] [6,3] [6,4] [6,5] [6,6]   [6,7] [6,8] [6,9] [6,10] [6,11]     [6,12] [6,13] 
       8   [7,0]  [7,1]     [7,2] [7,3] [7,4] [7,5] [7,6]   [7,7] [7,8] [7,9] [7,10] [7,11]     [7,12] [7,13] 
       9   [8,0]  [8,1]     [8,2] [8,3] [8,4] [8,5] [8,6]   [8,7] [8,8] [8,9] [8,10] [8,11]     [8,12] [8,13] 
      10   [9,0]  [9,1]     [9,2] [9,3] [9,4] [9,5] [9,6]   [9,7] [9,8] [9,9] [9,10] [9,11]     [9,12] [9,13] 
            
         */

    public  static List<DC_Component> testList = new List<DC_Component>();
    private Node leftPos, leftNeg, rightPos, rightNeg, ground, supply;//Node columns A,B,M,N in that order
    private int rows = 10, cols = 14;//for current breadboard model 10x14
    private Node[,] nodeArray;
    private bool[,] occupied;
    [SerializeField]
    private GameObject sourceWire, groundWire;
    private GameObject sourceVoltage;

    //Array list of commponents on bread board currently
    List<DC_Component> components;
    List<List<List<DC_Component>>> bb;


    public BreadBoard(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        nodeArray = new Node[rows, cols];
        occupied = new bool[rows, cols];
        sourceVoltage = GameObject.Find("SourceBox_VoltageText");

        components = new List<DC_Component>();
        initialize();  //set default board values
    }
    private void initialize()
    {
        //Set all positions to "free"
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                occupied[i, j] = new bool();
                occupied[i, j] = false; //10 rows x 14 (A-N) columns  ,  spaces that can be filled
            }
        }
        //initialize all nodes
        //Nodes  0 -1
        leftPos = new Node();// column A Is one node
        leftNeg = new Node();// column B Is one node
        //Nodes 2 - 21
        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < rows; k++)
            {
                nodeArray[k, i] = new Node();// 10 x 2 , 10 rows by [(C,D,E,F,G) , (H,I,J,K,L) ]
            }
        }
        //Nodes 22-23
        rightPos = new Node();// column M is one node
        rightNeg = new Node();// columnN is one node
        DC_Component.intializeSupplyGround(supply, ground);
    }

    //--If node was free it is put in use, else if it was in use it is now free  
    public bool freeNode(int row, int col)
    {
        occupied[row, col] = !occupied[row, col];
        //Debug.Log("OCCUPIUED: " + row + "," + col + ":" + occupied[row, col]);
        return occupied[row, col];
    }

    public void makeSupply(Node n)
    {
        supply = n;
    }

    public void makeGround(Node n)
    {
        ground = n;
    }

    public Node getNode(int row, int col)
    {
        if (col == 0) { return leftPos; } //A
        else if (col == 1) { return leftNeg; }//B
        else if (col == 12) { return rightPos; }//M
        else if (col == 13) { return rightNeg; }//N
        else if (col > 1 && col < 7) { return nodeArray[row, 0]; }//2-6 is col 1 of nodes
        else if (col > 6 && col < 12) { return nodeArray[row, 1]; }//7 - 11 is col 2 of nodes
        return null;
    }


    public void placeComponent(GameObject component, String name, int[] row, int[] col, int LED_)
    {
        Node n1 = getNode(row[0], col[0]);
        Node n2 = getNode(row[1], col[1]);
        Node positive = getNode(row[1], col[1]); //Postive side of LED, Adding more components make sure to follow convention

        //LED_ usless at this point? 
        //Debug.Log("Positive Node : " + positive + "Node 1: " + n1 + " Node 2: " + n2 + "LED_: " + LED_);

        if (n1.getNodeID() > n2.getNodeID())//&&  LED_ < 0 )//&& !name.Equals("LED"))
        {
            n2 = getNode(row[0], col[0]);
            n1 = getNode(row[1], col[1]);            
        }
        
        DC_Component c = null;
        if (name.Equals("Wire")) { c = component.GetComponent<Wire>(); }
        else if (name.Equals("Resistor")) { c = component.GetComponent<Resistor>(); }
        else if (name.Equals("LED")) { c = component.GetComponent<LED_Component>(); }
        c.changeNodeL(n1);
        c.changeNodeR(n2);
        c.setPositiveNode(positive);
        components.Add(c); //will be sorted later...eventually
        c.setLocation(row, col);//component to remember grid values incase it is deleted
        if (n1.getNodeID() > n2.getNodeID())
        {
            c.swapLocation();
        }
    }

    public void removeComponent(DC_Component c)
    {
        freeNode(c.getRow()[0], c.getCol()[0]);//Free node space on bread board and delete object
        freeNode(c.getRow()[1], c.getCol()[1]);
        components.Remove(c);
    }

    private void rightSort(List<DC_Component> components)
    {
        List<DC_Component> rightList = new List<DC_Component>();
        int MaxId = rightNeg.getNodeID();//ground
        // for (int counter = MaxId; counter >= 0; counter--)

        for (int counter = 0; counter <= MaxId; counter++)
        {
            foreach (DC_Component c in components)
            {
                if (c.getNodeR().getNodeID() == counter)
                {
                    rightList.Add(c); //all right nodes ordered from 23 downTo 0
                }
            }
            foreach (DC_Component dc in rightList)
            {
                components.Remove(dc);
            }
        }
        foreach (DC_Component dc in rightList)
        {
            components.Add(dc);
            //Debug.Log("R>" + dc);
        }
    }

    public void leftSort(List<DC_Component> components)
    {
        List<DC_Component> leftList = new List<DC_Component>();
        int MaxId = rightNeg.getNodeID();//ground
        for (int counter = 0; counter <= MaxId; counter++)
        {
            foreach (DC_Component c in components)
            {
                if (c.getNodeL().getNodeID() == counter)
                {
                    leftList.Add(c); //all left nodes ordered from 0 - 23
                }
            }
            foreach (DC_Component dc in leftList)
            {
                components.Remove(dc);
            }
        }
        foreach (DC_Component dc in leftList)
        {
            components.Add(dc); //all lists in numerical order         
                                // Debug.Log("L>" + dc);
        }
    }

    private void checkSort(List<DC_Component> components, List<DC_Component> tmpList)
    {
        bool belongs = false;
        foreach (DC_Component compare in components)//check that at least one of the nodes connect to the main circuit
        {
            belongs = false;
            foreach (DC_Component comparedWith in components)
            {
                if (compare.getRow() != comparedWith.getRow() && compare.getCol() != comparedWith.getCol())
                {
                    if (compare.getNodeL().getNodeID() == comparedWith.getNodeL().getNodeID())
                    {
                        belongs = true;
                    }
                    else if (compare.getNodeL().getNodeID() == comparedWith.getNodeR().getNodeID())
                    {
                        belongs = true;
                    }
                    else if (compare.getNodeR().getNodeID() == comparedWith.getNodeL().getNodeID())
                    {
                        belongs = true;
                    }
                    else if (compare.getNodeR().getNodeID() == comparedWith.getNodeR().getNodeID())
                    {
                        belongs = true;
                    }
                    if (belongs)
                    {
                        break;
                    }
                }
            }
            if (!belongs)
            {
                Debug.Log("DOES NOT BELONG . PLACED IN TMP >>" + compare);
                tmpList.Add(compare);
            }
        }
        foreach (DC_Component c in components)//trivial components
        {
            if (c.getNodeL().getNodeID() == c.getNodeR().getNodeID())
            {
                tmpList.Add(c);
            }
        }


        foreach (DC_Component c in tmpList)
        {
            components.Remove(c);
        }
        
    }

    private void chainSort(List<DC_Component> components, List<DC_Component> tmp)
    {
        List<DC_Component> newList = new List<DC_Component>();
        testList = new List<DC_Component>();//***************************************************************************************************************************ManzMan
        bool loopCondition = true;
        int preIndex = 0, i = components.Count;// (int)Math.Pow(components.Count, components.Count + components.Count); //stop infinate loop
        int offSet = 1;

        while (loopCondition)
        {
            
            foreach (DC_Component c in components)
            {
                //Debug.Log(preIndex);
                try
                {
                    if (newList.Count == 0 || c.getNodeL().getNodeID() == 0)
                    {
                        newList.Add(c);//Add the first component into the list                                
                    }
                    //If left matches the prev right, add it right behind
                    else if (c.getNodeL().getNodeID() == newList[preIndex].getNodeR().getNodeID())
                    {
                        newList.Insert(preIndex + offSet, c);
                        testList.Add(c);
                        offSet++;
                    }
                    //If right matches the prev right, add it right behind after flip
                    else if (c.getNodeR().getNodeID() == newList[preIndex].getNodeR().getNodeID() && c.getNodeR().getNodeID() != ground.getNodeID())
                    {
                        /*Node tmp = c.getNodeL();
                        c.changeNodeL(c.getNodeR());
                        c.changeNodeR(tmp);*/
                        c.swapNodes();
                        newList.Insert(preIndex + offSet, c);
                        testList.Add(c);
                        offSet++;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e + "\n " + preIndex + " " + c);
                    tmp.Add(c);
                }
            }
            foreach (DC_Component c in newList)
            {
                components.Remove(c);
            }
            foreach (DC_Component c in tmp)
            {
                components.Remove(c);
            }
            if (components.Count == 0 || i-- == 0)
            {
                loopCondition = false;
            }
            preIndex++;
            offSet = 1;
            
        }
        foreach (DC_Component c in newList)
        {
            components.Add(c);
            //Debug.Log("N>>"+c);
        }
        foreach (DC_Component c in testList)
        {
            Debug.Log(">>>>>>>>>>>>>>>>" + c.getNodeL() + " " + c.getNodeR());
        }
    }

    

    private void circuitSplit(List<List<DC_Component>> circuit)
    {
        List<DC_Component> series = null;
        try
        {
            foreach (DC_Component c in components)
            {
                //Each component has been sorted such that the series are together with
                //Any subparts under
                if (c.getNodeL().getNodeID() == supply.getNodeID())//source id = 0
                {
                    if (series != null)
                    {
                        circuit.Add(series);
                    }
                    series = new List<DC_Component>();
                }
                series.Add(c);
            }
            circuit.Add(series);
        }
        catch (Exception e)
        {
            Debug.Log("nothing connected to source");
        }
    }

    public void parallelSort(List<List<DC_Component>> circuit)
    {
        //split each series to find number of parrallels => put into individial series
        List<List<DC_Component>> cir = new List<List<DC_Component>>();
        List<DC_Component> par = new List<DC_Component>();
        int nextID = 0;

        foreach (List<DC_Component> series in circuit)
        {
            for (int y = 0; y < series.Count; y++)
            {
                par.Add(series[y]);
                if (y + 1 < series.Count)
                {
                    nextID = series[y + 1].getNodeL().getNodeID();
                }
                else
                {
                    nextID = series[y].getNodeR().getNodeID() - 1; //makesure last condition fails to add par to the cir
                }
                if (series[y].getNodeR().getNodeID() != nextID)//possible issue at last item if component shares same location for both nodes?
                {
                    cir.Add(par);
                    par = new List<DC_Component>();
                }
            }
        }
        //pray to god it works
        circuit.Clear();
        foreach (List<DC_Component> s in cir)
        {
            circuit.Add(s);
        }
    }

    public void fixSort(List<List<DC_Component>> circuit, List<DC_Component> temp)//?
    {
        DC_Component prev = null;
        DC_Component curr = null;

        foreach (List<DC_Component> list in circuit)
        {
            for (int i = (list.Count - 1); i > 0; i--)
            {
                prev = list[i - 1];
                curr = list[i];

                if (prev.getNodeL().getNodeID() == supply.getNodeID() || prev.getNodeR().getNodeID() == ground.getNodeID())
                {/* Do nothing*/ }
                else if (curr.getNodeL().getNodeID() != prev.getNodeR().getNodeID())
                {
                    if (curr.getNodeL().getNodeID() == prev.getNodeL().getNodeID())
                    {
                        Node tmp = prev.getNodeL();
                        prev.changeNodeL(prev.getNodeR());
                        prev.changeNodeR(tmp);
                        Debug.Log("SWITCH COMPLETE!"); //tbh shouldnt even have to happen
                    }
                }
            }
        }
    }

    private void circuitSplitter(List<List<List<DC_Component>>> b, List<List<DC_Component>> circuit)
    {
        List<List<DC_Component>> c = null;
        foreach (List<DC_Component> s in circuit) //for each series list in the circuit so far
        {
            if (s[0].getNodeL().getNodeID() == supply.getNodeID())//if it starts with 0 add new circuit
            {
                if (c != null)
                {
                    b.Add(c);
                }
                c = new List<List<DC_Component>>();
            }
            c.Add(s);//continue adding it to the circuit prev
        }
        b.Add(c);//add all circuits to the breadboard list >> circuits >> series/parrallels >> components
    }

    private void checkChain(List<List<List<DC_Component>>> b, List<DC_Component> tmp)
    {
        bool start;
        bool end;
        bool resort = false;
        List<List<List<DC_Component>>> x = new List<List<List<DC_Component>>>();
        foreach (List<List<DC_Component>> circuit in b)
        {
            start = false;
            end = false;

            if (circuit.Count == 1)
            {
                start = circuit[0][0].getNodeL().getNodeID() == supply.getNodeID();
                end = circuit[0][circuit[0].Count - 1].getNodeR().getNodeID() == ground.getNodeID();
                if (!start || !end)
                {
                    x.Add(circuit);
                    foreach (DC_Component c in circuit[0])
                    {
                        tmp.Add(c);
                    }
                }
            }
            else
            {
                //start = circuit[0][0].getNodeL().getNodeID() == supply.getNodeID();
                end = circuit[0][circuit[0].Count - 1].getNodeR().getNodeID() == ground.getNodeID();
                if (!end)
                {
                    tmp.Add(circuit[0][circuit[0].Count - 1]);
                    resort = true;                  
                }
            }
        }
        foreach (List<List<DC_Component>> y in x)
        {
            b.Remove(y);
        }

        foreach (DC_Component c in tmp)
        {
            components.Remove(c);
        }
        if (resort)
        {
    
            List<List<List<DC_Component>>> newbreadboard = sort(tmp);
            b.Clear();

            foreach (List<List<DC_Component>> newcircuit in newbreadboard)
            {
                b.Add(newcircuit);
            }

        }
    }
    
    private List<List<List<DC_Component>>> sort(List<DC_Component> temp)
    {
        bb = new List<List<List<DC_Component>>>();
        List<List<DC_Component>> circuit = new List<List<DC_Component>>();
        rightSort(components);     //left don't care, right in order                                                     1   [*]
        leftSort(components);      //order lists by left  >> node Right in order from prev function                      2   [*] 
        checkSort(components, temp);//remove components that are not attached to anything.                               3   [*]
        chainSort(components,temp);     //sort it chains of series                                                       4   [*]    
        circuitSplit(circuit);     //breaks series into lists of series                                                  5   [*]     
        parallelSort(circuit);     //tmp should hold odd balls? re add to circuit?                                       6   [*]
        //fixSort(circuit, temp);    //Fix series chains to start from the correct node -> end at groundID                   [x] not required (?) 
        circuitSplitter(bb, circuit);  //Final split into circuits >> series/parallels >> components                     7   [*]  
        checkChain(bb, temp); //remove incomplete chains that don't reach ground                                         8   [*]    

        return bb;
    }

    private bool nodeCondition(Node n)
    {
        return (n.getVoltage() == -1);
    }

    private void series(List<DC_Component> list, double voltage)//***********************************************************************************************
    {
        double rEQ = seriesResistance(list, 0); //115ohm
        double current = voltage / rEQ;    // 0.130A
        double preV = voltage;     //15V
        bool fullcircuit = true;
        DC_Component dummy = new Wire();
        //Are all LEDS in the right directions     [ * ]  
        foreach (DC_Component c in list)
        {
            if (c.GetType().Equals(typeof(LED_Component)))
            {
                fullcircuit = c.orientedRight();
                if (!fullcircuit)
                {
                    dummy = c;
                    c.turnComponentOff();
                    break;
                }
            }
        }
        if (fullcircuit)//If so calculate voltages
        {
            foreach (DC_Component c in list)
            {
                if (nodeCondition(c.getNodeR()))
                {
                    preV = c.getVoltageDrop(preV, current);
                    c.getNodeR().setVoltage(preV);
                    double x = c.getInternalCurrent();
                    Debug.Log(x);
                }
                else
                {
                    current = c.getInternalCurrent();
                }
            }
        }
        else 
        {
            preV = voltage;
            foreach (DC_Component c in list)
            {
                if (c.Equals(dummy))
                {
                    break;
                }
                if (nodeCondition(c.getNodeR()))
                {
                    //preV = c.getVoltageDrop(preV, current);
                    c.getNodeR().setVoltage(preV);
                    //double x = c.getInternalCurrent();
                    //Debug.Log(x);
                }
               
            }
            foreach (DC_Component c in list)
            {
                c.turnComponentOff();//current = 0
                if (nodeCondition(c.getNodeR()))
                {
                    c.getNodeR().setVoltage(0);
                }
                if (c.getNodeR().getNodeID() == dummy.getNodeL().getNodeID())
                {
                    c.getNodeR().setVoltage(c.getNodeL().getVoltage());
                }
            }
        }
    }
   

    private double par(List<List<DC_Component>> list)
    {
        double vD = 10000000000000000000;
        foreach (List<DC_Component> c in list)
        {
            double tmp = 0;
            if (tmp < vD)
            {
                vD = tmp;
            }
        }
        return vD;
    }




    private void calculateVoltages(List<List<List<DC_Component>>> maincircuit)
    {
        resetVoltages();
        adjustments(maincircuit);
        //List<double> circuitResistances = circuitResistance(maincircuit);
        //double totalResistance;
        double totalCurrent;
        double voltage = double.Parse(sourceVoltage.GetComponent<TextMesh>().text.Substring(8, 2)); //trycatch?

        supply.setVoltage(voltage);//done already?
        ground.setVoltage(0);



        foreach (List<List<DC_Component>> circuit in maincircuit)
        {
            //totalResistance = circuitResistances[maincircuit.IndexOf(circuit)];//TotalEqResistance for the circuit
            //totalCurrent = voltage / totalResistance;
            double nodeV = voltage;
            if (circuit.Count == 1)//if only 1 series ie no parrallels
            {

                series(circuit[0], voltage);         
            }
            else
            {
                double r = parallelRes(circuit);

                totalCurrent = voltage / r;
                foreach (List<DC_Component> series in circuit)
                {
                    double seriesNodeV = nodeV;
                    totalCurrent = nodeV / seriesResistance(series,0);
                    foreach (DC_Component c in series)
                    {
                        
                        if (nodeCondition(c.getNodeR()))
                        {
                            double v = totalCurrent * c.getInternalResistance();
                            v = nodeV - v;
                            c.getNodeR().setVoltage(v);
                        }
                    }
                }

                /*  List<Node> startN = new List<Node>();
                  foreach (List<DC_Component> series in circuit)
                  {
                      startN.Add(series[0].getNodeL());
                  }
                  startN.Reverse();
                  foreach (List<DC_Component> series in circuit)
                  {

                      if (series[0].getNodeL().getNodeID() == supply.getNodeID())
                      {

                      }

                  }*/
            }
        }
        supply.setVoltage(voltage);//done already?
        ground.setVoltage(0);
    }


    private List<double> circuitResistance(List<List<List<DC_Component>>> circuit)//  [*] 
    {
        List<double> circEqs = new List<double>();
        double temp = 0;
        //Run through Each circuit
        for (int x = 0; x < circuit.Count; x++) //List of , lists of  , lists dc components ...BreadBoard >> Circuits >> parallels/series >> components
        {
            //Run through each series/parrallel
            for (int y = 0; y < circuit[x].Count; y++)
            {
                //If parrallel exsists
                if (circuit[x].Count > 1)
                {
                    temp = calculateParallel(circuit[x]);//Find parrallel resistance eq

                }
                else
                {
                    temp = seriesResistance(circuit[x][y], 0);//calculate total resistance
                }
                circEqs.Add(temp);
            }
        }
        return circEqs;
    }

    private double seriesResistance(List<DC_Component> series, int startIndex)//  [*] 
    {
        double totalResistance = 0;
        for (int i = startIndex; i < series.Count; i++)
        {
            totalResistance += series[i].getInternalResistance();
        }
        return totalResistance;
    }
    //Assumes all lists in parrallel are trully parrallel with eachother
    private double parallelRes(List<List<DC_Component>> parrallel)//  [*] 
    {
        List<double> resisters = new List<double>();
        foreach (List<DC_Component> series in parrallel)
        {
            resisters.Add(seriesResistance(series, 0));
        }
        return parallelRes(resisters);
    }
    //Assumes all lists in parrallel are trully parrallel with eachother
    private double parallelRes(List<double> resisters)//  [*] 
    {
        double equavRes = 0;
        foreach (double r in resisters)
        {
            equavRes += 1 / r;
        }
        equavRes = 1 / equavRes;
        return equavRes;/* 1/RT = 1/R1 + 1/R2 + ... + 1/Rn-1 + 1/Rn*/
    }
    //Find split nodes ie. 0 , 7, 20
    //find alls parallel values for the bottom split, combine
    // find split for 2nd etc
    //return total
    private double calculateParallel(List<List<DC_Component>> resisters) // circuit >>series >> components
    {

        double equavRes = 0;
        /* 
        List<List<DC_Component>> tempList = new List<List<DC_Component>>();//Store parrallels
        List<DC_Component> dummies = new List<DC_Component>();//Store start and end points
        List<double> res = new List<double>();
        */
        List<double> nodes = new List<double>();//Store start points
        List<double> parR = new List<double>();

        //Find The order in which nodes split
        foreach (List<DC_Component> series in resisters)
        {
            if (!nodes.Contains(series[0].getNodeL().getNodeID()))
            {
                nodes.Add(series[0].getNodeL().getNodeID());//Each node stores the splits
            }
        }

        int split = (int)nodes[nodes.Count - 1];//last node is first to be calculated
        int splitter = 0;
        foreach (double d in nodes)
        {
            //foreach list of components
            for (int i = 0; i < resisters.Count; i++)
            {
                //Look if components have split node
                for (int k = 0; k < resisters[i].Count; k++)
                {
                    if (resisters[i][k].getNodeL().getNodeID() == split)
                    {
                        splitter = k; //has the splitter done searching that series
                        break;
                    }
                    else
                    {
                        splitter = -1;//does not have the split ignore it
                    }
                }
                if (splitter > -1)
                {
                    parR.Add(seriesResistance(resisters[i], splitter));
                }
                equavRes = parallelRes(parR);
                parR.Clear();
                parR.Add(equavRes);
            }
            split = (int)nodes[nodes.IndexOf(split) - 1];
        }

        return equavRes;/* 1/RT = 1/R1 + 1/R2 + ... + 1/Rn-1 + 1/Rn*/
    }



    /*Call sort to organize component date  which is than used to find indivial node voltages*/
    public List<List<List<DC_Component>>> checkCircuit()
    {
        List<DC_Component> temp = new List<DC_Component>();
        List<List<List<DC_Component>>> circuit = sort(temp);


        calculateVoltages(circuit);


        int i = 1, j = 1;
        foreach (List<List<DC_Component>> b in circuit)
        {
            Debug.Log("Circuit number: " + (i++) + "\n");
            j = 1;
            foreach (List<DC_Component> list in b)
            {
                Debug.Log("\tCircuit series : " + (j++) + "\n");
                foreach (DC_Component c in list)
                {
                    Debug.Log("\t\t" + c);
                    Debug.Log("\t\t >> VoltageL:" + c.getNodeL().getVoltage().ToString().PadRight(3) + " , VoltageR :" + c.getNodeR().getVoltage().ToString().PadRight(3) + "\n");
                }
            }
        }
        Debug.Log("\n");


        foreach (DC_Component c in temp)
        {
            Debug.Log("Is not connected to circuit : " + c);
            components.Add(c);
        }

        return circuit;
    }


    private void resetVoltages()
    {
        double voltage = 0;
        try
        {
            voltage = double.Parse(sourceVoltage.GetComponent<TextMesh>().text.Substring(8, 2)); //trycatch?
        }
        catch (Exception e) { Debug.Log("Issue finding Voltage vale"); }

        leftNeg.setVoltage(-1);
        leftPos.setVoltage(-1);
        rightNeg.setVoltage(-1);
        rightPos.setVoltage(-1);
        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 10; k++)
            {
                nodeArray[k, i].setVoltage(-1);
            }
        }
        supply.setVoltage(voltage);
        ground.setVoltage(0);
    }

    private void adjustments(List<List<List<DC_Component>>> circuit)
    {
        components.Reverse();
        foreach (DC_Component c in components)
        {
            if (c.GetType().Equals(typeof(Wire)))
            {
                if (c.getNodeR().getNodeID() == ground.getNodeID())
                {
                    c.getNodeL().setVoltage(0);
                }
            }
            if (c.GetType().Equals(typeof(LED_Component)) )
            {
                

             /* 
              * nOT REquired anymore
              *   if (c.getNodeR().getVoltage() == -1)
                {
                    c.getNodeL().setVoltage(((LED_Component)c).getVoltageDrop());
                }
                else
                {
                    if (c.getNodeL().getNodeID() == supply.getNodeID())
                    {
                        if (c.getNodeR().getVoltage() == -1)
                        {
                            c.getNodeR().setVoltage(c.getNodeL().getVoltage());
                        }
                        else
                        {
                            c.getNodeR().setVoltage(c.getNodeL().getVoltage() - ((LED_Component)c).getVoltageDrop());
                        }
                    }
                    else
                    {
                        c.getNodeL().setVoltage(((LED_Component)c).getVoltageDrop() + c.getNodeR().getVoltage());
                    }
                }*/
            }
            
        }
        components.Reverse();
    }
}
