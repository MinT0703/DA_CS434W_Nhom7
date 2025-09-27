using System;
using System.Data.SqlClient;

namespace DA_CS434W
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master is DA_CS434W.Site m)
                m.BodyCssClass = "page-auth";
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string pwd = txtPassword.Text.Trim();
            string name = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(name))
            {
                lblMessage.Text = "Vui lòng nhập đầy đủ Email, Mật khẩu và Họ tên.";
                return;
            }

            try
            {
                using (SqlConnection conn = Connection.GetConnection())
                {
                    conn.Open();

                    using (var check = new SqlCommand("SELECT COUNT(1) FROM dbo.users WHERE Email=@e", conn))
                    {
                        check.Parameters.AddWithValue("@e", email);
                        int exists = (int)check.ExecuteScalar();
                        if (exists > 0)
                        {
                            lblMessage.Text = "Email đã được đăng ký.";
                            return;
                        }
                    }

                    int roleId = EnsureCustomerRole(conn);

                    using (var cmd = new SqlCommand(@"
                        INSERT INTO dbo.users(Email, MatKhauHash, HoTen, SoDienThoai, VaiTroId)
                        VALUES(@e,@p,@n,@s,@r);", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@p", pwd);
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@s", (object)phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@r", roleId);
                        cmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect("~/Login.aspx?registered=1", endResponse: false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                lblMessage.Text = "Có lỗi xảy ra khi đăng ký.";
            }
        }

        private int EnsureCustomerRole(SqlConnection conn)
        {
            using (var get = new SqlCommand(
                "SELECT TOP 1 VaiTroId FROM dbo.roles WHERE TenVaiTro = N'Khách hàng';", conn))
            {
                object val = get.ExecuteScalar();
                if (val != null && val != DBNull.Value) return Convert.ToInt32(val);
            }

            using (var ins = new SqlCommand(
                "INSERT INTO dbo.roles(TenVaiTro) VALUES (N'Khách hàng'); SELECT SCOPE_IDENTITY();", conn))
            {
                return Convert.ToInt32(Math.Round(Convert.ToDecimal(ins.ExecuteScalar())));
            }
        }
    }
}
