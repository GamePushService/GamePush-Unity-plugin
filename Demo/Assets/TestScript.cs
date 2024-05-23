using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(GP_Device.IsMobile());
    }

    // Update is called once per frame
    void Update()
    {

    }
}