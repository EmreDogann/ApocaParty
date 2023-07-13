namespace UI.Views
{
    public class DialogueView : View
    {
        public override void Open()
        {
            transform.parent.gameObject.SetActive(true);
            base.Open();
        }
    }
}