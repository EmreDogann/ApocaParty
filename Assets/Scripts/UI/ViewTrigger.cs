using MyBox;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class ViewTrigger : MonoBehaviour
    {
        [SerializeField] private View view;

        [ButtonMethod]
        private void ShowView()
        {
            UIManager.Instance.Show(view);
        }

        [ButtonMethod]
        private void CloseView()
        {
            UIManager.Instance.Back();
        }
    }
}