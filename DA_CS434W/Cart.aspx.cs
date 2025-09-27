using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace DA_CS434W
{
    public partial class Cart : System.Web.UI.Page
    {
        private const string CART_KEY = "CART_ITEMS";
        private const string COUPON_KEY = "APPLIED_COUPON";
        private const decimal SHIPPING_FEE = 30000m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-cart";

            if (!IsPostBack) Bind();
        }

        private Dictionary<int, int> GetCartDict()
        {
            var d = Session[CART_KEY] as Dictionary<int, int>;
            if (d == null) { d = new Dictionary<int, int>(); Session[CART_KEY] = d; }
            return d;
        }

        private DataTable BuildCartData()
        {
            var cart = GetCartDict();
            var dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("ImageUrl", typeof(string));

            if (cart.Count == 0) return dt;

            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                foreach (var kv in cart)
                {
                    using (var cmd = new SqlCommand(@"
                        SELECT p.Ten, p.GiaGoc,
                               (SELECT TOP 1 i.DuongDan
                                FROM dbo.product_images i
                                WHERE i.SanPhamId = p.SanPhamId
                                ORDER BY i.LaAnhChinh DESC, i.HinhAnhId ASC) AS ImageUrl
                        FROM dbo.products p
                        WHERE p.SanPhamId = @id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", kv.Key);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                string name = r["Ten"].ToString();
                                decimal price = Convert.ToDecimal(r["GiaGoc"]);
                                string img = r["ImageUrl"] == DBNull.Value ? ResolveUrl("~/Styles/no-image.png")
                                                                            : r["ImageUrl"].ToString();
                                int qty = kv.Value;
                                dt.Rows.Add(kv.Key, name, price, qty, price * qty, img);
                            }
                        }
                    }
                }
            }
            return dt;
        }

        private void Bind()
        {
            var dt = BuildCartData();

            // List
            pnlEmpty.Visible = dt.Rows.Count == 0;
            rpCart.DataSource = dt;
            rpCart.DataBind();

            // Totals
            decimal subtotal = 0;
            foreach (DataRow row in dt.Rows) subtotal += (decimal)row["Total"];

            // Coupon
            string couponCode = Session[COUPON_KEY] as string;
            decimal discount = 0;
            if (!string.IsNullOrEmpty(couponCode))
            {
                TryCalcCoupon(couponCode, subtotal, out discount);
                txtCoupon.Text = couponCode;
            }

            decimal shipping = dt.Rows.Count > 0 ? SHIPPING_FEE : 0m;
            decimal grand = Math.Max(0, subtotal - discount) + shipping;

            lblSubtotal.Text = ToK(subtotal);
            lblDiscount.Text = ToK(discount);
            lblShipping.Text = ToK(shipping);
            lblGrand.Text = ToK(grand);

        }

        private static string ToK(decimal vnd)
        {
            var k = Math.Round(vnd / 1000m);
            return k.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + "k";
        }

        // ============ Repeater commands ============
        protected void rpCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            var cart = GetCartDict();

            if (e.CommandName == "Remove")
            {
                cart.Remove(id);
            }
            else if (e.CommandName == "Inc" || e.CommandName == "Dec")
            {
                var txt = (TextBox)e.Item.FindControl("txtQty");
                int qty = 1;
                if (txt != null) int.TryParse(txt.Text, out qty);
                qty = Math.Max(1, qty);

                if (e.CommandName == "Inc") qty++;
                if (e.CommandName == "Dec") qty = Math.Max(1, qty - 1);

                cart[id] = qty;
            }

            Session[CART_KEY] = cart;
            Bind();
        }

        // ============ Coupon ============
        protected void btnApplyCoupon_Click(object sender, EventArgs e)
        {
            string code = (txtCoupon.Text ?? "").Trim();
            var dt = BuildCartData();
            decimal subtotal = 0;
            foreach (DataRow row in dt.Rows) subtotal += (decimal)row["Total"];

            if (subtotal <= 0)
            {
                lblMsg.Text = "Giỏ hàng đang trống.";
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                Session[COUPON_KEY] = null;
                lblMsg.Text = "Đã bỏ áp dụng mã giảm giá.";
                Bind();
                return;
            }

            if (TryCalcCoupon(code, subtotal, out var discount))
            {
                Session[COUPON_KEY] = code;
                lblMsg.Text = $"Áp dụng mã {code} thành công (−{ToK(discount)}).";
                Bind();
            }
            else
            {
                lblMsg.Text = "Mã không hợp lệ hoặc đã hết hạn.";
            }
        }

        private bool TryCalcCoupon(string code, decimal subtotal, out decimal discount)
        {
            discount = 0;
            try
            {
                using (var conn = Connection.GetConnection())
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 Kieu, GiaTri, HetHanLuc
                    FROM dbo.coupons
                    WHERE MaCode = @code;", conn))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return false;

                        int kieu = Convert.ToInt32(r["Kieu"]);
                        decimal giatri = Convert.ToDecimal(r["GiaTri"]);
                        DateTime? exp = r["HetHanLuc"] == DBNull.Value ? (DateTime?)null : (DateTime)r["HetHanLuc"];

                        if (exp.HasValue && exp.Value < DateTime.UtcNow) return false;

                        if (kieu == 1) // %
                            discount = Math.Round(subtotal * (giatri / 100m), 0);
                        else           // VND
                            discount = giatri;

                        if (discount < 0) discount = 0;
                        if (discount > subtotal) discount = subtotal;
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "Tính năng thanh toán sẽ được bổ sung.";
        }
    }
}
