using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TortionRod : MonoBehaviour
{


    [SerializeField]
    protected GameObject representation;
    [SerializeField]
    protected GameObject solidCyl, hollowCyl;


    [SerializeField]
    protected Slider dimention1, dimention2, thickness;
    //scaleing factors
    private static float fac = 1.2f;
    protected float factor1 = (5f * fac) / 100, factor2 = 2.0f, factor3 = 2.468f * fac;
    protected float lengthFac = 0.1f * 4f;

    [SerializeField]
    protected RawImage[] cross_img;
    [SerializeField]
    protected Material defaultSkin, defaultIndicator;

    protected float length, polarMoment, ModulasofRigidity;
    protected string labelA, labelB, labelC, units;
    protected string[] subMenuText;

    [SerializeField]
    protected bool solid;

    protected float breakingForce;

    protected float minDimention, maxDimention;
    protected float minThickness, maxthickness, maxThicknessPercent;
    protected float partitions;

    private Vector2[] angleByForce;    

    public TortionRod()//(float length, float polarMoment, float ModulasofRigidity, bool s)
    {
        this.length = 1;
        this.polarMoment = 1;
        this.ModulasofRigidity = 1;

        labelA = "Dimention 1";
        labelB = "Dimention 2";
        labelC = "thickness";

        units = Tortion.units;//units
        subMenuText = new string[2];
        subMenuText[0] = "Solid";
        subMenuText[1] = "Hollow";
        minDimention = 10f;//cm
        maxDimention = 50f;//cm
        minThickness = 0f;//cm
        maxThicknessPercent = 100f;//100% *100 m>cm
        maxthickness = maxDimention / 2 * maxThicknessPercent;

        partitions = 99f;
    }

    public float getBreakingForce()
    {
        return breakingForce;

    }

    public bool getSolid()
    {
        return solid;
    }

    /*********************************************************************************************/

    public void setLength(float newLength, float minLength)
    {
        float x = representation.transform.localScale.x;
        float y = representation.transform.localScale.y;
        float z = representation.transform.localScale.z;
        length = newLength / 100;


        length = length / factor1;

        length = length * 0.1f;//Rod Models are 1:10 scaled radius:length

        x = length;
        representation.transform.localScale = new Vector3(x, y, z);


    }

    /*********************************************************************************************/

    public void calculatePolarMoment()
    {
        if (solid)
        {
            polarMoment = solidMoment();
        }
        else
        {
            polarMoment = hollowMoment();
        }
        //  Debug.Log(polarMoment);
    }

    public abstract float solidMoment();
    public abstract float hollowMoment();
    public abstract void breakForce(float tauMax);

    protected float min = Tortion.min, max = Tortion.max;
    public abstract void updateSolidShape();
    public abstract void updateHollowShape();
    public abstract void activesliders();
    public abstract void update_();


    public void intializeData(int numOfDataPoints)
    {
        angleByForce = new Vector2[numOfDataPoints];
            
    }

    public void updateData(int index ,float xitem,float yitem)
    {
        angleByForce[index].x = xitem;
        angleByForce[index].y = yitem;
    }
    public Vector2[] getData()
    {
        return angleByForce;
    }

    /*********************************************************************************************/
    public void changeMaterial(float materislModulas, Material m)
    {
        Material[] tmp = { m, defaultIndicator };
        //int n = representation.transform.GetChild(0).transform.childCount - 1;
        //representation.transform.GetChild(0).transform.GetChild(n).GetComponent<SkinnedMeshRenderer>().materials = tmp;
        int n = solidCyl.transform.GetChild(0).transform.childCount - 1;
        solidCyl.transform.GetChild(0).transform.GetChild(n).GetComponent<SkinnedMeshRenderer>().materials = tmp;

        Material[] tmp1 = { m, defaultIndicator, };
        n = hollowCyl.transform.GetChild(0).transform.childCount - 1;
        hollowCyl.transform.GetChild(0).transform.GetChild(n).GetComponent<SkinnedMeshRenderer>().materials = tmp1;

        setModulasofRigidity(materislModulas);

    }

    public void setModulasofRigidity(float x)
    {
        ModulasofRigidity = x;
    }

    /*********************************************************************************************/


    public void update()
    {
        update_();//update dimentions
        calculatePolarMoment();
        if (solid)
        {
            updateSolidShape();
        }
        else
        {
            updateHollowShape();
        }

    }

    public float angleOfTwist(float Torque, float lengthPercent)
    {
        float length = this.length * lengthPercent;
        //        Debug.Log(length + " = " + this.length + " * " + lengthPercent);
        float theta = (float)(Torque * length) / (float)(ModulasofRigidity * polarMoment);
        //  Debug.Log(Torque + "* " + length + "/" + ModulasofRigidity + "*" + polarMoment);
        //Debug.Log(theta);
        return theta;
    }
    public float shearStress(float Torque, float r)
    {
        return Torque * r / polarMoment;
    }

    public int jointCount(GameObject joint)
    {
        int counter = 0;
        while (joint.transform.childCount > 0)
        {
            joint = joint.transform.GetChild(0).gameObject;
            counter++;
        }
        return counter;
    }


    public GameObject getRepresentation()
    {
        return representation;
    }

    public void changeSubmenu(Dropdown sub)
    {
        string[] options = getSubMenutext();
        sub.options.Clear();
        //sub.ClearOptions();
        foreach (string o in options)
        {
            Dropdown.OptionData x = new Dropdown.OptionData(o);
            sub.options.Add(x);
        }
    }
    public string[] getSubMenutext()
    {
        return subMenuText;
    }

    public void crossSection_img(Dropdown submenu)
    {
        GameObject tmp = representation;
        foreach (RawImage o in cross_img)
        {
            o.gameObject.SetActive(false);
        }
        int x = submenu.value;
        cross_img[x].gameObject.SetActive(true);
        representation.SetActive(false);
        if (subMenuText[x].Contains("Solid"))
        {
            solid = true;
            thickness.value = thickness.maxValue;
            representation = solidCyl;
            Debug.Log("solid");
        }
        else
        {
            solid = false;
            representation = hollowCyl;
            Debug.Log("hollow");
        }
        representation.transform.localScale = new Vector3(0.33f, 1, 1);
        update();
        representation.SetActive(true);
        activesliders();

    }

    public void reset()
    {
        dimention1.GetComponent<Slider>().value = 0;
        dimention2.GetComponent<Slider>().value = 0;
        thickness.GetComponent<Slider>().value = 0;

        update();
        solid = true;

        representation.transform.localScale = new Vector3(0.33f, 1, 1);

    }
    public void resetRotation()
    {
        GameObject mainRod = representation.transform.GetChild(0).gameObject;
        GameObject outterJoint = mainRod.transform.GetChild(0).gameObject;
        GameObject innerJoint = solid ? null : mainRod.transform.GetChild(1).gameObject;
        float numberOfJoints = jointCount(outterJoint);
        Quaternion resetRotation = Quaternion.Euler(new Vector3(0, 180f, 0));

        for (int i = 0; i < numberOfJoints; i++)
        {
            outterJoint.transform.rotation = resetRotation;
            outterJoint = outterJoint.transform.GetChild(0).gameObject;
            if (!solid)
            {
                innerJoint.transform.rotation = resetRotation;
                innerJoint = innerJoint.transform.GetChild(0).gameObject;
            }
        }

    }

    public void setdefaultSkin(Material m, float x)
    {
        defaultSkin = m;
        changeMaterial(x, m);
    }



    /*****************************************************************************************************/
    /*http://www.roymech.co.uk/Useful_Tables/Torsion/Torsion.html */
    /** STANDARD POLARMOMENT AND MAX SHEAR STRESS EQUATIONS**/

    public static float polarMoment_Circle(float radius)
    {
        return 0.5f * Mathf.PI * Mathf.Pow(radius, 4);
    }
    public static float polarMoment_Ellipse(float radiusA, float radiusB)
    {
        return (Mathf.PI * Mathf.Pow(radiusA, 3) * Mathf.Pow(radiusB, 3)) / (Mathf.Pow(radiusA, 2) + Mathf.Pow(radiusB, 2));
    }
    public static float polarMoment_Square(float length)
    {
        return 2.25f * Mathf.Pow(length * 0.5f, 4);
    }
    public static float polarMoment_Rectangle(float length, float width)
    {
        width = width * 0.5f;
        length = length * 0.5f;
        if (width < length)/*a>b*/
        {
            float temp = width;
            width = length;
            length = temp;
        }
        return width * Mathf.Pow(length, 3f) * ((16f / 3f) - 3.36f * (length / width) * (1f - Mathf.Pow(length, 4f) / (12f * Mathf.Pow(width, 4f))));
    }
    public static float polarMoment_EquilateralTriangle(float baseLength) { return (Mathf.Pow(baseLength, 4) * Mathf.Sqrt(3)) / (80); }
    public static float polarMoment_IsocelesTriangle(float baseLength, float height)
    {
        float angle_Alpha = 2 * Mathf.Atan((0.5f * baseLength) / height) * Mathf.Rad2Deg;
        float J = -1;
        if (angle_Alpha > 39 && angle_Alpha < 82)
        {
            J = (Mathf.Pow(baseLength, 3) * Mathf.Pow(height, 3)) / (15 * Mathf.Pow(baseLength, 2) + 20 * Mathf.Pow(height, 2));
        }
        else if (angle_Alpha > 82 && angle_Alpha < 120)
        {
            J = 0.0915f * Mathf.Pow(height, 4) * ((baseLength / height) - 0.8592f);
        }
        else
        {
            //throw new ArgumentException("Invalid dimentions: Angle calculated = " + angle_Alpha + ".\nAngle between sides must be between 32 degrees and 120 degrees.");
            throw new System.ArgumentException(angle_Alpha.ToString());
        }
        return J;
    }
    public static float polarMoment_EquilateralHexagon(float height, bool edgeHeight)
    {
        if (edgeHeight)
        {
            return 0.0649f * Mathf.Pow(height, 4);
        }
        return 0.1154f * Mathf.Pow(height, 4);
    }

    public static float polarMoment_HollowElliptical(float radiusA, float radiusB, float radiusA_, float radiusB_)
    {
        float numor = Mathf.PI * Mathf.Pow(radiusA, 3) * Mathf.Pow(radiusB, 3);
        float denom = Mathf.Pow(radiusA, 2) + Mathf.Pow(radiusB, 2);
        float q = radiusA_ / radiusA;
        float total = numor * (1 - Mathf.Pow(q, 4));
        total = total / denom;
        return total;
    }

    public static float polarMoment_HollowRectangle(float width, float height, float t, float t_)
    {
        float a = width, b = height;
        float numor = 2 * t * t_ * Mathf.Pow((a - 2), 2) * Mathf.Pow((b - t_), 2);
        float denom = a * t + b * t_ - Mathf.Pow(t, 2) - Mathf.Pow(t_, 2);
        float total = numor / denom;
        return total;
    }

    public static float polarMoment_OpenTube(float medianLength, float thickness)
    {
        return (1 * medianLength * Mathf.Pow(thickness, 2)) / 3f;
    }

    public static float polarMoment_Rectangles(float[] b, float[] h, char type)
    {
        // I  , L , T , C , + 
        // 0  , 1 , 2 , 3 , 4
        char[] form = { 'I', 'L', 'T', 'C', '+' };
        int model = 0;
        for (int i = 0; i < form.Length; i++)
        {
            if (type == form[i])
            {
                model = i;
                break;
            }
        }
        float n = b.Length;
        float[] n_ = { 1.3f, 1.0f, 1.12f, 1.3f, 1.17f };

        float total = 0;
        for (int i = 0; i < n; i++)
        {
            total += Mathf.Pow(b[i], 3) * h[i];
        }
        total = (n_[model] / 3) * total;
        return total;
    }
    /********************************************************************************************************/

    //tauMax = Torque * Factor
    //return TauMax/Factor ==> Torque

    public static float maxForce_Circle(float tauMax, float radius)
    {
        return (tauMax * Mathf.PI * Mathf.Pow(radius, 3) / 2);
    }
    public static float maxForce_Rectangle(float tauMax, float width, float height)
    {
        float a = width;
        float b = height;
        if (height < width)
        {
            a = height;
            b = width;
        }

        float factor = (3 / (8 * a * Mathf.Pow(b, 2))
                        * (1 + 0.6095f * (b / a) + 0.8865f * Mathf.Pow((b / a), 2)
                                - 1.8023f * Mathf.Pow((b / a), 3) + 0.91f * Mathf.Pow((b / a), 4)));

        return tauMax / factor;
    }

    //if thicnkess is verysmall
    public static float maxForce_ThinTube(float tauMax, float area, float thickness)
    {
        return tauMax * 2 * thickness * area;
    }

    public static float maxForce_HollowElliptical(float tauMax, float radiusA, float radiusB, float radiusA_, float radiusB_)
    {
        float a = radiusA;
        float b = radiusB;
        if (a < b)
        {
            a = radiusB;
            b = radiusA;
        }
        float q = radiusA_ / radiusA;
        float factor = 2 / (Mathf.PI * a * Mathf.Pow(b, 2) * (1 - Mathf.Pow(q, 4)));

        return tauMax / factor;
    }

    public static float maxForce_HollowRectangle(float tauMax, float width, float height, float t, float t_)
    {
        float a = width, b = height;
        float T_a = tauMax * 2 * t * (a - t) * (b - t_);
        float T_b = tauMax * 2 * t_ * (a - t) * (b - t_);
        float T = T_a;

        if (T_b < T_a)//return the lower force that would break either inner or outter structure
        {
            T = T_b;
        }
        return T;
    }

    public static float maxForce_OpenTube(float tauMax, float medianLength, float thickness)
    {
        float factor = (3 * medianLength + 1.8f * thickness) / (Mathf.Pow(medianLength, 2) * Mathf.Pow(thickness, 2));
        return tauMax / factor;
    }

    protected bool isThin(float radii, float thickness)
    {
        float ratio = thickness / radii;
        if (ratio < 0.02f) // Thin is considered to be thickness = 1/50 of the radius~
        { 
            return true;
        }
        return false;
    }

    /*****************************************************************************************************/

}
