using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class vibemeterScript : MonoBehaviour
{
    public int maxfill;
    public int current;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tofill();
    }
    void tofill()
    {
        float fillAmount=(float)current/(float)maxfill;
        mask.fillAmount=fillAmount;
    }
}
