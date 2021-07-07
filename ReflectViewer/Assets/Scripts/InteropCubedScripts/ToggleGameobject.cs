using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameobject : MonoBehaviour
{
    public void ToggleGameObject()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
    }
}
