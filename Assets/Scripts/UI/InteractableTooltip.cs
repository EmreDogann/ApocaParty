using DG.Tweening;
using Interactions;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class InteractableTooltip : MonoBehaviour
    {
        private TextMeshProUGUI _tooltipText;

        private void Awake()
        {
            _tooltipText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            MouseInteraction.OnHover += OnHover;
        }

        private void OnDisable()
        {
            MouseInteraction.OnHover -= OnHover;
        }

        private void OnDestroy()
        {
            _tooltipText.DOKill();
        }

        public void SetTooltip(string text)
        {
            _tooltipText.DOKill();
            if (text == string.Empty)
            {
                _tooltipText.DOFade(0, 0.05f).SetUpdate(true);
                return;
            }

            _tooltipText.alpha = 0;
            _tooltipText.text = text;
            _tooltipText.ForceMeshUpdate();

            Vector2 textSize = _tooltipText.GetRenderedValues(false);

            _tooltipText.rectTransform.sizeDelta = textSize;
            _tooltipText.rectTransform.anchoredPosition = new Vector2(_tooltipText.rectTransform.anchoredPosition.x,
                -_tooltipText.rectTransform.sizeDelta.y);

            _tooltipText.DOFade(1, 0.1f).SetUpdate(true);
        }

        private void OnHover(InteractableBase interactable)
        {
            _tooltipText.DOKill();
            if (interactable == null)
            {
                _tooltipText.DOFade(0, 0.05f).SetUpdate(true);
                return;
            }

            _tooltipText.alpha = 0;
            _tooltipText.text = interactable.GetTooltipName();
            _tooltipText.ForceMeshUpdate();

            Vector2 textSize = _tooltipText.GetRenderedValues(false);

            _tooltipText.rectTransform.sizeDelta = textSize;
            _tooltipText.rectTransform.anchoredPosition = new Vector2(_tooltipText.rectTransform.anchoredPosition.x,
                -_tooltipText.rectTransform.sizeDelta.y);

            _tooltipText.DOFade(1, 0.1f).SetUpdate(true);
        }
    }
}