using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class UIShineWave : MonoBehaviour
    {
        private const string ShaderName = "Dreamy/UI/Shine Wave";
        private static readonly int ShineColorId = Shader.PropertyToID("_ShineColor");
        private static readonly int ShinePositionId = Shader.PropertyToID("_ShinePosition");
        private static readonly int ShineWidthId = Shader.PropertyToID("_ShineWidth");
        private static readonly int ShineSoftnessId = Shader.PropertyToID("_ShineSoftness");
        private static readonly int ShineDirectionId = Shader.PropertyToID("_ShineDirection");

        [SerializeField] private Graphic target;
        [SerializeField] private Shader shineShader;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField, Min(0f)] private float speed = 0.6f;
        [SerializeField, Range(0f, 1f)] private float phase;
        [SerializeField, Range(-180f, 180f)] private float rotation = 45f;
        [SerializeField] private Color shineColor = new Color(1f, 1f, 1f, 0.7f);
        [SerializeField, Range(0.01f, 1f)] private float width = 0.18f;
        [SerializeField, Range(0.001f, 1f)] private float softness = 0.12f;

        private Material originalMaterial;
        private Material runtimeMaterial;
        private bool isPlaying;
        private bool isOneShot;
        private float oneShotSpeed;

        public bool IsPlaying => isPlaying;

        private void Reset()
        {
            target = GetComponent<Graphic>();
            shineShader = Shader.Find(ShaderName);
        }

        private void OnEnable()
        {
            isPlaying = playOnEnable;
            EnsureMaterial();
            ApplyProperties();
        }

        private void OnDisable()
        {
            ReleaseMaterial();
        }

        private void OnDestroy()
        {
            ReleaseMaterial();
        }

        private void OnValidate()
        {
            speed = Mathf.Max(0f, speed);
            phase = Mathf.Clamp01(phase);
            if (isActiveAndEnabled)
            {
                EnsureMaterial();
                ApplyProperties();
            }
        }

        private void Update()
        {
            if (!isPlaying || runtimeMaterial == null) return;

            float phaseSpeed = isOneShot ? oneShotSpeed : speed;
            phase += phaseSpeed * Time.unscaledDeltaTime;
            if (phase >= 1f)
            {
                if (isOneShot)
                {
                    phase = 1f;
                    isPlaying = false;
                    isOneShot = false;
                }
                else
                {
                    phase = Mathf.Repeat(phase, 1f);
                }
            }

            ApplyProperties();
        }

        public void Play()
        {
            isOneShot = false;
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
            isOneShot = false;
        }

        public void SetPhase(float value)
        {
            phase = Mathf.Clamp01(value);
            ApplyProperties();
        }

        public void SetRotation(float value)
        {
            rotation = Mathf.Repeat(value + 180f, 360f) - 180f;
            ApplyProperties();
        }

        public void PlayWave(float duration, float waveRotation)
        {
            SetRotation(waveRotation);
            phase = 0f;
            oneShotSpeed = 1f / Mathf.Max(0.01f, duration);
            isOneShot = true;
            isPlaying = true;
            ApplyProperties();
        }

        private void EnsureMaterial()
        {
            if (runtimeMaterial != null) return;

            if (target == null)
            {
                target = GetComponent<Graphic>();
            }

            Shader shader = shineShader != null ? shineShader : Shader.Find(ShaderName);
            if (target == null || shader == null) return;

            originalMaterial = target.material;
            runtimeMaterial = new Material(shader)
            {
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
            };
            runtimeMaterial.mainTexture = target.mainTexture;
            target.material = runtimeMaterial;
        }

        private void ApplyProperties()
        {
            if (runtimeMaterial == null) return;

            runtimeMaterial.SetColor(ShineColorId, shineColor);
            runtimeMaterial.SetFloat(ShinePositionId, Mathf.Lerp(-1.3f, 1.3f, phase));
            runtimeMaterial.SetFloat(ShineWidthId, width);
            runtimeMaterial.SetFloat(ShineSoftnessId, softness);
            float radians = rotation * Mathf.Deg2Rad;
            runtimeMaterial.SetVector(
                ShineDirectionId,
                new Vector4(Mathf.Cos(radians), Mathf.Sin(radians), 0f, 0f));
            target?.SetMaterialDirty();
        }

        private void ReleaseMaterial()
        {
            if (runtimeMaterial == null) return;

            if (target != null && target.material == runtimeMaterial)
            {
                target.material = originalMaterial;
            }

            if (Application.isPlaying)
            {
                Destroy(runtimeMaterial);
            }
            else
            {
                DestroyImmediate(runtimeMaterial);
            }

            runtimeMaterial = null;
            originalMaterial = null;
        }
    }
}
