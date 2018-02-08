using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircuitStation : Station
{
    //Breadboard standard components oriented Left to Right
    [SerializeField]
    private GameObject resisterLR;
    [SerializeField]
    private GameObject LEDLR;//RIGHT+
    private int LED_;//1 If left needs to be +    
    [SerializeField]
    private GameObject[] wire;

    //Deletion of component
    static GameObject selectedComponent;
    private bool selected = false;

    private GameObject currentType;

    //Breadboard values
    [SerializeField]
    protected GameObject[] breadboardRows;
    [SerializeField]
    private int rows, cols;
    [SerializeField]
    BoxCollider box;
    [SerializeField]
    private Camera bbcam;
    [SerializeField]
    private Camera pdfcam;
    [SerializeField]
    private GameObject gate;


    private BreadBoard bb;
    private GameObject loc1, loc2;
    //Power supply
    public static bool power = false;
    private int voltage = 0;

    //Player and camera parameters
    [SerializeField]
    GameObject playerx;
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private Transform breadBoardPos;
    private Vector3 mainCamStartPos;
    private Quaternion mainCamStartRot;


    //UI
    [SerializeField]
    private Button initializeButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button poweeOn;
    [SerializeField]
    private Button supplyMinus;
    [SerializeField]
    private Button supplyPlus;
    [SerializeField]
    private GameObject[] inputFields;//node1,2,resistor,resistorunit,wirecol,ledcolor
    [SerializeField]
    private Button[] components;//resistor,cap etc
    [SerializeField]
    private GameObject[] componentB;
    [SerializeField]
    private UnityEngine.UI.Text[] value;//10,1,1
    [SerializeField]
    private UnityEngine.UI.Text[] units;//kohms,node1,node2

    void Start()
    {
        mainCam = projectManager.mainCamera;
        bb = new BreadBoard(rows, cols);
        //Exit Circuit Station
        exitButton.onClick.AddListener(Exit);
        poweeOn.onClick.AddListener(powerOn);
        supplyMinus.onClick.AddListener(incrementSupply);
        supplyPlus.onClick.AddListener(incrementSupply);
        //Button UI
        for (int i = 0; i < components.Length - 1; i++)
        {
            //components[i].onClick.AddListener(makeComponent);
            components[i].onClick.AddListener(SetCurrentComponent);
            //Debug.Log(value[i].text + units[i].text);
        }
        components[5].onClick.AddListener(deleteComponent);

        bb.freeNode(0, 0); //power supply
        bb.freeNode(0, 13); //ground
        bb.makeSupply(bb.getNode(0, 0)); //throws and exception if ground == supply
        bb.makeGround(bb.getNode(0, 13));
        bb.getNode(0, 13).setVoltage(15);
        bb.getNode(0, 13).setVoltage(0);
        power = false;
        voltage = 15;
        GameObject.Find("SourceBox_VoltageText").GetComponent<TextMesh>().text = "Voltage:" + voltage.ToString().PadRight(2) + "V";
        Trigger("oscilloscope");
    }

    private bool placing = false;
    private GameObject currentlyPlacing;
    private GameObject leftJoint;
    private GameObject rightJoint;
    private GameObject body;
    [SerializeField]
    private LayerMask lm;
    [SerializeField]
    private LayerMask compMask;

    [SerializeField]
    private GameObject circuitData;

    private int[] row = new int[2];
    private int[] col = new int[2];

    // Update is called once per frame   
    private void Update()
    {
        if (ProjectManager.menuOpen)
        {
            return;
        }
        bool leftClick = Input.GetMouseButtonDown(0);
        bool midClick = Input.GetMouseButtonDown(2);
        if (leftClick || midClick && !placing)
        {
            RaycastHit hitinfo;
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hitinfo, 10f, compMask))
            {
                String name = hitinfo.transform.parent.name;
                if (!name.Equals("oscilloscope") && !name.Contains("staion") && name.Length > 2)
                {
                    selectedComponent = hitinfo.transform.parent.gameObject;
                    inputFields[6].GetComponent<InputField>().text = name;
                    if (midClick)
                    {
                        deleteComponent();//Shortcut
                    }
                }
            }
        }

        if (currentType != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, 10f, lm))
                {
                    if (hit.transform.tag == "Interactable")
                    {
                        if (NodeEmpty(hit.transform.name, !placing ? 1 : 2))
                        {
                            if (!placing)
                            {
                                currentlyPlacing = GameObject.Instantiate(currentType);
                                currentlyPlacing.name = currentType.name + currentlyPlacing.GetComponent<DC_Component>().componentNum();
                                currentlyPlacing.transform.parent = this.transform;

                                SetColour();

                                leftJoint = currentlyPlacing.transform.GetChild(0).gameObject;
                                rightJoint = currentlyPlacing.transform.GetChild(1).gameObject;
                                body = currentlyPlacing.transform.GetChild(2).gameObject;

                                body.transform.position = hit.transform.position;
                                leftJoint.transform.position = hit.transform.position;
                                rightJoint.transform.position = hit.transform.position;

                                inputFields[0].GetComponent<InputField>().text = hit.transform.name;
                                placing = true;
                            }
                            else
                            {
                                rightJoint.transform.position = hit.transform.position;
                                inputFields[1].GetComponent<InputField>().text = hit.transform.name;
                                MoveCurrent();
                                //makeComponent();
                                placing = false;
                                bb.placeComponent(currentlyPlacing, currentType.name, row, col, LED_);

                                float scale = currentlyPlacing.transform.localScale.x;
                                //Debug.Log(loc1.transform.position.z);
                                //Debug.Log(loc2.transform.position.z);
                                //Debug.Log(scale);
                                Vector3 size = new Vector3();
                                size.x = (float)Math.Sqrt(Math.Pow(((leftJoint.transform.position.z - rightJoint.transform.position.z) / scale), 2)
                                    + Math.Pow((leftJoint.transform.position.x - rightJoint.transform.position.x) / scale, 2));
                                //Debug.Log(x.ToString());
                                size.y = currentlyPlacing.GetComponent<BoxCollider>().size.y;
                                size.z = currentlyPlacing.GetComponent<BoxCollider>().size.z;

                                body.GetComponent<BoxCollider>().size = size;
                                //Debug.Log(currentlyPlacing.GetComponent<BoxCollider>().size.x.ToString());
                                //********************************************************************************************
                            }
                        }
                    }
                }
            }
            else if (placing)
            {
                RaycastHit hit;
                if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, 10f, lm))
                {
                    if (hit.transform.tag == "Interactable")
                    {
                        Debug.Log(hit.transform.name);
                        rightJoint.transform.position = hit.point;
                        inputFields[1].GetComponent<InputField>().text = hit.transform.name;

                        MoveCurrent();
                    }
                }
            }
        }
    }

    private void SetColour()
    {
        String name = currentType.name;
        if (name.Equals("Wire"))
        {
            currentlyPlacing.GetComponent<Wire>().setColor(value[0].text);
        }
        else if (name.Equals("Resistor"))
        {
            currentlyPlacing.GetComponent<Resistor>().setColor(value[2].text);
        }
        else if (name.Equals("LED"))
        {
            currentlyPlacing.GetComponent<LED_Component>().setColor(value[1].text);
        }
    }

    private bool NodeEmpty(string name, int clickNum) //Click num either 1 or 2
    {
        clickNum -= 1;
        //getbuttonName make item
        Debug.Log("*" + name + "*" + value[4].text + "*");
        //Change info into row x col
        try
        {
            col[clickNum] = name.ToUpper().ToCharArray()[0] - 65; //A-A = 0
            if (name.Length == 3)
            {
                String temp = name.ToCharArray()[1].ToString() + "" + name.ToCharArray()[2].ToString() + "";
                Debug.Log(temp);
                row[clickNum] = Int32.Parse(temp) - 1; // 10 -1 = 9
            }
            else
            {
                row[clickNum] = name.ToCharArray()[1] - 49; //1-49 = 0
            }

            bool use = bb.freeNode(row[clickNum], col[clickNum]); //If avalaible use Node, use = true
            if (!use)
            {
                bb.freeNode(row[clickNum], col[clickNum]);
            }
            return use;
        }
        catch (Exception e)
        {
            //Output: " incorrect input"
            Debug.Log("Incorrect Input format" + e + e.Message);
            Debug.Log("Creating" + name + "...Enter two free locations");
        }
        return false;
    }
    private void MoveCurrent()
    {
        Vector3 leftPos = leftJoint.transform.position;
        Vector3 rightPos = rightJoint.transform.position;
        float angle;
        if (rightPos.x - leftPos.x == 0f)
        {
            if (leftPos.z > rightPos.z)
            {
                angle = -90;
            }
            else
            {
                angle = 90;
            }
        }
        else
        {
            angle = Mathf.Atan((rightPos.z - leftPos.z) / (rightPos.x - leftPos.x)) * Mathf.Rad2Deg;
        }

        if (leftPos.x > rightPos.x)
        {
            angle -= 180;
        }

        Debug.Log(angle);
        body.transform.position = new Vector3((rightPos.x + leftPos.x) / 2, body.transform.position.y, (rightPos.z + leftPos.z) / 2);

        rightJoint.transform.rotation = leftJoint.transform.rotation = body.transform.rotation = Quaternion.Euler(body.transform.rotation.x, -angle, body.transform.rotation.z);

    }

    private void SetCurrentComponent()
    {
        String name = EventSystem.current.currentSelectedGameObject.name;
        currentText.text = "Currently placing: " + name;
        if (name.Equals("Wire"))
        {
            Debug.Log("Chose wire");
            currentType = wire[0];
        }
        else if (name.Equals("Resistor"))
        {
            Debug.Log("Chose resistor");
            currentType = resisterLR;
        }
        else if (name.Equals("LED"))
        {
            Debug.Log("Chose led");
            currentType = LEDLR;
        }
    }

    //                                         -------------------------------------
    private void powerOn()
    {
        power = !power;
        ColorBlock cb = poweeOn.colors;
        if (power)
        {
            checkCircuit();
            Debug.Log("power on");
            cb.normalColor = Color.green;

            poweeOn.colors = cb;

        }
        else
        {
            Debug.Log("power off");
            cb.normalColor = Color.black;

            poweeOn.colors = cb;

            //colors.normalColor = Color.black;
        }
    }

    protected override void ExportToExcel()
    {
        // throw new NotImplementedException();
    }

    protected override void ExportToPDF()
    {
        bb.checkCircuit();

        //StartCoroutine(pdfManager.ExportCircuitPDF(bb.checkCircuit(), mainCam,pdfcam));       
    }

    //Turn on power supply?                     //                                  ---------------------------------------------------
    protected override IEnumerator StartTest()
    {
        throw new NotFiniteNumberException();
    }


    private void checkCircuit()
    {
        try
        {
            ExportFiles(false);
        }
        catch (Exception e)
        {
            Debug.Log("Breadboard is empty" + e);
        }

    }

    private void incrementSupply()
    {
        String name = EventSystem.current.currentSelectedGameObject.name;

        if (name.Equals(GameObject.Find("plus").name))
        {
            if (voltage < 15)
            {
                voltage++;
            }
            else
            {
                voltage = 15;
            }

        }
        else if (name.Equals(GameObject.Find("minus").name))
        {
            if (voltage > 0)
            {
                voltage--;
            }
            else
            {
                voltage = 0;
            }
        }
        bb.getNode(0, 0).setVoltage(voltage);
        Debug.Log("vOLTAGE:" + voltage);
        GameObject.Find("SourceBox_VoltageText").GetComponent<TextMesh>().text = "Voltage:" + voltage.ToString().PadRight(2) + "V";

    }
    private void Exit()
    {
        chm.MoveCameraToStart();
        //mainCam.transform.position = mainCamStartPos;
        //mainCam.transform.rotation = mainCamStartRot;
        box.enabled = true;

        //mainCam.orthographic = false;
        //mainCam.orthographicSize = 5;
        //Cursor.lockState = CursorLockMode.Locked;
        GameObject.Find(name).transform.gameObject.tag = "Interactable";
        GameObject.Find(name).GetComponent<BoxCollider>().enabled = false;
        gate.SetActive(false);
        //base.ResetStation();

        circuitData.SetActive(false);


        power = false;
        voltage = 0;

    }

    //Place player into circuit station
    public override void Trigger(string name)
    {

        Debug.Log(name.ToString());


        if (name.Contains("oscilloscope"))
        {
            Debug.Log("bb");
            base.Trigger("Controls");

            //playerx.transform.position = new Vector3(-198.1947F, 0, 17.4F);
            //playerx.transform.rotation = Quaternion.Euler(0, 269.5f, 0);

            //playerx.transform.position = playerPos.position;
            //playerx.transform.rotation = playerPos.rotation;

            chm.SetBackupCameraValues();
            chm.MoveCamera(breadBoardPos);


            //mainCamStartPos = mainCam.transform.position;
            //mainCamStartRot = mainCam.transform.rotation;

            //mainCam.transform.position = new Vector3(bbcam.transform.position.x, mainCam.transform.position.y, bbcam.transform.position.z);
            //mainCam.transform.rotation = bbcam.transform.rotation;

            //mainCam.orthographic = true;
            //mainCam.orthographicSize = 0.2f;

            //gate.SetActive(true);
            circuitData.SetActive(true);
            //GameObject.Find(name).GetComponent<BoxCollider>().enabled = false;

            //testCircuit();
        }
    }

    public static void setSelectedComponent(GameObject g)
    {
        selectedComponent = g;
    }

    public int componentSelected()
    {
        selected = !selected;
        if (selected)
        {
            return 1;
        }
        return 0;
    }
    public void makeComponent()
    {
        power = true;
        powerOn();
        int[] row = new int[2];
        int[] col = new int[2];
        bool use1, use2;

        //getbuttonName make item
        String name = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("*" + value[3].text.ToString() + "*" + value[4].text + "*");
        //Change info into row x col
        try
        {


            col[0] = value[3].text.ToUpper().ToCharArray()[0] - 65; //A-A = 0
            col[1] = value[4].text.ToUpper().ToCharArray()[0] - 65; //N-A = 13
            if (value[3].text.ToCharArray().Length == 3)
            {
                String temp = value[3].text.ToCharArray()[1].ToString() + "" + value[3].text.ToCharArray()[2].ToString() + "";
                Debug.Log(temp);
                row[0] = Int32.Parse(temp) - 1; // 10 -1 = 9
            }
            else
            {
                row[0] = value[3].text.ToCharArray()[1] - 49; //1-49 = 0
            }
            if (value[4].text.ToCharArray().Length == 3)
            {
                String temp = value[4].text.ToCharArray()[1].ToString() + "" + value[4].text.ToCharArray()[2].ToString() + "";
                Debug.Log(temp);
                row[1] = Int32.Parse(temp) - 1; // 10 -1 = 9

            }
            else
            {
                row[1] = value[4].text.ToCharArray()[1] - 49; //9-49 = 8
            }

            Debug.Log("Node 1: " + row[0] + "," + col[0] + " Node 2: " + row[1] + "," + col[1]);

            if (row[0] != row[1] || col[0] != col[1])
            {
                use1 = bb.freeNode(row[0], col[0]); //If avalaible use Node, use = true
                use2 = bb.freeNode(row[1], col[1]); //Else throw error , use = false
            }
            else
            {
                throw new MemberAccessException();
            }
            String msg = "";
            if (!use1 || !use2)
            {
                bb.freeNode(row[0], col[0]);//Reuse node because it was turned off.
                bb.freeNode(row[1], col[1]);//Reuse node because it was turned off.
                msg += "\n\t- => Node 1: " + row[0] + "," + col[0];
                msg += "\n\t- => Node 2: " + row[1] + "," + col[1];
            }
            if (!use1 || !use2)
            {
                throw new FieldAccessException("Location(s) Already in use : " + msg);

            }


            //If input field:Node1 is placed below Node 2
            LED_ = 0;
            if (row[0] > row[1]) // || col1 > col2 ???
            {
                var temp = value[3];
                value[3] = value[4];
                value[4] = temp;
                LED_ = 1;
            }

            loc1 = GameObject.Find(value[3].text.ToUpper());
            loc2 = GameObject.Find(value[4].text.ToUpper());
            if (loc1 == null || loc2 == null)
            {
                throw new Exception("loc null");
            }
            //Update breadbord that a component has been placed.


            String text = "WHITE";
            if (name.Equals("Wire"))
            {
                text = value[0].text;
            }
            else if (name.Equals("Resistor"))
            {
                text = value[2].text;
            }
            else if (name.Equals("LED"))
            {
                text = value[1].text;
            }
            else
            {
                text = "WHITE";
            }
            Debug.Log("COLOR " + text);
            bb.placeComponent(placeComponent(name), name, row, col, LED_);

        }
        catch (FieldAccessException e)
        {
            Debug.Log(e.Message);
        }
        catch (MemberAccessException e)
        {
            Debug.Log("Node is already in use" + e);
            Debug.Log("Creating" + name + "...Enter two free locations");
        }
        catch (Exception e)
        {
            //Output: " incorrect input"
            Debug.Log("Incorrect Input format" + e + e.Message);
            Debug.Log("Creating" + name + "...Enter two free locations");
        }

    }

    private GameObject placeComponent(String name)
    {
        GameObject newComponent = null;//new GameObject();


        if (name.Equals("Wire"))
        {
            newComponent = wire[0];
            //newDC = new Wire(n1,n2);         
        }
        else if (name.Equals("Resistor"))
        {
            newComponent = resisterLR;
        }
        else if (name.Equals("LED"))
        {
            newComponent = LEDLR;
        }
        else
        {
            return null;
        }

        //Find rotation angle between two points
        float x1 = loc1.transform.position.x;
        float y1 = loc1.transform.position.y;
        float z1 = loc1.transform.position.z;

        float x2 = loc2.transform.position.x;
        float y2 = loc2.transform.position.y;
        float z2 = loc2.transform.position.z;

        float angle = System.Convert.ToSingle(Math.Atan((z2 - z1) / (x2 - x1)) * 180 / Math.PI);
        //Correct for poliarization rotation aka switched input fields
        angle += 180 * LED_;

        Debug.Log(angle + "Degrees");

        float x = (loc1.transform.position.x + loc2.transform.position.x) * 0.5f;
        float y = (loc1.transform.position.y + loc2.transform.position.y) * 0.5f; //should 0 always
        float z = (loc1.transform.position.z + loc2.transform.position.z) * 0.5f;
        Vector3 newPos = new Vector3(x, y, z);

        GameObject newComp = GameObject.Instantiate(newComponent, newPos, newComponent.transform.rotation);
        //newComp.name = "Hi";
        //newComp.transform.parent = this.gameObject.transform;



        newComp.transform.rotation = Quaternion.Euler(newComp.transform.rotation.x, -angle, newComp.transform.rotation.z);

        //Move ends to node specified

        GameObject joint1 = newComp.transform.GetChild(0 + LED_).gameObject;
        GameObject joint2 = newComp.transform.GetChild(1 - LED_).gameObject;  //+   when LED_ = 0 

        joint1.transform.position = loc1.transform.position;
        joint2.transform.position = loc2.transform.position;

        //newComp.GetComponent<DC_Component>().setPositiveNode(newComp.transform.GetChild(1 - LED_).gameObject);
        //LED_ = (int) angle; 

        //********************************************************************************************
        //Increase box collider size
        float scale = newComp.transform.localScale.x;
        //Debug.Log(loc1.transform.position.z);
        //Debug.Log(loc2.transform.position.z);
        //Debug.Log(scale);
        x = (float)Math.Sqrt(Math.Pow(((loc1.transform.position.z - loc2.transform.position.z) / scale), 2)
            + Math.Pow(((loc1.transform.position.x - loc2.transform.position.x) / scale), 2));
        //Debug.Log(x.ToString());
        y = newComp.GetComponent<BoxCollider>().size.y;
        z = newComp.GetComponent<BoxCollider>().size.z;

        newComp.GetComponent<BoxCollider>().size = new Vector3(x, y, z);
        //Debug.Log(newComp.GetComponent<BoxCollider>().size.x.ToString());
        //********************************************************************************************

        return newComp;

    }

    public void deleteComponent()
    {
        try
        {
            bb.removeComponent(selectedComponent.GetComponent<DC_Component>());
        }
        catch (Exception e)
        {
            Debug.Log(selectedComponent.GetComponent<DC_Component>());
        }

        GameObject.DestroyObject(selectedComponent);
    }

    private void testCircuit()
    {
        //componentB: wire 0,res 1 ,led 2
        //1 and 0 switched for testing should not matter technically
        inputFields[1].GetComponent<InputField>().text = "A2";
        inputFields[0].GetComponent<InputField>().text = "C2";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[1]);
        makeComponent();

        inputFields[1].GetComponent<InputField>().text = "G2";
        inputFields[0].GetComponent<InputField>().text = "H2";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[2]);
        makeComponent();

        inputFields[1].GetComponent<InputField>().text = "L2";
        inputFields[0].GetComponent<InputField>().text = "N2";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[0]);
        makeComponent();

        inputFields[0].GetComponent<InputField>().text = "A6";
        inputFields[1].GetComponent<InputField>().text = "C6";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[1]);
        makeComponent();

        inputFields[0].GetComponent<InputField>().text = "G6";
        inputFields[1].GetComponent<InputField>().text = "H6";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[2]);
        makeComponent();

        inputFields[0].GetComponent<InputField>().text = "L6";
        inputFields[1].GetComponent<InputField>().text = "N6";
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(componentB[0]);
        makeComponent();

        try
        { checkCircuit(); }
        catch (Exception e)
        {

        }

    }

    public override void SetFromToggle(string group, int index)
    {
        throw new NotImplementedException();
    }

    public override void RestartTest()
    {
        throw new NotImplementedException();
    }
}
