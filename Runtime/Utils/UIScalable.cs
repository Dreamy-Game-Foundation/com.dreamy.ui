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
        [SerializeField] private bool playIdleAnimation;
        [SerializeField] private float idleScaleMultiplier = 1.03f;
        [SerializeField, Min(0f)] private float idleDuration = 0.8f;
        [SerializeField, Min(0f)] private float idleDelay = 0.2f;
        [SerializeField] private Ease idleEase = Ease.InOutSine;

        private Tween currentTween;

        private void Reset()
        {
            originScale = transform.localScale.x;
        }

        private void OnEnable()
        {
            PlayIdle();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopCurrentTween();
            currentTween = transform
                .DOScale(originScale * pressScaleMultiplier, duration)
                .SetUpdate(true);
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
            StopCurrentTween();
            transform.localScale = Vector3.one * originScale;
        }

        private void OnDestroy()
        {
            StopCurrentTween();
        }

        private void PlayRelease()
        {
            StopCurrentTween();
            currentTween = DOTween.Sequence()
                .SetUpdate(true)
                .Append(transform.DOScale(originScale * releaseScaleMultiplier, duration))
                .Append(transform.DOScale(originScale, duration))
                .OnComplete(PlayIdle);
        }

        private void PlayIdle()
        {
            if (!playIdleAnimation || !isActiveAndEnabled)
            {
                return;
            }

            StopCurrentTween();
            currentTween = transform
                .DOScale(originScale * idleScaleMultiplier, idleDuration)
                .SetDelay(idleDelay)
                .SetEase(idleEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void StopCurrentTween()
        {
            currentTween?.Kill();
            currentTween = null;
        }
    }
}
