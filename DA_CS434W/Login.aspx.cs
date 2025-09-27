using System;
using System.Data.SqlClient;
using System.Drawing;

namespace DA_CS434W
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-auth";

            if (!IsPostBack && Request.QueryString["registered"] == "1")
            {
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "Đăng ký thành công! Vui lòng đăng nhập.";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string pwd = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
            {
                lblMessage.Text = "Vui lòng nhập email và mật khẩu.";
                return;
            }

            try
            {
                using (SqlConnection conn = Connection.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 NguoiDungId, HoTen, Email
                    FROM dbo.users
                    WHERE Email = @e AND MatKhauHash = @p;", conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", pwd);

                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            Session["USER_ID"] = r["NguoiDungId"];
                            Session["USER_NAME"] = r["HoTen"]?.ToString() ?? "";
                            Session["USER_EMAIL"] = r["Email"]?.ToString() ?? "";

                            Response.Redirect("~/Default.aspx", endResponse: false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                    }
                }

                lblMessage.Text = "Sai thông tin đăng nhập!";
            }
            catch
            {
                lblMessage.Text = "Có lỗi xảy ra khi đăng nhập.";
            }
        }
    }
}
