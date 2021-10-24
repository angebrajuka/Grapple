using UnityEngine;
using System.Collections.Generic;
using System;

public class Guns : MonoBehaviour
{
    public static Gun[] guns;

    public void Init()
    {
        guns = new Gun[50];

        int index = 0;

        for(int i=0; i<transform.childCount; i++)
        {
            var gun = transform.GetChild(i).GetComponent<Gun>();
            if(gun == null) continue;

            guns[index] = gun;
            guns[index].index = index++;
        }

        Array.Resize(ref guns, index);
    }
}