using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Bae.Materials
{
    /// <summary>
    /// A material that uses the new Standard Shader.
    /// </summary>
    public class MWStandardMaterial : MWBaseMaterial
    {
        Material _standardMaterial;
        Material _standardCutoutMaterial;

        public MWStandardMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            _standardMaterial = new Material(Shader.Find("Standard"));
            _standardCutoutMaterial = Resources.Load<Material>("Materials/StandardCutout");
        }

        public override Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material;
            //check if the material is already cached
            if (!_existingMaterials.TryGetValue(mp, out material))
            {
                //otherwise create a new material and cache it
                if (mp.alphaBlended)
                    material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
                else if (mp.alphaTest) material = BuildMaterialTested(mp.alphaCutoff);
                else material = BuildMaterial();
                if (mp.textures.mainFilePath != null)
                {
                    material.mainTexture = _textureManager.LoadTexture(mp.textures.mainFilePath);
                    if (BaeSettings.generateNormalMap)
                    {
                        material.EnableKeyword("_NORMALMAP");
                        material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture, BaeSettings.normalGeneratorIntensity));
                    }
                }
                else material.DisableKeyword("_NORMALMAP");
                if (mp.textures.bumpFilePath != null)
                {
                    material.EnableKeyword("_NORMALMAP");
                    material.SetTexture("_NORMALMAP", _textureManager.LoadTexture(mp.textures.bumpFilePath));
                }
                _existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardMaterial);
            return material;
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            //material.SetInt("_SrcBlend", (int)sourceBlendMode);
            //material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardCutoutMaterial);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}