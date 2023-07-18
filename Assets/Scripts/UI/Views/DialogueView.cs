namespace UI.Views
{
    public class DialogueView : View
    {
        internal override void Open(bool beingSwapped)
        {
            transform.parent.gameObject.SetActive(true);
            base.Open(beingSwapped);
        }
    }
}