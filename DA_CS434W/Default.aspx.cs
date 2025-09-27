using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace DA_CS434W
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-home";

            if (!IsPostBack)
            {
                LoadCategories_H();
                SyncSortDropdowns_H("featured");
                BindProducts_H();
            }
        }

        private void LoadCategories_H()
        {
            using (var conn = Connection.GetConnection())
            using (var cmd = new SqlCommand("SELECT DanhMucId, Ten FROM dbo.categories ORDER BY Ten;", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    cblCategories_H.DataSource = r;
                    cblCategories_H.DataTextField = "Ten";
                    cblCategories_H.DataValueField = "DanhMucId";
                    cblCategories_H.DataBind();
                }
            }
        }

        private void SyncSortDropdowns_H(string value)
        {
            ddlSort_H.SelectedValue = value;
            ddlSortTop_H.SelectedValue = value;
        }

        protected void btnApply_H_Click(object sender, EventArgs e)
        {
            SyncSortDropdowns_H(ddlSort_H.SelectedValue);
            BindProducts_H();
        }

        protected void ddlSortTop_H_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncSortDropdowns_H(ddlSortTop_H.SelectedValue);
            BindProducts_H();
        }

        protected void btnReset_H_Click(object sender, EventArgs e)
        {
            txtQ_H.Text = "";
            txtMin_H.Text = "";
            txtMax_H.Text = "";
            foreach (ListItem it in cblCategories_H.Items) it.Selected = false;
            SyncSortDropdowns_H("featured");
            BindProducts_H();
        }

        private void BindProducts_H()
        {
            string q = txtQ_H.Text.Trim();
            decimal? min = decimal.TryParse(txtMin_H.Text.Trim(), out var mn) ? mn : (decimal?)null;
            decimal? max = decimal.TryParse(txtMax_H.Text.Trim(), out var mx) ? mx : (decimal?)null;
            var catIds = cblCategories_H.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
            string sort = ddlSort_H.SelectedValue;

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
                rpProducts_H.DataSource = dt;
                rpProducts_H.DataBind();
                litCount_H.Text = dt.Rows.Count.ToString();
            }
        }

        protected void rpProducts_H_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var row = (DataRowView)e.Item.DataItem;
            var imgThumb = (Image)e.Item.FindControl("imgThumb_H");
            var litName = (Literal)e.Item.FindControl("litName_H");
            var litPrice = (Literal)e.Item.FindControl("litPrice_H");
            var lnkDetail = (HyperLink)e.Item.FindControl("lnkDetail_H");

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
