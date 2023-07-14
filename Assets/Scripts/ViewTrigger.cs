using MyBox;
using UI;
using UI.Views;
using UnityEngine;

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