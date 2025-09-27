using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace DA_CS434W
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public string BodyCssClass
        {
            get => bodyTag.Attributes["class"] ?? "";
            set => bodyTag.Attributes["class"] = value ?? "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateAuthMenu();
            UpdateCartBadge();
        }

        private void UpdateCartBadge()
        {
            int count = 0;
            var d = Session["CART_ITEMS"] as Dictionary<int, int>;
            if (d != null)
            {
                foreach (var qty in d.Values) count += Math.Max(0, qty);
            }

            var badge = FindControl("cartBadge") as HtmlGenericControl;
            if (badge != null) badge.InnerText = count.ToString();
        }

        private void UpdateAuthMenu()
        {
            bool loggedIn = Session["USER_ID"] != null;
            ToggleNavLink("~/Login.aspx", !loggedIn);
            ToggleNavLink("~/Register.aspx", !loggedIn);
        }

        private void ToggleNavLink(string virtualHref, bool visible)
        {
            foreach (var a in FindAnchors(this))
            {
                try
                {
                    string hrefOfA = ResolveUrl(a.HRef ?? "");
                    string target = ResolveUrl(virtualHref);
                    if (hrefOfA.Equals(target, StringComparison.OrdinalIgnoreCase))
                        a.Visible = visible;
                }
                catch { }
            }
        }

        private System.Collections.Generic.IEnumerable<System.Web.UI.HtmlControls.HtmlAnchor> FindAnchors(System.Web.UI.Control root)
        {
            if (root is System.Web.UI.HtmlControls.HtmlAnchor ha) yield return ha;
            foreach (System.Web.UI.Control c in root.Controls)
                foreach (var child in FindAnchors(c))
                    yield return child;
        }
    }
}
