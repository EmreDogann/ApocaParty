using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class vibemeterScript : MonoBehaviour
{
    public int maxfill=100;
    [Range(0,100)]public int current;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        new VibeMeterClass(maxfill, current);
    }

    // Update is called once per frame
    void Update()
    {
        tofill();
    }
    void tofill()
    {
        current=Mathf.Clamp(current,0,100);
        float fillAmount=(float)current/(float)maxfill;
        mask.fillAmount=fillAmount;
    }
}
