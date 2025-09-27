<%@ Page Title="Giỏ hàng" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Cart.aspx.cs"
    Inherits="DA_CS434W.Cart" %>

<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container page-cart">
        <div class="pagehead">
            <h1>Giỏ hàng của bạn</h1>
        </div>

        <div class="layout">
            <!-- Cột trái: danh sách sản phẩm -->
            <div class="card">
                <div class="card-h">Sản phẩm</div>
                <div class="card-b">
                    <asp:Panel ID="pnlEmpty" runat="server" CssClass="empty" Visible="false">
                        Chưa có sản phẩm nào trong giỏ.
                    </asp:Panel>

                    <div class="cart-list">
                        <asp:Repeater ID="rpCart" runat="server" OnItemCommand="rpCart_ItemCommand">
                            <ItemTemplate>
                                <div class="item">
                                    <div class="thumb">
                                        <asp:Image ID="imgThumb" runat="server"
                                            ImageUrl='<%# Eval("ImageUrl") %>'
                                            AlternateText="Ảnh"
                                            Style="width: 100%; height: 100%; object-fit: cover; border-radius: 10px" />
                                    </div>

                                    <div>
                                        <div class="name"><%# Eval("ProductName") %></div>
                                        <div class="meta">Mã SP: <%# Eval("ProductID") %></div>
                                    </div>

                                    <div class="price">
                                        <div class="now"><%# string.Format("{0:N0}đ", Eval("Price")) %></div>
                                    </div>

                                    <div class="qty">
                                        <asp:Button runat="server" Text="−" CommandName="Dec" CommandArgument='<%# Eval("ProductID") %>' />
                                        <asp:TextBox ID="txtQty" runat="server" Text='<%# Eval("Qty") %>' />
                                        <asp:Button runat="server" Text="+" CommandName="Inc" CommandArgument='<%# Eval("ProductID") %>' />
                                    </div>

                                    <asp:Button runat="server" CssClass="remove" Text="×"
                                        CommandName="Remove" CommandArgument='<%# Eval("ProductID") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <!-- Cột phải: tóm tắt đơn hàng -->
            <div class="card">
                <div class="card-h">Tóm tắt đơn hàng</div>
                <div class="card-b">
                    <div class="code">
                        <asp:TextBox ID="txtCoupon" runat="server" placeholder="Nhập mã giảm (VD: SALE10)" />
                        <asp:Button ID="btnApplyCoupon" runat="server" CssClass="btn" Text="Áp dụng" OnClick="btnApplyCoupon_Click" />
                    </div>

                    <div class="sum-row">
                        <span class="muted">Tạm tính</span>
                        <span>
                            <asp:Label ID="lblSubtotal" runat="server" Text="0k" /></span>
                    </div>

                    <div class="sum-row">
                        <span class="muted">Giảm giá</span>
                        <span>
                            <asp:Label ID="lblDiscount" runat="server" Text="0k" /></span>
                    </div>

                    <div class="sum-row">
                        <span class="muted">Phí vận chuyển</span>
                        <span>
                            <asp:Label ID="lblShipping" runat="server" Text="0k" /></span>
                    </div>

                    <hr />

                    <div class="sum-row" style="font-size: 18px; font-weight: 900; color: #222">
                        <span>Tổng cộng</span>
                        <span>
                            <asp:Label ID="lblGrand" runat="server" Text="0k" /></span>
                    </div>

                    <div class="mini">
                        <a class="btn outline" runat="server" href="~/Product.aspx">← Tiếp tục mua sắm</a>
                        <asp:Button ID="btnCheckout" runat="server" CssClass="btn" Text="Thanh toán" OnClick="btnCheckout_Click" />
                    </div>

                    <asp:Label ID="lblMsg" runat="server" ForeColor="#d24b62" />
                </div>
            </div>
        </div>
    </main>
</asp:Content>
