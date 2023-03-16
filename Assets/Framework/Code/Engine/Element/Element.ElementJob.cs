namespace Jape
{
    public abstract partial class Element
    {
        public class ElementJob : Job
        {
            private Element element;

            internal ElementJob(Element element) { this.element = element; }

            protected override Job Copy() { return element.CreateJob().Set(routine); }

            protected override void Dispatch() { coroutine = element.StartCoroutine(Enumeration()); }

            protected override void Recall()
            {
                if (coroutine == null) { return; }
                element.StopCoroutine(coroutine);
            }
        }
    }
}