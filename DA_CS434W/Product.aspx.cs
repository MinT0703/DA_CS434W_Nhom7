using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace DA_CS434W
{
    public partial class Product : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-products";

            if (!IsPostBack)
            {
                LoadCategories();
                SyncSortDropdowns("featured");
                BindProducts();
            }
        }

        private void LoadCategories()
        {
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT DanhMucId, Ten
                FROM dbo.categories
                ORDER BY Ten;", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    cblCategories.DataSource = r;
                    cblCategories.DataTextField = "Ten";
                    cblCategories.DataValueField = "DanhMucId";
                    cblCategories.DataBind();
                }
            }
        }

        private void SyncSortDropdowns(string value)
        {
            ddlSort.SelectedValue = value;
            ddlSortTop.SelectedValue = value;
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            SyncSortDropdowns(ddlSort.SelectedValue);
            BindProducts();
        }

        protected void ddlSortTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncSortDropdowns(ddlSortTop.SelectedValue);
            BindProducts();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtQ.Text = "";
            txtMin.Text = "";
            txtMax.Text = "";
            foreach (ListItem it in cblCategories.Items) it.Selected = false;
            SyncSortDropdowns("featured");
            BindProducts();
        }

        private void BindProducts()
        {
            string q = txtQ.Text.Trim();
            decimal? min = decimal.TryParse(txtMin.Text.Trim(), out var mn) ? mn : (decimal?)null;
            decimal? max = decimal.TryParse(txtMax.Text.Trim(), out var mx) ? mx : (decimal?)null;
            var catIds = cblCategories.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
            string sort = ddlSort.SelectedValue;

            string where = "p.DangBan = 1";
            if (!string.IsNullOrEmpty(q)) where += " AND p.Ten LIKE @q";
            if (min.HasValue) where += " AND p.GiaGoc >= @min";
            if (max.HasValue) where += " AND p.GiaGoc <= @max";
            if (catIds.Any()) where += $" AND p.DanhMucId IN ({string.Join(",", catIds.Select((_, i) => "@c" + i))})";

            string orderBy;
            if (sort == "price-asc")
                orderBy = "p.GiaGoc ASC";
            else if (sort == "price-desc")
                orderBy = "p.GiaGoc DESC";
            else if (sort == "name-asc")
                orderBy = "p.Ten ASC";
            else if (sort == "name-desc")
                orderBy = "p.Ten DESC";
            else if (sort == "newest")
                orderBy = "p.NgayTao DESC, p.SanPhamId DESC";
            else
                orderBy = "p.NgayTao DESC, p.SanPhamId DESC";

            string sql = $@"
                SELECT
                    p.SanPhamId,
                    p.Ten,
                    p.GiaGoc,
                    (SELECT TOP 1 i.DuongDan
                     FROM dbo.product_images i
                     WHERE i.SanPhamId = p.SanPhamId
                     ORDER BY i.LaAnhChinh DESC, i.HinhAnhId ASC) AS ImageUrl
                FROM dbo.products p
                WHERE {where}
                ORDER BY {orderBy};";

            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(q)) cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                if (min.HasValue) cmd.Parameters.AddWithValue("@min", min.Value);
                if (max.HasValue) cmd.Parameters.AddWithValue("@max", max.Value);
                for (int i = 0; i < catIds.Length; i++)
                    cmd.Parameters.AddWithValue("@c" + i, int.Parse(catIds[i]));

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                rpProducts.DataSource = dt;
                rpProducts.DataBind();
                litCount.Text = dt.Rows.Count.ToString();
            }
        }

        protected void rpProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var row = (DataRowView)e.Item.DataItem;

            var imgThumb = (Image)e.Item.FindControl("imgThumb");
            var litName = (Literal)e.Item.FindControl("litName");
            var litPrice = (Literal)e.Item.FindControl("litPrice");
            var lnkDetail = (HyperLink)e.Item.FindControl("lnkDetail");

            int id = Convert.ToInt32(row["SanPhamId"]);
            string name = row["Ten"].ToString();
            decimal gia = Convert.ToDecimal(row["GiaGoc"]);
            string imageUrl = row["ImageUrl"] == DBNull.Value ? null : row["ImageUrl"].ToString();

            litName.Text = name;
            litPrice.Text = gia.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + "đ";
            lnkDetail.NavigateUrl = "~/ProductDetail.aspx?id=" + id;

            imgThumb.ImageUrl = !string.IsNullOrWhiteSpace(imageUrl)
                ? imageUrl
                : ResolveUrl("~/Styles/no-image.png");
        }
    }
}
