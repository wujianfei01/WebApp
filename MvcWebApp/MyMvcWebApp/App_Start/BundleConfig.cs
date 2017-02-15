using System.Web;
using System.Web.Optimization;

namespace MvcWebApp
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                       "~/Scripts/home.js"));

            //easyui
            bundles.Add(new StyleBundle("~/Content/themes/blue/css").Include("~/Content/themes/blue/easyui.css"));
            bundles.Add(new StyleBundle("~/Content/themes/gray/css").Include("~/Content/themes/gray/easyui.css"));
            bundles.Add(new StyleBundle("~/Content/themes/metro/css").Include("~/Content/themes/metro/easyui.css"));
            bundles.Add(new StyleBundle("~/Content/themes/mystyle/css").Include("~/Content/themes/mystyle/easyui.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/query/js").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.easyui.min.js",
                "~/Scripts/easyui/locale/easyui-lang-zh_CN.js",
                "~/Scripts/jBox/jquery.jBox-2.3.min.js",
                "~/Scripts/control/pagecontrol.js",
                "~/Scripts/control/querypage.js",
                "~/Scripts/control/foreditpage.js"));


            bundles.Add(new StyleBundle("~/bundles/query/css").Include("~/Scripts/jBox/Skins/GrayCool/jbox.css",
                "~/Content/themes/icon.css",
                "~/Content/themes/gray/easyui.css",
                "~/Content/controls/MyStyle.css"));

  

            // 使用 Modernizr 的开发版本进行开发和了解信息。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}