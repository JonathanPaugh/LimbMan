using System;
using System.Collections;

namespace Jape
{
    public abstract partial class Element
    {
        public class ElementActivity : Activity
        {
            private Element element;

            internal ElementActivity(Element element)
            {
                this.element = element;
                SetQueue(element.CreateJobQueue());
            }

            protected override Job CreateJob(IEnumerable routine) { return element.CreateJob().Set(routine); }
            protected override Job CreateJob(Action routine) { return element.CreateJob().Set(routine); }
        }
    }
}