using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Yes it's "that" script and it does a lot of stuff
*/
public class _Utilities : MonoBehaviour
{
    // Create range from value n which acts in range start1 to stop1 to new range
    public static float MapRange(float n, float start1, float stop1, float start2, float stop2) 
    {
        float newval = (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        //if (newval != ) {return newval;}
        if (start2 < stop2) 
        {
            return Mathf.Clamp(newval, start2, stop2);
        } 
        else 
        {
            return Mathf.Clamp(newval, stop2, start2);
        }
    }

    public static float RoundTo(float v, float dec) 
    {
        return Mathf.Floor(v*Mathf.Pow(10,dec))/Mathf.Pow(10,dec);
    }

    public static Vector3 AddVec3(Vector3 v, Vector3 adding) {
        return new Vector3(v.x + adding.x, v.y + adding.y, v.z + adding.z);
    }

    public static Vector3 SubVec3(Vector3 v, Vector3 substrac)
    {
        return new Vector3(v.x - substrac.x, v.y - substrac.y, v.z - substrac.z);
    }

    public static Vector3 RndVec3(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x),Mathf.Round(v.y),Mathf.Round(v.z));
    }

    public static Vector3 AbsVec(Vector3 v) 
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
}
