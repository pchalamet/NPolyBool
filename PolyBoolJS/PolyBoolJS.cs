using System;
using Noesis.Javascript;

namespace PolyBoolJS
{
    public class PolyBool : IDisposable
    {
        private JavascriptContext context;

        private static string PolyBoolJs()
        {
            // return System.IO.File.ReadAllText("../../../../node_modules/polybooljs/dist/polybool.js");
            return System.IO.File.ReadAllText("../../../../polybool.js");
        }

        private JavascriptContext GetContext()
        {
            if (context == null)
            {
                context = new JavascriptContext();
                var script = PolyBoolJs();
                context.Run(script);
            }

            return context;
        }

        private global::PolyBool.Polygon Operate(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2, string selector)
        {
            var context = GetContext();
            context.SetParameter("poly1", poly1.ToJavaScript());
            context.SetParameter("poly2", poly2.ToJavaScript());

            var polybooljs = PolyBoolJs();

            var script = $"PolyBool.{selector}(poly1, poly2);";
            var poly3 = context.Run(script);
            return poly3.PolygonFromJavaScript();
        }

        public global::PolyBool.Polygon Union(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2)
        {
            return Operate(poly1, poly2, "union");
        }

        public global::PolyBool.Polygon Difference(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2)
        {
            return Operate(poly1, poly2, "difference");
        }

        public void Dispose()
        {
            using (context) { }
        }
    }
}
