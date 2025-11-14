using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkSize : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
}
