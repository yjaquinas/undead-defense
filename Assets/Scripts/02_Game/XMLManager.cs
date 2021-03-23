using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class XMLManager : MonoBehaviour
{

    public static XMLManager instance;

    public TextAsset patternFileXml;

    struct Pattern
    {
        public int pattern0;
        public int pattern1;
        public int pattern2;
        public int pattern3;
        public int pattern4;
        public int pattern5;
        public int pattern6;
        public int pattern7;
        public int pattern8;
        public int pattern9;
    }

    Dictionary<string, Pattern> dicPattern = new Dictionary<string, Pattern>();

    // Use this for initialization
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void MakePatternXML(ref int p0, ref int p1, ref int p2, ref int p3, ref int p4, ref int p5, ref int p6, ref int p7, ref int p8, ref int p9)
    {
        XmlDocument patternXMLDoc = new XmlDocument();
        patternXMLDoc.LoadXml(patternFileXml.text);

        XmlNodeList patternNodeList = patternXMLDoc.GetElementsByTagName("row");

        foreach(XmlNode patternNode in patternNodeList)
        {
            foreach(XmlNode childNode in patternNode.ChildNodes)
            {
                switch(childNode.Name)
                {
                    case "pattern0":
                        p0 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern1":
                        p1 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern2":
                        p2 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern3":
                        p3 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern4":
                        p4 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern5":
                        p5 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern6":
                        p6 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern7":
                        p7 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern8":
                        p8 = int.Parse(childNode.InnerText);
                        break;
                    case "pattern9":
                        p9 = int.Parse(childNode.InnerText);
                        break;
                }
            }
        }
;
    }

    public void LoadPatternFromXML(string pattern, CEnemySpawn es)
    {
        es.pattern0 = dicPattern[pattern].pattern0;
        es.pattern1 = dicPattern[pattern].pattern1;
        es.pattern2 = dicPattern[pattern].pattern2;
        es.pattern3 = dicPattern[pattern].pattern3;
        es.pattern4 = dicPattern[pattern].pattern4;
        es.pattern5 = dicPattern[pattern].pattern5;
        es.pattern6 = dicPattern[pattern].pattern6;
        es.pattern7 = dicPattern[pattern].pattern7;
        es.pattern8 = dicPattern[pattern].pattern8;
        es.pattern9 = dicPattern[pattern].pattern9;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
