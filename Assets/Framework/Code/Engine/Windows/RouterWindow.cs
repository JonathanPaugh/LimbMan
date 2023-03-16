namespace Jape
{
    public class RouterWindow
    {
        public static void Call(Entity entity) { Member.Static(Assemblies.FrameworkEditor, nameof(RouterWindow), nameof(Call)).Get(entity); }
    }
}