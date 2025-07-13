using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class ToastBehaviourBase : MonoBehaviour
    {
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _idleDuration = 1f;

        [SerializeField] private GameObject _rootGameobject;
        [Header("Assign all graphics that should fade")]
        [SerializeField] private List<Graphic> graphicsToFade;

        private List<float> _originalAlphas = new List<float>();

        private void Awake()
        {
            CacheOriginalAlphas();
            _rootGameobject.SetActive(false);
        }

        protected void ShowToast()
        {
            StopAllCoroutines();
            StartCoroutine(Display());
        }

        private void CacheOriginalAlphas()
        {
            _originalAlphas.Clear();
            foreach (var graphic in graphicsToFade)
            {
                if (graphic != null)
                {
                    _originalAlphas.Add(graphic.color.a);
                }
                else
                {
                    _originalAlphas.Add(1f);
                }
            }
        }

        private IEnumerator Display()
        {
            _rootGameobject.SetActive(true);
            float timer = 0f;
            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / _fadeDuration);
                SetAlpha(t);
                yield return null;
            }

            SetAlpha(1f);
            yield return new WaitForSeconds(_idleDuration);

            // Fade out
            timer = 0f;
            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(1f - (timer / _fadeDuration));
                SetAlpha(t);
                yield return null;
            }

            SetAlpha(0f);
            _rootGameobject.SetActive(false);
        }

        private void SetAlpha(float factor)
        {
            for (int i = 0; i < graphicsToFade.Count; i++)
            {
                var graphic = graphicsToFade[i];
                if (graphic == null) continue;

                var c = graphic.color;
                c.a = Mathf.Lerp(0f, _originalAlphas[i], factor);
                graphic.color = c;
            }
        }
    }
}
