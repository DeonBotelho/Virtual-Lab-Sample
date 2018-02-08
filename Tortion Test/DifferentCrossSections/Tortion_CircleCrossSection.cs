using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tortion_CircleCrossSection : TortionRod
{

    //parent
    //  protected float length, polarMoment, Modulas;
    // protected string labelA, labelB,labelC;

    //child
    private float radius, t;

    // private bool solid;
    [SerializeField]
    private GameObject innerJoint, outterJoint;

    public Tortion_CircleCrossSection()//(float length, float polarMoment, float ModulasofRigidity,bool solid) : base(length, polarMoment, ModulasofRigidity,solid)
    {
        radius = 1;
        t = 1;

        labelA = "Radius";
        maxDimention = maxDimention / 2;
        minDimention = minDimention / 2;
        
        
    }

    public float maxThickness()
    {
        return ((radius) * maxThicknessPercent);
    }
    public override void update_()
    {
        thickness.maxValue = maxThickness();
        float inc = (maxDimention - minDimention) / partitions;
        float inc_ = (maxThickness() - minThickness) / partitions;

        radius = minDimention + dimention1.value* inc;
       // t = minThickness + thickness.value * inc_;
        t = thickness.value;

        radius = (float)Tortion.truncate(radius, 1000);
        t = (float)Tortion.truncate(t, 1000);

        
        dimention1.transform.parent.GetComponent<Text>().text = labelA + ": " + radius + " " + units;
        thickness.transform.parent.GetComponent<Text>().text = labelC + ": " + t + " " + units;

        radius = radius / 100;
        t = t / 100;
    }

    public override float solidMoment()
    {
        return polarMoment_Ellipse(radius, radius);
    }
    public override float hollowMoment()
    {
        float radius_ = radius - t;

        return polarMoment_HollowElliptical(radius, radius, radius_, radius_);
    }

    //cross section
    //0.025m : 0.50 scale  ==> 0.50 scale is 2.50 cm or 25.00 mm
    //0.100m : 2.00 scale  ==> 2.00 scale is 10.0 cm or 100.0 mm

    public override void updateSolidShape()
    {

        float x = representation.transform.localScale.x;
        float y = representation.transform.localScale.y;
        float z = representation.transform.localScale.z;

        y = radius / factor1 ;
        z = y;
        
        representation.transform.localScale = new Vector3(x, y, z);
    }

    public override void updateHollowShape()
    {

        float x = outterJoint.transform.localScale.x;
        float y = outterJoint.transform.localScale.y;
        float z = outterJoint.transform.localScale.z;
       
        y = radius / factor1;
        z = y;

        outterJoint.transform.localScale = new Vector3(x, y, z);

        y = factor2 * y - (thickness.value / factor3);
        z = y;
        x = (x < 0) ? 0 : x;
        y = (y < 0) ? 0 : y;
        z = (z < 0) ? 0 : z;
        innerJoint.transform.localScale = new Vector3(x, y, z);

    }

    public override void activesliders()
    {
        dimention1.transform.parent.gameObject.SetActive(true);
        dimention2.transform.parent.gameObject.SetActive(false);
        thickness.transform.parent.gameObject.SetActive(!solid);
    }

    public override void breakForce(float tauMax)
    {
        bool thin = base.isThin(radius, t);
        float maxForce;
        if (thin && !solid)
        {
           // Debug.Log("Thin hollow");
           /* maxForce = maxForce_HollowElliptical(tauMax, radius, radius, radius - t, radius - t);
            Debug.Log("Hollow Elliptical : " + maxForce);
            */
            float area = Mathf.Pow((radius - (t / 2)), 2) * Mathf.PI;
            maxForce = maxForce_ThinTube(tauMax, area, t);
            Debug.Log("Thin Tube" + maxForce);
        }
        else if (!thin && !solid)
        {
            Debug.Log("Thick hollow");
            maxForce = maxForce_Circle(tauMax, radius) - maxForce_Circle(tauMax, radius - t);
            Debug.Log("Solid outter - solid inner" + maxForce);                       
        }
        else
        {
            Debug.Log("solid");
            maxForce = maxForce_Circle(tauMax, radius);
        }
        breakingForce = maxForce;        
    }

  
   
    public override string ToString()
    {
        string tmp = (solid ? "Solid" : "Hollow") + " " + this.GetType().ToString().Substring(7) + "\n"
                        + "Radius " + radius + "m\n"
                        + "Thickness " + t + "m\n"
                        + "Length " + length + "m\n"
                        + "Polar Moment " + polarMoment + "m⁴\n"
                        + "Modulas of Rigidity" + ModulasofRigidity + "Gpa\n"
                        + "Breaking Torque " + breakingForce + "Nm"
                        + "Angle Of twist" + angleOfTwist(breakingForce, 1) + "rads";


        return tmp;
    }
}