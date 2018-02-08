using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tortion_SplitCrossSection : TortionRod
{

    private float medianLength, t;
    private float arcLength;
    private float arcAngle = 36;//degrees
    private float radius;


    [SerializeField]
    private GameObject innerJoint, outterJoint;

    public Tortion_SplitCrossSection()
    {
        medianLength = 1;
        t = 1;
        arcLength = 1;
        radius = medianLength + arcLength;
        labelA = "Median Line";
        solid = false;
        string[] tmp = { "---" };
        subMenuText = tmp;
        maxDimention = maxDimention / 2;
        minDimention = minDimention / 2;
    }

    //assume Thicness and radius is known
    private void updateMedian()
    {        
        float medianRadius = radius - (t / 2);        
        float circumference = 2 * Mathf.PI * medianRadius;
        arcLength = (arcAngle / 360) * 2 * Mathf.PI * medianRadius;
        medianLength = circumference - arcLength;
    }

    public override float solidMoment() { return hollowMoment(); }
    public override float hollowMoment()
    {
        return  polarMoment_OpenTube(medianLength, t);
    }

    public override void updateSolidShape() { updateHollowShape(); }
    public override void updateHollowShape()
    {
        float x = outterJoint.transform.localScale.x;
        float y = outterJoint.transform.localScale.y;
        float z = outterJoint.transform.localScale.z;

        y = radius /factor1;
        z = y;
        
        outterJoint.transform.localScale = new Vector3(x, y, z);

        y = factor2 * y - thickness.value / factor3;
        z = y;


        x = (x < 0) ? 0 : x;
        y = (y < 0) ? 0 : y;
        z = (z < 0) ? 0 : z;
        innerJoint.transform.localScale = new Vector3(x, y, z);
    }


    public float maxThickness()
    {
        return Mathf.Ceil(radius  * maxThicknessPercent);
    }
    public override void update_()
    {
        thickness.maxValue = maxThickness();
        float inc = (maxDimention - minDimention) / partitions;
        float inc_ = (maxThickness() - minThickness) / partitions;

        radius = minDimention + dimention1.value * inc;
        t = thickness.value;

        radius = (float)Tortion.truncate(radius, 1000);
        t = (float)Tortion.truncate(t, 1000);

        updateMedian();
        dimention1.transform.parent.GetComponent<Text>().text = labelA + ": " + medianLength + " " + units;
        thickness.transform.parent.GetComponent<Text>().text = labelC + ": " + t + " " + units;

        radius = radius / 100;
        t = t / 100;
        medianLength = medianLength / 100;        

    }

    public override void activesliders()
    {
        dimention1.transform.parent.gameObject.SetActive(true);
        dimention2.transform.parent.gameObject.SetActive(false);
        thickness.transform.parent.gameObject.SetActive(true);
    }

    public override void breakForce(float tauMax)
    {       
        float maxForce;
        maxForce = maxForce_OpenTube(tauMax, medianLength, t);
        breakingForce = maxForce;
    }

  

   
    public override string ToString()
    {
        string tmp = (solid ? "Solid" : "Hollow") + " " + this.GetType().ToString().Substring(7) + "\n"
                        + "Radius " + radius + "m\n"
                        + "Median Length " +medianLength + "m\n"
                        + "Thickness " + t + "m\n"
                        + "Length " + length + "m\n"
                        + "Polar Moment " + polarMoment + "m⁴\n"
                        + "Modulas of Rigidity" + ModulasofRigidity + "N/m ²\n"
                        + "Breaking Torque " + breakingForce + "Nm"
                        + "Angle Of twist" + angleOfTwist(breakingForce, 1) + "rads";


        return tmp;
    }
}
