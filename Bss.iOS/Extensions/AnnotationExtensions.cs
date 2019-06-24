using MapKit;
using ObjCRuntime;
namespace Bss.iOS.Extensions
{
    public static class AnnotationExtensions
    {
        public static bool IsUserAnnotation(this IMKAnnotation This)
        {
            if (This == null) 
                return false;
            var annotation = Runtime.GetNSObject(This.Handle) as MKUserLocation;
			return annotation != null;
        }
    }
}
