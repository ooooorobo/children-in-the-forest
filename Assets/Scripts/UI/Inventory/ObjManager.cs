using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour {
    private static ObjManager _objManager = null;
    public Inventory inventory;

    public static ObjManager objManager
    {
        get
        {
            if (_objManager == null)
            {
                _objManager = FindObjectOfType(typeof(ObjManager)) as ObjManager;
                _objManager.inventory = _objManager.gameObject.GetComponent<Inventory>();


                if (_objManager == null)
                {
                    Debug.Log("obj매니저 없어요");
                }
            }

            return _objManager;
        }
    }
}
