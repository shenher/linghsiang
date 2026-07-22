// page-products.jsx — 產品：分類(推薦/蛋糕/餅乾)切換。
// PC：每列 4 項、最多 2 列(8 項)。手機：一項一列，往下滑瀏覽。

// 單一產品格：滑鼠移上去時，半透明遮罩淡入、"查看更多" 按鈕由下浮出。
// 點擊整格 → 把選到的商品存進 sessionStorage 後跳至詳細頁。
function ProductCard({ it, catKey, catEn, index, isMobile }) {
  const [hover, setHover] = React.useState(false);
  const open = () => goToDetail({ catKey, index, name: it.name, size: it.size, price: it.price, image: it.image, tag: it.tag });

  return (
    <article
      onClick={open}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      style={{ cursor: 'pointer' }}
    >
      <div style={{ position: 'relative' }}>
        <div style={{
          transform: hover && !isMobile ? 'translateY(-3px)' : 'translateY(0)',
          boxShadow: hover && !isMobile ? '0 18px 36px -18px rgba(58,46,34,0.45)' : '0 0 0 rgba(0,0,0,0)',
          borderRadius: 6, transition: 'transform .35s ease, box-shadow .35s ease',
        }}>
          <ProductImage label={`${catEn} ${index + 1}`} src={it.image} ratio="1 / 1" radius={6} />
        </div>

        {/* hover 遮罩 + 查看更多 */}
        <div style={{
          position: 'absolute', inset: 0, borderRadius: 6,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          background: hover && !isMobile ? 'rgba(58,46,34,0.34)' : 'rgba(58,46,34,0)',
          transition: 'background .35s ease', pointerEvents: 'none',
        }}>
          <span style={{
            display: 'inline-flex', alignItems: 'center', gap: 8,
            padding: '11px 22px', borderRadius: 999,
            background: 'var(--surface)', color: 'var(--accent)',
            fontSize: 14, fontWeight: 600, letterSpacing: '0.02em',
            boxShadow: '0 8px 20px -8px rgba(0,0,0,0.4)',
            opacity: hover && !isMobile ? 1 : 0,
            transform: hover && !isMobile ? 'translateY(0)' : 'translateY(12px)',
            transition: 'opacity .35s ease, transform .35s ease',
          }}>查看更多 <IconArrow size={15} /></span>
        </div>

        {it.tag && (
          <span style={{
            position: 'absolute', top: 12, left: 12,
            background: 'var(--surface)', color: 'var(--accent)',
            padding: '4px 12px', borderRadius: 999,
            fontSize: 11, letterSpacing: '0.08em', fontWeight: 500,
            border: '1px solid var(--line)',
          }}>{it.tag}</span>
        )}
      </div>
      <div style={{ marginTop: 14 }}>
        <div style={{ fontFamily: 'var(--font-title)', fontSize: 18, fontWeight: 500, color: 'var(--fg)' }}>{it.name}</div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'baseline', marginTop: 8, paddingTop: 8, borderTop: '1px solid var(--line)' }}>
          <span style={{ fontSize: 14, color: 'var(--muted)' }}>{it.size}</span>
          <span style={{ fontSize: 16, fontWeight: 600, color: 'var(--accent)' }}>{it.price}</span>
        </div>
      </div>
    </article>
  );
}

