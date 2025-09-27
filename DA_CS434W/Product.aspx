<%@ Page Title="Sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="DA_CS434W.Product" %>

<asp:Content ID="c1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="c2" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container page-products">
        <!-- Page head -->
        <div class="pagehead">
            <div class="crumbs"><a runat="server" href="~/Default.aspx">Home</a> / <b>Sản phẩm</b></div>
            <h1>Bộ sưu tập</h1>
        </div>

        <div class="layout">
            <!-- Sidebar filters (server controls) -->
            <aside>
                <p class="aside-title">Bộ lọc</p>

                <div class="field">
                    <label class="label" for="<%= txtQ.ClientID %>">Tìm kiếm</label>
                    <asp:TextBox ID="txtQ" runat="server" placeholder="Từ khóa (ví dụ: jeans, bag...)" />
                </div>

                <div class="field">
                    <div class="label">Danh mục</div>
                    <asp:CheckBoxList ID="cblCategories" runat="server" CssClass="checklist" RepeatLayout="Flow" />
                </div>

                <div class="field">
                    <div class="label">Khoảng giá (đ)</div>
                    <div class="range">
                        <asp:TextBox ID="txtMin" runat="server" TextMode="Number" placeholder="Min" />
                        <asp:TextBox ID="txtMax" runat="server" TextMode="Number" placeholder="Max" />
                    </div>
                </div>

                <div class="field">
                    <div class="label">Sắp xếp</div>
                    <asp:DropDownList ID="ddlSort" runat="server">
                        <asp:ListItem Value="featured" Text="Nổi bật" />
                        <asp:ListItem Value="price-asc" Text="Giá tăng dần" />
                        <asp:ListItem Value="price-desc" Text="Giá giảm dần" />
                        <asp:ListItem Value="name-asc" Text="Tên A → Z" />
                        <asp:ListItem Value="name-desc" Text="Tên Z → A" />
                        <asp:ListItem Value="newest" Text="Mới nhất" />
                    </asp:DropDownList>
                </div>

                <div class="field" style="display: flex; gap: 10px">
                    <asp:Button ID="btnApply" runat="server" CssClass="apply" Text="Áp dụng" OnClick="btnApply_Click" />
                    <asp:Button ID="btnReset" runat="server" CssClass="apply reset" Text="Reset" OnClick="btnReset_Click" CausesValidation="false" />
                </div>
            </aside>

            <!-- Grid sản phẩm -->
            <section>
                <div class="toolbar">
                    <div class="result">
                        <asp:Literal ID="litCount" runat="server" />
                        sản phẩm
                    </div>
                    <div class="sort">
                        Sắp xếp nhanh:
            <asp:DropDownList ID="ddlSortTop" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortTop_SelectedIndexChanged">
                <asp:ListItem Value="featured" Text="Nổi bật" />
                <asp:ListItem Value="price-asc" Text="Giá tăng dần" />
                <asp:ListItem Value="price-desc" Text="Giá giảm dần" />
                <asp:ListItem Value="name-asc" Text="Tên A → Z" />
                <asp:ListItem Value="name-desc" Text="Tên Z → A" />
                <asp:ListItem Value="newest" Text="Mới nhất" />
            </asp:DropDownList>
                    </div>
                </div>

                <div class="grid">
                    <asp:Repeater ID="rpProducts" runat="server" OnItemDataBound="rpProducts_ItemDataBound">
                        <ItemTemplate>
                            <div class="card">
                                <div class="thumb">
                                    <asp:Image ID="imgThumb" runat="server" AlternateText="Ảnh sản phẩm"
                                        Style="width: 100%; height: 100%; object-fit: cover" />
                                </div>
                                <div class="card-body">
                                    <div class="title">
                                        <asp:Literal ID="litName" runat="server" />
                                    </div>
                                    <div class="price">
                                        <div class="now">
                                            <asp:Literal ID="litPrice" runat="server" />
                                        </div>
                                    </div>
                                    <div class="meta">
                                        <div class="stars">★★★★☆</div>
                                        <div class="stock">Còn hàng</div>
                                    </div>
                                    <div class="cta">
                                        <asp:HyperLink ID="lnkDetail" runat="server" CssClass="btn-add" Text="Xem chi tiết" />
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </div>
    </main>
</asp:Content>
