using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace DA_CS434W
{
    public partial class ProductDetail : System.Web.UI.Page
    {
        private int _productId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-detail";

            if (!int.TryParse(Request.QueryString["id"], out _productId) || _productId <= 0)
            {
                Response.Redirect("~/Product.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProduct();
                LoadImages();
                LoadOptions();
                LoadRelated();
            }
        }

        private void LoadProduct()
        {
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 Ten, ThuongHieu, MoTa, GiaGoc
                FROM dbo.products
                WHERE SanPhamId = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        string ten = r["Ten"].ToString();
                        decimal gia = Convert.ToDecimal(r["GiaGoc"]);
                        string brand = r["ThuongHieu"] == DBNull.Value ? "" : r["ThuongHieu"].ToString();

                        lblName.Text = lblNameCrumb.Text = ten;
                        lblPriceNow.Text = gia.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + "đ";
                        litBrand.Text = string.IsNullOrEmpty(brand) ? "—" : brand;
                        litDesc.Text = r["MoTa"] == DBNull.Value ? "—" : r["MoTa"].ToString();
                        litSku.Text = _productId.ToString();

                        // Giả lập rating & reviewCount
                        litStars.Text = "★★★★☆";
                        litReviewCount.Text = "124";
                    }
                    else
                    {
                        Response.Redirect("~/Product.aspx");
                        return;
                    }
                }
            }

            // Tính tồn kho = tổng SoLuongTon của các biến thể
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(SoLuongTon),0)
                FROM dbo.product_variants
                WHERE SanPhamId=@id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                int stock = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                lblStock.Text = stock.ToString();
            }
        }

        private void LoadImages()
        {
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                ;WITH imgs AS (
                    SELECT TOP 10 DuongDan, LaAnhChinh,
                           ROW_NUMBER() OVER(ORDER BY LaAnhChinh DESC, HinhAnhId ASC) AS rn
                    FROM dbo.product_images
                    WHERE SanPhamId=@id
                    ORDER BY LaAnhChinh DESC, HinhAnhId ASC
                )
                SELECT DuongDan, LaAnhChinh, rn FROM imgs ORDER BY rn;", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                rpThumbs.DataSource = dt;
                rpThumbs.DataBind();

                if (dt.Rows.Count > 0)
                    imgStage.ImageUrl = dt.Rows[0]["DuongDan"].ToString();
                else
                    imgStage.ImageUrl = ResolveUrl("~/Styles/no-image.png");

                imgStage.Width = Unit.Empty;
                imgStage.Height = Unit.Empty;
                imgStage.Attributes.Remove("width");
                imgStage.Attributes.Remove("height");
                imgStage.Style["max-width"] = "100%";
                imgStage.Style["max-height"] = "100%";
                imgStage.Style["width"] = "auto";
                imgStage.Style["height"] = "auto";
                imgStage.Style["object-fit"] = "contain";
                imgStage.Style["display"] = "block";
            }
        }

        private void LoadOptions()
        {
           
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT DISTINCT ISNULL(NULLIF(LTRIM(RTRIM(MauSac)), ''), N'Không xác định') AS MauSac
                FROM dbo.product_variants
                WHERE SanPhamId=@id
                ORDER BY MauSac;", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                dt.Columns.Add("ColorCss", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string colorName = row["MauSac"].ToString().ToLowerInvariant();
                    row["ColorCss"] = MapColorToCss(colorName);
                }

                rpColors.DataSource = dt;
                rpColors.DataBind();
            }

            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT DISTINCT ISNULL(NULLIF(LTRIM(RTRIM(KichCo)), ''), N'Freesize') AS KichCo
                FROM dbo.product_variants
                WHERE SanPhamId=@id
                ORDER BY KichCo;", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                rpSizes.DataSource = dt;
                rpSizes.DataBind();
            }
        }

        private void LoadRelated()
        {
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT TOP 4 SanPhamId, Ten
                FROM dbo.products
                WHERE DangBan = 1 AND SanPhamId <> @id
                ORDER BY NEWID();", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                relSec.Visible = dt.Rows.Count > 0;
                rpRelated.DataSource = dt;
                rpRelated.DataBind();
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            const string CART_KEY = "CART_ITEMS";
            var cart = Session[CART_KEY] as Dictionary<int, int>;
            if (cart == null) cart = new Dictionary<int, int>();

            int qty = 1;
            if (int.TryParse(txtQty?.Text, out var q)) qty = Math.Max(1, q);

            if (cart.ContainsKey(_productId)) cart[_productId] += qty;
            else cart[_productId] = qty;

            Session[CART_KEY] = cart;

            Response.Redirect("~/Cart.aspx", endResponse: false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private string MapColorToCss(string colorName)
        {
            switch (colorName)
            {
                case "đen": case "den": return "#222";
                case "trắng": case "trang": return "#f5f5f5";
                case "đỏ": case "do": return "#d93025";
                case "hồng": case "hong": return "#ff6f91";
                case "xanh": return "#1e88e5";
                case "vàng": case "vang": return "#f4c20d";
                case "nâu": case "nau": return "#8d6e63";
                case "xám": case "ghi": return "#9e9e9e";
                default: return "#ddd";
            }
        }
    }
}