function PageProducts() {
  const { isMobile } = useViewport();
  const [active, setActive] = React.useState(PRODUCT_CATEGORIES[0].key);
  const cat = PRODUCT_CATEGORIES.find((c) => c.key === active) || PRODUCT_CATEGORIES[0];
  const pad = isMobile ? '0 24px' : '0 48px';
  const items = cat.items.slice(0, 8); // 最多 2 列 × 4

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Nav active="products" />

      <section style={{ flex: 1, padding: isMobile ? '40px 0 56px' : '72px 0 96px' }}>
        <div style={{ maxWidth: 1180, margin: '0 auto', padding: pad }}>
          <img src={LOGO.mark} alt="拎香焙室商標" style={{ width: 48, height: 'auto', display: 'block', marginBottom: 18 }} />
          <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 14 }}>— MENU · 本季產品</div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', flexWrap: 'wrap', gap: 16 }}>
            <h1 style={{ fontFamily: 'var(--font-title)', fontWeight: 500, fontSize: isMobile ? 34 : 48, margin: 0, color: 'var(--fg)' }}>產品菜單</h1>
            <p style={{ maxWidth: 340, fontSize: 13, color: 'var(--muted)', margin: 0 }}>
              全素・無蛋無奶。價格依當季食材微調，最終以 LINE 私訊報價為準。
            </p>
          </div>

          {/* 分類切換 */}
          <div style={{ display: 'flex', gap: isMobile ? 8 : 12, marginTop: 32, flexWrap: 'wrap' }}>
            {PRODUCT_CATEGORIES.map((c) => {
              const on = c.key === active;
              return (
                <button key={c.key} onClick={() => setActive(c.key)} style={{
                  display: 'inline-flex', alignItems: 'baseline', gap: 8,
                  padding: isMobile ? '10px 18px' : '11px 22px', borderRadius: 999,
                  border: on ? '1px solid var(--accent)' : '1px solid var(--line)',
                  background: on ? 'var(--accent)' : 'var(--surface)',
                  color: on ? '#fff' : 'var(--fg)',
                  fontSize: isMobile ? 15 : 16, fontWeight: 500, transition: 'all .2s',
                }}>
                  {c.title}
                  <span style={{ fontFamily: '"Cormorant Garamond", serif', fontStyle: 'italic', fontSize: 13, opacity: on ? 0.85 : 0.55 }}>{c.en}</span>
                </button>
              );
            })}
          </div>
          <p style={{ fontSize: 13, color: 'var(--muted)', marginTop: 16 }}>{cat.blurb}</p>

          {/* 產品格 */}
          <div style={{
            display: 'grid',
            gridTemplateColumns: isMobile ? '1fr' : 'repeat(4, 1fr)',
            gap: isMobile ? 28 : 28,
            marginTop: isMobile ? 24 : 36,
          }}>
            {items.map((it, i) => (
              <ProductCard key={i} it={it} catKey={cat.key} catEn={cat.en} index={i} isMobile={isMobile} />
            ))}
          </div>

          {/* 下單導引 */}
          <div style={{
            marginTop: isMobile ? 40 : 56, padding: isMobile ? '24px' : '28px 32px',
            borderRadius: 10, background: 'var(--surface)', border: '1px solid var(--line)',
            display: 'flex', flexDirection: isMobile ? 'column' : 'row',
            alignItems: isMobile ? 'flex-start' : 'center', justifyContent: 'space-between', gap: 18,
          }}>
            <div>
              <div style={{ fontFamily: 'var(--font-title)', fontSize: isMobile ? 20 : 22, fontWeight: 500, color: 'var(--fg)' }}>看到喜歡的了嗎？</div>
              <div style={{ fontSize: 14, color: 'var(--muted)', marginTop: 6 }}>下單以 LINE 為主，回覆最快。告訴我品項與取貨日就好。</div>
            </div>
            <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
              <a href={CONTACT.lineUrl} style={{ display: 'inline-flex', alignItems: 'center', gap: 8, padding: '13px 24px', borderRadius: 999, background: '#06C755', color: '#fff', fontSize: 15, fontWeight: 500, textDecoration: 'none' }}><IconLine size={16} /> LINE 下單</a>
              <a href={PAGES.order.href} style={{ display: 'inline-flex', alignItems: 'center', gap: 8, padding: '13px 24px', borderRadius: 999, border: '1px solid var(--line)', background: 'var(--bg)', color: 'var(--fg)', fontSize: 15, fontWeight: 500, textDecoration: 'none' }}>下單流程 <IconArrow size={14} /></a>
            </div>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
window.PageProducts = PageProducts;
