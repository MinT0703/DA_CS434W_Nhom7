<%@ Page Title="Chi tiết sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetail.aspx.cs" Inherits="DA_CS434W.ProductDetail" %>

<asp:Content ID="c1" ContentPlaceHolderID="HeadContent" runat="server" />
<asp:Content ID="c2" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container page-detail">
        <div class="crumbs">
            <a runat="server" href="~/Default.aspx">Home</a> /
      <a runat="server" href="~/Product.aspx">Sản phẩm</a> /
      <b>
          <asp:Label ID="lblNameCrumb" runat="server" Text="..." /></b>
        </div>

        <section class="wrap">
            <!-- Gallery -->
            <div class="gallery">
                <div class="stage" id="stage">
                    <asp:Image ID="imgStage" runat="server" CssClass="stage-img" AlternateText="Ảnh sản phẩm" />
                    <div class="off" id="Div1" runat="server" visible="false">-25%</div>
                </div>

                <div class="thumbs" id="thumbs">
                    <asp:Repeater ID="rpThumbs" runat="server">
                        <ItemTemplate>
                            <div class="thumb">
                                <img src="<%# Eval("DuongDan") %>" alt="thumb" style="width: 100%; height: 100%; object-fit: cover" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <!-- Content -->
            <div class="content">
                <h1 class="title" id="p-name">
                    <asp:Label ID="lblName" runat="server" Text="Tên sản phẩm" /></h1>

                <div class="rating" id="rating">
                    <span id="stars">
                        <asp:Literal ID="litStars" runat="server" /></span>
                    <small>(<asp:Literal ID="litReviewCount" runat="server" />
                        đánh giá)</small>
                </div>

                <div class="price">
                    <div class="now" id="p-price">
                        <asp:Label ID="lblPriceNow" runat="server" Text="0đ" />
                    </div>
                    <span class="stock" id="p-stock">Còn:
                        <asp:Label ID="lblStock" runat="server" Text="0" /></span>
                </div>

                <div class="opt">
                    <div class="label">Màu sắc</div>
                    <div class="colors" id="color-list">
                        <asp:Repeater ID="rpColors" runat="server">
                            <ItemTemplate>
                                <button type="button" class="color" style="background: <%# Eval("ColorCss") %>;"
                                    title="<%# Eval("MauSac") %>">
                                </button>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="opt">
                    <div class="label">Kích cỡ</div>
                    <div class="sizes" id="size-list">
                        <asp:Repeater ID="rpSizes" runat="server">
                            <ItemTemplate>
                                <button type="button" class="size"><%# Eval("KichCo") %></button>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="qty-row">
                    <div class="qty">
                        <button type="button" id="minus" onclick="var q=document.getElementById('<%= txtQty.ClientID %>'); q.value=Math.max(1,parseInt(q.value||'1')-1)">−</button>
                        <asp:TextBox ID="txtQty" runat="server" Text="1" TextMode="Number" min="1" CssClass="qty-input" />
                        <button type="button" id="plus" onclick="var q=document.getElementById('<%= txtQty.ClientID %>'); q.value=parseInt(q.value||'1')+1">+</button>
                    </div>
                    <asp:Button ID="btnAddToCart" runat="server" CssClass="btn" Text="Add to cart" OnClick="btnAddToCart_Click" />
                </div>

                <!-- Tabs -->
                <div class="tabs">
                    <div class="tab-nav">
                        <button type="button" class="tab-btn active" onclick="showTab('desc')">Mô tả</button>
                        <button type="button" class="tab-btn" onclick="showTab('spec')">Thông số</button>
                    </div>
                    <div class="tab-pane active" id="tab-desc">
                        <asp:Literal ID="litDesc" runat="server" />
                    </div>
                    <div class="tab-pane" id="tab-spec">
                        <table class="spec">
                            <tr>
                                <td>Thương hiệu</td>
                                <td>
                                    <asp:Literal ID="litBrand" runat="server" /></td>
                            </tr>
                            <tr>
                                <td>Mã SP</td>
                                <td>
                                    <asp:Literal ID="litSku" runat="server" /></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </section>

        <section class="rel" runat="server" id="relSec" visible="false">
            <h3 style="font-family: 'Playfair Display',serif; margin: 0 0 10px; color: #222">Sản phẩm liên quan</h3>
            <div class="grid" id="related">
                <asp:Repeater ID="rpRelated" runat="server">
                    <ItemTemplate>
                        <div class="card">
                            <div class="thumb-rel"></div>
                            <div class="card-b">
                                <div class="title-sm"><%# Eval("Ten") %></div>
                                <a class="btn-sm" href='<%# "~/ProductDetail.aspx?id=" + Eval("SanPhamId") %>'>Xem</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </section>
    </main>

    <script>
        (function () {
            const img = document.getElementById('<%= imgStage.ClientID %>');
            if (!img) return;

            function decideFit() {
                // Nếu ảnh rất cao/hẹp hoặc rất rộng → dùng cover cho đỡ “lọt thỏm”
                const w = img.naturalWidth || img.width;
                const h = img.naturalHeight || img.height;
                if (!w || !h) return;

                const ratio = h / w; // > 2 là ảnh rất cao; < 0.5 là rất ngang
                if (ratio > 2 || ratio < 0.5) {
                    img.classList.add('cover');
                } else {
                    img.classList.remove('cover');
                }
            }

            if (img.complete) decideFit();
            else img.addEventListener('load', decideFit);
        })();
    </script>

</asp:Content>
