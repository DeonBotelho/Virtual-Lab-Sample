using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tortion_RectangleCrossSection : TortionRod {

    //child
    private float height, width,t;

    // private bool solid;
    [SerializeField]
    private GameObject innerJoint, outterJoint;


    public Tortion_RectangleCrossSection()//(float length, float polarMoment, float ModulasofRigidity, bool solid) : base(length, polarMoment, ModulasofRigidity,solid)
    {
        height = 1;
        width = 1;
        t= 1;
        
        labelA = "Height";
        labelB = "Width";
        

    }

   
    public float maxThickness()
    {
        float x = width;

        if (height < x)
        {
            x = height;
        }
        return (x /2  * maxThicknessPercent);
    }

    public override void update_()
    {
        thickness.maxValue = maxThickness();
        float inc = (maxDimention - minDimention) / partitions;
        float inc_ = (maxThickness() - minThickness) / partitions;

        height = minDimention + dimention1.value * inc;
        width = minDimention + dimention2.value * inc;
       // t = minThickness + thickness.value * inc_;
        t = thickness.value;
        height = (float)Tortion.truncate(height, 1000);
        width = (float)Tortion.truncate(width, 1000);
        t = (float)Tortion.truncate(t, 1000);

        dimention1.transform.parent.GetComponent<Text>().text = labelA + ": " + height + " " + units;
        dimention2.transform.parent.GetComponent<Text>().text = labelB + ": " + width + " " + units;
        thickness.transform.parent.GetComponent<Text>().text = labelC + ": " + t + " " + units;

        height = height / 100;
        width = width / 100;
        t = t / 100;

    }
    public override float solidMoment()
    {
        return  polarMoment_Rectangle(height, width);
    }
    public override float hollowMoment()
    {
        float height_ = height - t;
        float width_ = width - t;
        return  polarMoment_HollowRectangle(height, width, height_, width_);
    }

    //cross section
    //0.025m : 0.50 scale  ==> 0.50 scale is 2.50 cm or 25.00 mm
    //0.100m : 2.00 scale  ==> 2.00 scale is 10.0 cm or 100.0 mm
    
    public override void updateSolidShape()
    {
        float x = representation.transform.localScale.x;
        float y = representation.transform.localScale.y;
        float z = representation.transform.localScale.z;
        float h = height / 2;// height radii
        float w = width / 2;// width radii

        y = h / factor1;
        z = w / factor1;

        representation.transform.localScale = new Vector3(x, y, z);
    }
    public override void updateHollowShape()
    {
        float x = outterJoint.transform.localScale.x;
        float y = outterJoint.transform.localScale.y;
        float z = outterJoint.transform.localScale.z;
        float h = height / 2;// height radii
        float w = width / 2;// width radii

        y = h / factor1;
        z = w /  factor1;

        outterJoint.transform.localScale = new Vector3(x, y, z);
       
        y = factor2 * y - thickness.value / factor3;
        z = factor2 * z - thickness.value / factor3;
        x = (x < 0) ? 0 : x;
        y = (y < 0) ? 0 : y;
        z = (z < 0) ? 0 : z;
        innerJoint.transform.localScale = new Vector3(x, y, z);

    }

    public override void activesliders()
    {
        dimention1.transform.parent.gameObject.SetActive(true);
        dimention2.transform.parent.gameObject.SetActive(true);
        thickness.transform.parent.gameObject.SetActive(!solid);
    }

    public override void breakForce(float tauMax)
    {
        float tmp = height;
        if (width < height)
        {
            tmp = width;
        }
        bool thin = base.isThin(tmp, t);        
        float maxForce;
        if (thin)
        {
            maxForce = maxForce_HollowRectangle(tauMax, width, height, t, t);
        }
        else
        {
            maxForce = maxForce_Rectangle(tauMax, width, height);
        }
        breakingForce = maxForce;
    }
   

   
    public override string ToString()
    {
        string tmp = (solid ? "Solid" : "Hollow") + " " + this.GetType().ToString().Substring(7) + "\n"
                        + "Width " + width + "m\n"
                        + "Height " + height+ "m\n"
                        + "Thickness " + t + "m\n"
                        + "Length " + length + "m\n"
                        + "Polar Moment " + polarMoment + "m⁴\n"
                        + "Modulas of Rigidity" + ModulasofRigidity + "Gpa\n"
                        + "Breaking Torque " + breakingForce + "Nm"
                        + "Angle Of twist" + angleOfTwist(breakingForce, 1) + "rads";



        return tmp;
    }
}