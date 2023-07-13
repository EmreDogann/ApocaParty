using MyBox;
using UI.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Components.Buttons
{
    public class SwitchViewButton : MenuButton
    {
        [Separator("Switch View")]
        [SerializeField] private SwitcherMode switchMode;

        [ReadOnly(nameof(switchMode), false, SwitcherMode.Back, SwitcherMode.None)]
        [SerializeField] private View targetView;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            switch (switchMode)
            {
                case SwitcherMode.None:
                    break;
                case SwitcherMode.Back:
                    UIManager.Instance.Back();
                    break;
                case SwitcherMode.Replace:
                    UIManager.Instance.Show(targetView, false);
                    break;
                case SwitcherMode.Add:
                    UIManager.Instance.Show(targetView);
                    break;
            }
        }

        private enum SwitcherMode
        {
            None,
            Back,
            Replace,
            Add
        }
    }
}