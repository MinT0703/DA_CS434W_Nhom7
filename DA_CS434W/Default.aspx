<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DA_CS434W.Default" %>

<asp:Content ID="c1" ContentPlaceHolderID="HeadContent" runat="server" />
<asp:Content ID="c2" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container">
        <section class="hero" id="home">
            <div class="hero-grid">
                <div class="hero-left">
                    <div class="frame-photo" role="img" aria-label="Ảnh mẫu thời trang (placeholder)"></div>
                </div>
                <div class="hero-right">
                    <span class="badge-pill">40% OFF ALL STORE</span>
                    <h1 class="title">Ultimate Sale</h1>
                    <div class="subtitle">Ưu đãi cực lớn • Mua ngay hôm nay</div>
                    <p class="desc">Siêu giảm giá đến 50% khi mua các mặt hàng thời trang như Quần Jean, Túi Tote.</p>
                    <div class="cta-row">
                        <a class="btn" href="product.html">Shop Now</a>
                        <div style="color: #7d5b5b">Miễn phí giao hàng cho đơn ≥ 499k</div>
                    </div>
                    <div style="display: grid; gap: 12px">
                        <div style="display: grid; grid-template-columns: repeat(3,1fr); gap: 12px">
                            <div class="frame-photo" style="height: 88px; aspect-ratio: auto; border-radius: 12px; box-shadow: none; background: linear-gradient(0deg,rgba(0,0,0,.45),rgba(0,0,0,.12)),linear-gradient(135deg,#d9d9d9,#f0f0f0); display: grid; place-items: center; color: #fff; font-family: 'Playfair Display',serif; font-weight: 800; letter-spacing: .08em; text-transform: uppercase">Make up</div>
                            <div class="frame-photo" style="height: 88px; aspect-ratio: auto; border-radius: 12px; box-shadow: none; background: linear-gradient(0deg,rgba(0,0,0,.45),rgba(0,0,0,.12)),linear-gradient(135deg,#d9d9d9,#f0f0f0); display: grid; place-items: center; color: #fff; font-family: 'Playfair Display',serif; font-weight: 800; letter-spacing: .08em; text-transform: uppercase">Accessories</div>
                            <div class="frame-photo" style="height: 88px; aspect-ratio: auto; border-radius: 12px; box-shadow: none; background: linear-gradient(0deg,rgba(0,0,0,.45),rgba(0,0,0,.12)),linear-gradient(135deg,#d9d9d9,#f0f0f0); display: grid; place-items: center; color: #fff; font-family: 'Playfair Display',serif; font-weight: 800; letter-spacing: .08em; text-transform: uppercase">Jeans</div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- PRODUCTS SECTION -->
        <section id="products" aria-label="Danh sách sản phẩm">
            <div class="pagehead">
                <h2>Bộ sưu tập</h2>
            </div>

            <div class="layout">
                <!-- Sidebar filters -->
                <aside>
                    <p class="aside-title">Bộ lọc</p>

                    <div class="field">
                        <label class="label" for="<%= txtQ_H.ClientID %>">Tìm kiếm</label>
                        <asp:TextBox ID="txtQ_H" runat="server" placeholder="Từ khóa (ví dụ: jeans, bag...)" />
                    </div>

                    <div class="field">
                        <div class="label">Danh mục</div>
                        <asp:CheckBoxList ID="cblCategories_H" runat="server" CssClass="checklist" RepeatLayout="Flow" />
                    </div>

                    <div class="field">
                        <div class="label">Khoảng giá (đ)</div>
                        <div class="range">
                            <asp:TextBox ID="txtMin_H" runat="server" TextMode="Number" placeholder="Min" />
                            <asp:TextBox ID="txtMax_H" runat="server" TextMode="Number" placeholder="Max" />
                        </div>
                    </div>

                    <div class="field">
                        <div class="label">Sắp xếp</div>
                        <asp:DropDownList ID="ddlSort_H" runat="server">
                            <asp:ListItem Value="featured" Text="Nổi bật" />
                            <asp:ListItem Value="price-asc" Text="Giá tăng dần" />
                            <asp:ListItem Value="price-desc" Text="Giá giảm dần" />
                            <asp:ListItem Value="name-asc" Text="Tên A → Z" />
                            <asp:ListItem Value="name-desc" Text="Tên Z → A" />
                            <asp:ListItem Value="newest" Text="Mới nhất" />
                        </asp:DropDownList>
                    </div>

                    <div class="field" style="display: flex; gap: 10px">
                        <asp:Button ID="btnApply_H" runat="server" CssClass="apply" Text="Áp dụng" OnClick="btnApply_H_Click" />
                        <asp:Button ID="btnReset_H" runat="server" CssClass="apply reset" Text="Reset" OnClick="btnReset_H_Click" CausesValidation="false" />
                    </div>
                </aside>

                <!-- Main content -->
                <section>
                    <div class="toolbar">
                        <div class="result">
                            <asp:Literal ID="litCount_H" runat="server" />
                            sản phẩm
                        </div>
                        <div class="sort">
                            Sắp xếp nhanh:
              <asp:DropDownList ID="ddlSortTop_H" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortTop_H_SelectedIndexChanged">
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
                        <asp:Repeater ID="rpProducts_H" runat="server" OnItemDataBound="rpProducts_H_ItemDataBound">
                            <ItemTemplate>
                                <div class="card">
                                    <div class="thumb">
                                        <asp:Image ID="imgThumb_H" runat="server" AlternateText="Ảnh sản phẩm"
                                            Style="width: 100%; height: 100%; object-fit: cover" />
                                    </div>
                                    <div class="card-body">
                                        <div class="title-sm">
                                            <asp:Literal ID="litName_H" runat="server" /></div>
                                        <div class="price">
                                            <div class="now">
                                                <asp:Literal ID="litPrice_H" runat="server" />
                                            </div>
                                        </div>
                                        <div class="meta">
                                            <div class="stars">★★★★☆</div>
                                            <div class="stock">Còn hàng</div>
                                        </div>
                                        <div class="cta">
                                            <asp:HyperLink ID="lnkDetail_H" runat="server" CssClass="btn-add" Text="Xem chi tiết" />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </section>
            </div>
        </section>
    </main>
</asp:Content>
