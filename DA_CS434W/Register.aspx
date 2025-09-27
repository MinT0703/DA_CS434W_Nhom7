<%@ Page Title="Đăng ký" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Register.aspx.cs"
    Inherits="DA_CS434W.Register" %>

<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="shell">
    <div class="card">
      <div class="content">
        <h1>Đăng ký</h1>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        <div class="field">
          <label>Email</label>
          <asp:TextBox ID="txtEmail" runat="server" />
        </div>
        <div class="field">
          <label>Mật khẩu</label>
          <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
        </div>
        <div class="field">
          <label>Họ tên</label>
          <asp:TextBox ID="txtFullName" runat="server" />
        </div>
        <div class="field">
          <label>SĐT</label>
          <asp:TextBox ID="txtPhone" runat="server" />
        </div>
        <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" CssClass="btn" OnClick="btnRegister_Click" />
      </div>
    </div>
  </div>
</asp:Content>
