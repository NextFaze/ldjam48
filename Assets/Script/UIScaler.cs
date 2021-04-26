using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    public float offset169 = 54f;
    // Start is called before the first frame update
    void Awake()
    {
        var aspect = (float)Screen.width / Screen.height;

        if (aspect > (16f/10f))
        {
            var rectTransform = GetComponent<RectTransform>();

            rectTransform.offsetMin = new Vector2(offset169, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(-offset169, rectTransform.offsetMax.y);
        }
    }
}
