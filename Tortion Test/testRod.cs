using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testRod : MonoBehaviour
{
    [SerializeField]
    private GameObject virtualRod;//visual representation

    //abstract    
    private Vector3 rodScale; // 5cm:1scale
    private float a, b; //shape dimentions
    private float minA,minB;
    private int crossSectionType; //circle,ellipse,square,rectangle,eqTri,isoTri,eqHex


    //UI
    [SerializeField]
    private Slider sliderA, sliderB;
    [SerializeField]
    private string textA, textB;
    private string units;
    private int unitFactor;
    [SerializeField]
    private GameObject instuctionField;

    public testRod()
    {
        virtualRod = null;
        rodScale = new Vector3(1, 1, 1);
        a = 0;
        b = 0;
        minA = 0;
        minB = 0;
        sliderA = null;
        sliderB = null;
        textA = "";
        textB = "";
        units = "cm";
        unitFactor = 100;
        instuctionField = null;
    }

    //public void initialize(float minA,floatb, )
    

    public void UpdateRod()
    {
        a = sliderA.value * minA;
        b = sliderB.value * minB;

        string tmpa = textA + ":" + a + units;
        string tmpb = textA + ":" + a + units;



    } 







}
