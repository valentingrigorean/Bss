using Foundation;
namespace Bss.iOS.Extensions
{
    public static class NumberExtensions
    {
        public static NSIndexPath  CreateRowSection(this int This,int section = 0)
        {
            return NSIndexPath.FromRowSection(This, section);
        }
    }
}
