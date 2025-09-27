<%@ Page Title="Đăng nhập" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="DA_CS434W.Login" %>

<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="shell">
    <div class="card">
      <div class="brand"><div class="logo">Nessa House</div></div>
      <div class="content">
        <h1>Đăng nhập</h1>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        <div class="field">
          <label>Email</label>
          <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
        </div>
        <div class="field">
          <label>Mật khẩu</label>
          <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn" OnClick="btnLogin_Click" />
      </div>
    </div>
  </div>
</asp:Content>
