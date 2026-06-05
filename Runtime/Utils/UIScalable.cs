using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dreamy.UI
{
    public class UIScalable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float originScale = 1f;
        [SerializeField] private float pressScaleMultiplier = 0.9f;
        [SerializeField] private float releaseScaleMultiplier = 1.15f;
        [SerializeField] private float duration = 0.1f;

        private void Reset()
        {
            originScale = transform.localScale.x;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(originScale * pressScaleMultiplier, duration).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PlayRelease();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlayRelease();
        }

        private void OnDisable()
        {
            transform.DOKill();
            transform.localScale = Vector3.one * originScale;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private void PlayRelease()
        {
            transform.DOKill();
            DOTween.Sequence()
                .SetUpdate(true)
                .Append(transform.DOScale(originScale * releaseScaleMultiplier, duration))
                .Append(transform.DOScale(originScale, duration));
        }
    }
}
