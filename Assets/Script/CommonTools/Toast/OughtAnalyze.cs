using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OughtAnalyze : CopyVibration<OughtAnalyze>
{

    public void BeadOught(string info)
    {
        UIAnalyze.GetInstance().BeadUIFlank(nameof(Ought), info);
    }
}
