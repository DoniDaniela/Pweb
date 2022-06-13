using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace GameStore.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.fancybox.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/popup").Include(
                      "~/Content/popup.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/fancy").Include(
                     "~/Content/jquery.fancybox.css"));

            bundles.Add(new StyleBundle("~/Content/themes/css").Include(
                        "~/Content/themes/jquery.ui.core.css",
                        "~/Content/themes/jquery.ui.resizable.css",
                        "~/Content/themes/jquery.ui.selectable.css",
                        "~/Content/themes/jquery.ui.accordion.css",
                        "~/Content/themes/jquery.ui.autocomplete.css",
                        "~/Content/themes/jquery.ui.button.css",
                        "~/Content/themes/jquery.ui.dialog.css",
                        "~/Content/themes/jquery.ui.slider.css",
                        "~/Content/themes/jquery.ui.tabs.css",
                        "~/Content/themes/jquery.ui.datepicker.css",
                        "~/Content/themes/jquery.ui.progressbar.css",
                        "~/Content/themes/jquery.ui.theme.css"));
        }
    }
}