using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 消息传递的参数
/// </summary>
public class SharplyLoom
{
    /*
     *  1.创建独立的消息传递数据结构，而不使用object，是为了避免数据传递时的类型强转
     *  2.制作过程中遇到实际需要传递的数据类型，在这里定义即可
     *  3.实际项目中需要传递参数的类型其实并没有很多种，这种方式基本可以满足需求
     */
    public bool ChileDeem;
    public bool ChileDeem2;
    public int ChileNor;
    public int ChileNor2;
    public int ChileNor3;
    public float ChileRoost;
    public float ChileRoost2;
    public double ChileShovel;
    public double ChileShovel2;
    public string ChileChange;
    public string ChileChange2;
    public GameObject ChileFadeTypify;
    public GameObject ChileFadeTypify2;
    public GameObject ChileFadeTypify3;
    public GameObject ChileFadeTypify4;
    public Transform ChileMeteoroid;
    public List<string> ChileChangeFoul;
    public List<Vector2> ChileSki2Foul;
    public List<int> ChileNorFoul;
    public System.Action ToddlerTentPerm;
    public Vector2 Six2_1;
    public Vector2 Six2_2;
    public SharplyLoom()
    {
    }
    public SharplyLoom(Vector2 v2_1)
    {
        Six2_1 = v2_1;
    }
    public SharplyLoom(Vector2 v2_1, Vector2 v2_2)
    {
        Six2_1 = v2_1;
        Six2_2 = v2_2;
    }
    /// <summary>
    /// 创建一个带bool类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SharplyLoom(bool value)
    {
        ChileDeem = value;
    }
    public SharplyLoom(bool value, bool value2)
    {
        ChileDeem = value;
        ChileDeem2 = value2;
    }
    /// <summary>
    /// 创建一个带int类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SharplyLoom(int value)
    {
        ChileNor = value;
    }
    public SharplyLoom(int value, int value2)
    {
        ChileNor = value;
        ChileNor2 = value2;
    }
    public SharplyLoom(int value, int value2, int value3)
    {
        ChileNor = value;
        ChileNor2 = value2;
        ChileNor3 = value3;
    }
    public SharplyLoom(List<int> value,List<Vector2> value2)
    {
        ChileNorFoul = value;
        ChileSki2Foul = value2;
    }
    /// <summary>
    /// 创建一个带float类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SharplyLoom(float value)
    {
        ChileRoost = value;
    }
    public SharplyLoom(float value,float value2)
    {
        ChileRoost = value;
        ChileRoost = value2;
    }
    /// <summary>
    /// 创建一个带double类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SharplyLoom(double value)
    {
        ChileShovel = value;
    }
    public SharplyLoom(double value, double value2)
    {
        ChileShovel = value;
        ChileShovel = value2;
    }
    /// <summary>
    /// 创建一个带string类型的数据
    /// </summary>
    /// <param name="value"></param>
    public SharplyLoom(string value)
    {
        ChileChange = value;
    }
    /// <summary>
    /// 创建两个带string类型的数据
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    public SharplyLoom(string value1,string value2)
    {
        ChileChange = value1;
        ChileChange2 = value2;
    }
    public SharplyLoom(GameObject value1)
    {
        ChileFadeTypify = value1;
    }

    public SharplyLoom(Transform transform)
    {
        ChileMeteoroid = transform;
    }
}

