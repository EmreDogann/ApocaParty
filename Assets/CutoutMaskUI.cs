using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMaskUI : Image
{
    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
    private Material _material;
    public override Material materialForRendering
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(material);
                _material.SetInt(StencilComp, (int)CompareFunction.NotEqual);
            }

            return _material;
        }
    }
}