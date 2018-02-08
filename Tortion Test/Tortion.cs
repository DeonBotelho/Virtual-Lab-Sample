using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tortion : Station
{

    [SerializeField]
    protected GameObject mainCam;
    [SerializeField]
    protected GameObject motorShaft;//, stationaryShaft;
    [SerializeField]
    protected GameObject stablizer;
    [SerializeField]
    protected GameObject shaftLocation;

    [SerializeField]
    protected GameObject maxShearStress;
    protected float torqueApplied = 0;

    private float minLength = 60f / 3;

    public static readonly string units = "cm";
    public static readonly int shearStresspower = 6;//GigaNm^2
    private float radToDeg = Mathf.Rad2Deg;
    public static readonly float min = 0.5f, max = 2;

    [SerializeField]
    private GameObject lengthBar;

    [SerializeField]
    private GameObject dimentions;

    [SerializeField]
    protected GameObject[] testRods;
    protected TortionRod testRod;
    private int currentShape;

    //protected GameObject currentylSelcted;

    [SerializeField]
    protected GameObject startingPoint;

    private Coroutine running;

    [SerializeField]
    Dropdown mat, selectedShape;
    [SerializeField]
    Material[] materials;
    //http://www.engineeringtoolbox.com/modulus-rigidity-d_946.html
    float[] modG = { 21, 27, 79.3f, 13f, 1 };//Giga pascals , concrete,alum,steel,wood
    int currentMaterial;


    [SerializeField]
    protected GameObject customizeCam;
    protected float rotateAngleX = 0.0f;
    protected float rotateAngleY = 1.4f;
    protected float rotateAngleZ = 0.0f;

    private Quaternion defaultJointRotation;

    void Start()
    {
        currentShape = 0;
        mat.value = 0;
        currentMaterial = 0;
        /**/
        foreach (GameObject o in objectParameters)
        {
            o.SetActive(false);
        }
        objectParameters[currentShape].SetActive(true);


        /**/
        testRod = testRods[currentShape].GetComponent<TortionRod>();

        testRod.changeSubmenu(submenu);

        submenu.value = 0;

        /**/
        foreach (GameObject x in testRods)
        {
            // x.SetActive(false);           

            //x.transform.localScale = new Vector3(1, 1, 1);
            x.GetComponent<TortionRod>().setdefaultSkin(materials[currentMaterial], 21);
            x.GetComponent<TortionRod>().reset();
        }

        defaultJointRotation = testRod.getRepresentation().transform.GetChild(0).GetChild(0).transform.rotation;
      
        /**/
        testRod.getRepresentation().SetActive(true);
        updateRod();
        controls.SetActive(true);
        chm.MoveCamera(stationCamera);
        chm.setOrthographic(true);
        chm.setOrthoSize(0.2f);
      //  chm.setOrthographic(false);
    }

    protected bool choose = false;
    public void choosecomponent()
    {
        checkWarnings();
        choose = !choose;
        float x = warning.transform.childCount;
        bool tmp;

        for (int i = 0; i < x; i++)
        {
            tmp = warning.transform.GetChild(i).gameObject.activeSelf;
            if (tmp)
            {
                choose = false;
                break;
            }
        }

        if (choose)
        { 
            updateRod();
          
            testRod.getRepresentation().transform.position = shaftLocation.transform.position;
            GameObject e = endOfRod();
            for (int i =(int) lengthBar.transform.GetChild(0).GetComponent<Slider>().value; i <=3 ; i++)
            {
                e = e.transform.parent.gameObject;
            }
            //stablizer.transform.position = endOfRod().transform.parent.parent.parent.position;
            stablizer.transform.position = e.transform.position;
            running = StartCoroutine(StartTest());
        }
        else if (running != null)//usless
        {
            StopCoroutine(running);
            running = null;
        }

    }



    void Update()
    {
              
       if (!choose)
        {
            Transform tmp = customizeCam.transform.parent;

            float x = tmp.eulerAngles.x + rotateAngleX;
            float y = tmp.eulerAngles.y + rotateAngleY;
            float z = tmp.eulerAngles.z + rotateAngleZ;
            tmp.rotation = Quaternion.Euler(new Vector3(x , y, z ));

            chm.MoveCamera(customizeCam.transform);
        }


    }

    private void rotateJoint(GameObject joint, Vector3 axis, float turnDegree)
    {       
        joint.transform.rotation = defaultJointRotation;
        joint.transform.Rotate(axis, turnDegree);

        if (joint.transform.childCount > 0)
        {
            // joint.transform.GetChild(0).Rotate(axis, turnDegree * -1);
        }
    }

    public void UIchange()
    {
        testRod.resetRotation();
        graphImage.SetActive(false);
        currentText.transform.parent.gameObject.SetActive(false);
    }
    private GameObject nextJoint(GameObject joint)
    {
        if (joint.transform.childCount > 0)
        {
            joint = joint.transform.GetChild(0).gameObject;
        }
        return joint;
    }
    private GameObject prevJoint(GameObject joint)
    {
        return joint.transform.parent.gameObject;
    }
    public GameObject endOfRod()
    {
        GameObject end = testRod.getRepresentation().gameObject;
        while (end.transform.childCount > 0)
        {
            end = end.transform.GetChild(0).gameObject;

        }
        return end;
    }
    public GameObject endOfRod(GameObject end)
    {
        while (end.transform.childCount > 0)
        {
            end = end.transform.GetChild(0).gameObject;

        }
        return end;
    }




    [SerializeField]
    protected GameObject[] objectParameters;
    [SerializeField]
    protected Dropdown submenu;
    public void changeshape()
    {
        UIchange();
       
        //Remove old item from display
        objectParameters[currentShape].SetActive(false);
        testRod.getRepresentation().SetActive(false);

        //Collect required information to put onto new item
        float temp1 = lengthBar.transform.GetChild(0).GetComponent<Slider>().value;
        int temp2 = mat.GetComponent<Dropdown>().value;

        //get current item
        currentShape = selectedShape.GetComponent<Dropdown>().value;
        testRod = testRods[currentShape].GetComponent<TortionRod>();
        testRod.changeSubmenu(submenu);
        submenu.value = 0;

        //Turn new item on
        objectParameters[currentShape].SetActive(true);
        testRod.getRepresentation().gameObject.SetActive(true);
        testRod.activesliders();

        //Correct for values to be transfered over
        lengthBar.transform.GetChild(0).GetComponent<Slider>().value = temp1;
        mat.GetComponent<Dropdown>().value = temp2;
        float d1 = dimentions.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value;
        dimentions.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = d1;
        updateLength();
        updateMaterial();
        changeIn_submenu();
        updateShearStress();
        testRod.update();
    }

    public void changeIn_submenu()
    {
        UIchange();
        testRod.crossSection_img(submenu);
        testRod.update();
        updateLength();
    }
    private GameObject tmp;
    public void updateLength()
    {
        UIchange();
        float length = lengthBar.transform.GetChild(0).gameObject.GetComponent<Slider>().value * minLength;

        testRod.setLength(length, minLength);
        string[] txt = { "small", "medium", "large" };


        lengthBar.GetComponent<Text>().text = "Test rod length : " + txt[(int)(length / minLength) - 1] + " (" + length + ") " + units;

        GameObject t = testRod.getRepresentation().transform.GetChild(0).gameObject;
        int n2 = testRod.jointCount(t.transform.GetChild(0).gameObject) / 2 + 1;//Bone# at center
        for (int i = 0; i <= n2; i++)
        {
            t = t.transform.GetChild(0).gameObject;
        }
        customizeCam.transform.parent.position = t.transform.position;
        
        //testing purposes to see camera is focused in center of rod
         /*GameObject.Destroy(tmp);
         tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
         tmp.transform.SetPositionAndRotation( customizeCam.transform.parent.position, new Quaternion(1, 0, 0, 0));        
         tmp.transform.localScale = new Vector3(0.05f, 0.075f, 0.075f);
         */

    }
    //When crosssection param changes re calculate J and update text     
    public void updateShearStress()
    {
        UIchange();
        float power = shearStresspower;
        //"⁰ ² ³ ⁴ ⁵ ⁶ ⁷ ⁸ ⁹ "
        try
        {
            float tauMax = float.Parse(maxShearStress.GetComponent<InputField>().text);
            tauMax = tauMax * Mathf.Pow(10, power);//N/m^2 x 10 ^ power
            testRod.breakForce(tauMax);
            torqueApplied = testRod.getBreakingForce();
            float angle = testRod.angleOfTwist(torqueApplied, 1);
            Debug.Log("Max Torque to before breaking: " + torqueApplied + "Nm");
            Debug.Log("Angle of twist: " + angle + "rads");
            currentText.text = "";
            if (torqueApplied <= 0 || angle <= 0)
            {
                currentText.text = "Try Increasing tau if too small, or decresing if too large.Thickness can also play a large factor.";
                currentText.transform.parent.gameObject.SetActive(true);
                throw new Exception();
            }
            if (angle > 1000)
            {
                currentText.text = "Tau value is to large, Item will instantly break.";
                currentText.transform.parent.gameObject.SetActive(true);
                throw new Exception();
            }
            warning.transform.GetChild(1).gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            currentText.text = "INVALID PARAMETERS \n" + currentText.text;
            warning.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void updateMaterial()
    {
        UIchange();
        customMat.gameObject.SetActive(false);
        currentMaterial = mat.value;
        if (currentMaterial == 4)
        {
            try
            {
                customMat.gameObject.SetActive(true);
                modG[4] = float.Parse(customMat.text);
                warning.transform.GetChild(0).gameObject.SetActive(false);
            }
            catch
            {
                warning.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        testRod.changeMaterial(getModulasofRigidity(mat), materials[currentMaterial]);
    }
    public void updateRod()
    {        
        updateMaterial();
        updateLength();
        testRod.update();
        updateShearStress();
    }

    public void checkWarnings()
    {
        updateMaterial();
        updateShearStress();
    }


    [SerializeField]
    private InputField customMat;
    [SerializeField]
    private GameObject warning;
    public float getModulasofRigidity(Dropdown m)
    {
        int x = m.value;

        float ModulasofRigidity = modG[x];
        float gigaPrefix = Mathf.Pow(10, 9);
        ModulasofRigidity = ModulasofRigidity * gigaPrefix;// n/m^2 * 10^9 = GigaPascals
        return ModulasofRigidity;

    }
    public static double truncate(double num, int factor)
    {
        num = (int)(num * factor);
        num = num / factor;
        return num;
    }

    protected override IEnumerator StartTest()
    {
        chm.MoveCamera(mainCam.transform);
        chm.setOrthographic(true);        
        testRod.resetRotation();
        
        controls.SetActive(false);

        
        
        
        yield return new WaitForSeconds(1f);
        //intialization
        GameObject mainRod = testRod.getRepresentation().transform.GetChild(0).gameObject;
        bool Solid = testRod.getSolid();
        GameObject outterJoint = mainRod.transform.GetChild(0).gameObject;
        GameObject innerJoint = Solid ? null : mainRod.transform.GetChild(1).gameObject;
        int numofJoints = testRod.jointCount(outterJoint);


        //Rotational parameters
        Vector3 axis = new Vector3(1, 0, 0);
        int numplotPoints = 20;//default could be user selected
        float maxTorque = testRod.getBreakingForce();
        float maxTwist = testRod.angleOfTwist(maxTorque, 1);
        float maxTwistD = maxTwist * radToDeg;
        float forceIncrements = maxTorque / numplotPoints;
        float animationFrames = 30f;
        Vector3 motorRotation = motorShaft.transform.rotation.eulerAngles;

        if (maxTorque <= 0)
        {
            Debug.Log(maxTorque);
            yield break;
        }

        InitializeTest();
        graphManager.InitalizeLegend(null);
        testRod.intializeData(numplotPoints);
        /************************************************************************************/
       
        int maxPowerX = graphPower(maxTorque);
        int maxPowerY = graphPower(maxTwistD);
        
        int maxX = (int)truncate((maxTorque / Mathf.Pow(10, maxPowerX)), 100) + 1;
        int maxY = (int)truncate((maxTwistD / Mathf.Pow(10, maxPowerY)), 100) + 1;
        //int maxY = (int)(maxTwistD + 1);
        Debug.Log(maxTorque + " " + maxPowerX + " " + maxX);
        Debug.Log(maxTwistD + " " + maxPowerY + " " + maxY);

//        string TorqueUnits = (maxPowerX != 0) ? "[N/m x 10^" + maxPowerX + "]" : "[N/m]";
  //      string TwistUnitsD = (maxPowerY != 0) ? "[ ⁰ x 10^"  + maxPowerY + "]" : "[⁰]";

        string TorqueUnits = (maxPowerX != 0) ? "x 10^" + maxPowerX + "[N/m]" : "[N/m]";
        string TwistUnitsD = (maxPowerY != 0) ? "x 10^" + maxPowerY + "[⁰]" : "[⁰]";

        graphManager.InitializeLineGraph(maxX, maxY, "Applied Torque " + TorqueUnits, "Angle Of Twist "+TwistUnitsD, "Angle of Twist vs Applied Torque");
        graphImage.SetActive(true);
        graphManager.UpdateGraph(0, 0, 0);
        for (float currentTorque = 0; currentTorque <= maxTorque;)
        {
            float goal = currentTorque + forceIncrements;
            for (float fractionalTorque = forceIncrements / animationFrames; currentTorque <= goal; currentTorque += fractionalTorque)
            {
                outterJoint = mainRod.transform.GetChild(0).gameObject;
                innerJoint = Solid ? null : mainRod.transform.GetChild(1).gameObject;

                for (int jointNum = 0; jointNum < numofJoints; jointNum++)
                {
                    float turnDegree = testRod.angleOfTwist(currentTorque, (float)(numofJoints - jointNum) / (float)numofJoints) * radToDeg;// coverted to degrees                    
                    if (jointNum == 0)
                    {
                        // Debug.Log(">>" + motorShaft.name + " turned " + turnDegree + " Degree ");                        
                        motorShaft.transform.rotation = Quaternion.Euler(new Vector3(motorRotation.x, motorRotation.y, motorRotation.z + turnDegree));
                        // motorShaft.transform.Rotate(new Vector3(0, 0, 1), turnDegree);
                    }
                    turnDegree = turnDegree * -1;//max speciman rotate same way as shaft

                    rotateJoint(outterJoint, axis, turnDegree);
                    outterJoint = nextJoint(outterJoint);
                    rotateJoint(outterJoint, axis, turnDegree * -1);

                    //inner joints technically should have it's own rotational calculations but it is not nessary at the moment
                    if (!Solid)
                    {
                        rotateJoint(innerJoint, axis, turnDegree);
                        innerJoint = nextJoint(innerJoint);
                        rotateJoint(innerJoint, axis, turnDegree * -1);
                    }

                    //yield return new WaitForSeconds(0.1f);
                }
                //yield return new WaitForSeconds(1f / animationFrames);

            }
            float ang = testRod.angleOfTwist(goal, 1);
            float angD = ang * radToDeg;                                
            float c = (float)truncate(currentTorque / Mathf.Pow(10, maxPowerX),1000);

            angD = (float)truncate(angD / Mathf.Pow(10, maxPowerY), 100000);
            int angPow = graphPower(ang);
            //string angUnits = (angPow != 0) ? "[rads x 10^" + angPow + "]" : "[rads]";
            string angUnits = (angPow != 0) ? "x 10^" + angPow + " [rads]" : "[rads]";
            ang = ang / Mathf.Pow(10,angPow);

            Debug.Log(">>>>>>>>>>>>>>>>>>[" + (int)(currentTorque / forceIncrements) + "]Current Torque: " + c + TorqueUnits + " by angleOfTwist: " + ang + angUnits + " = " + angD + TwistUnitsD);


            /*********Important*************/

            //testRod.updateData((int)(currentTorque / forceIncrements) - 1, c, angD);  //Formated data would need to use the powerX and powerY 

            //Current torque = c    * 10 ^ powerX
            //Angle of Twist = angD * 10 ^ powerY

            testRod.updateData((int)(currentTorque / forceIncrements) - 1, currentTorque, testRod.angleOfTwist(goal, 1) * radToDeg);//Raw data not formated


            //**********************************/

            motorShaft.transform.rotation = Quaternion.Euler(new Vector3(motorRotation.x, motorRotation.y, motorRotation.z + angD));
            graphManager.UpdateGraph(0, c, angD);

            yield return new WaitForSeconds(0.5f);//wait half a second between new data points
        }
        Debug.Log(testRod.ToString());
        choose = false;
        controls.SetActive(true);
        currentText.text = "Test Completed";
        currentText.transform.parent.gameObject.SetActive(true);
        chm.MoveCamera(customizeCam.transform);
        //chm.setOrthographic(false);
        testRod.getRepresentation().transform.position = startingPoint.transform.position;
        dofakeRotation(axis);


        ExportToExcel();/***************************************************************           NOT IMPLEMENTED YET                ***********************************/
        //ExportToPDF();/***************************************************************           NOT IMPLEMENTED YET                ***********************************/
        yield break;
    }

    //for values who's angle of twist is below 10 degrees
    private void dofakeRotation(Vector3 axis)
    {
        float maxTorque = testRod.getBreakingForce();
        float maxAngleOFTwist = testRod.angleOfTwist(maxTorque, 1) * radToDeg;
        float minTwist = 25f;//degrees

        if (maxAngleOFTwist < minTwist)
        {
            Debug.Log("Do Fake Rotation");
            currentText.text = currentText.text + " \n the angle shown has been amplified";
            GameObject joint = testRod.getRepresentation().transform.GetChild(0).GetChild(0).gameObject;
            GameObject jointi = testRod.getSolid() ? null : testRod.getRepresentation().transform.GetChild(0).GetChild(1).gameObject;
            float num = testRod.jointCount(joint);

            for (int i = 0; i <= num; i++)
            {
                float twist = minTwist - minTwist / num * i;
                rotateJoint(joint, axis, twist);
                joint = nextJoint(joint);

                if (jointi != null)
                {
                    rotateJoint(jointi, axis, twist);
                    jointi = nextJoint(jointi);
                }
            }
        }
    }

    private int graphPower(float maxValue)
    {       
        int i = 0;
        if (maxValue >= 1 || maxValue <=-1)
        {
            for (; maxValue >= 10; i++)
            {
                maxValue = maxValue / 10;
            }
            if (maxValue < 7)
            {
                i--;
            }
            if (i < 2)
            {
                return 0;//formating
            } 
        }
        else
        {
            for (; maxValue <10; i--)
            {
                maxValue = maxValue * 10;                
            }            
            if (maxValue < 7)
            {
                i++;
            }
        }
        
        return i;
    }
   
    protected override void ExportToPDF()
    {
        Vector2[] data = testRod.getData();
        throw new NotImplementedException();
    }

    protected override void ExportToExcel()
    {
        Vector2[] data = testRod.getData();//data to be imported into excel
        foreach(Vector2 v in data)
        {
            Debug.Log("(" + v.x + "," + v.y + ")");
        }    
    }
    public override void SetFromToggle(string group, int index)/****??****/
    {
        throw new NotImplementedException();
    }

    public override void RestartTest()/** Not sure how you're planning to implment this***/
    {
        Debug.Log("test in progress");
    }
}

