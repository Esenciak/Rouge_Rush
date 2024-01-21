using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities // jest static ¿eby móc siê do tego odwo³ywaæ
{
    
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName+" jest pusty i musi zawierac wartosc obiektu " + thisObject.ToString());
            return true;
        }
        return false;
    }


    public static bool ValidateCheckENumerableValues(Object thisObject,string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;


        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no value in object " + thisObject.name.ToString());
            error = true;
        }
        return error;

    }
}
