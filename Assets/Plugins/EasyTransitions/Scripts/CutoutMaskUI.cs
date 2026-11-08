using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Plugins.EasyTransitions.Scripts
{
    [AddComponentMenu("UI/Cutout Mask UI")]
    public class CutoutMaskUI : Image
    {
        private Material _maskMaterial;
        private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");

        public override Material materialForRendering
        {
            get
            {
                if (_maskMaterial) return _maskMaterial;
                var baseMat = base.materialForRendering;
                if (baseMat == null) return null;

                _maskMaterial = new Material(baseMat)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _maskMaterial.SetInt(StencilComp, (int)CompareFunction.NotEqual);

                return _maskMaterial;
            }
        }

        protected override void OnDestroy()
        {
            {
                if (Application.isPlaying)
                    Destroy(_maskMaterial);
                else
                    DestroyImmediate(_maskMaterial);

                _maskMaterial = null;
            }

            base.OnDestroy();
        }
    }
}