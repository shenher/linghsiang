// page-detail.jsx — 產品詳細：左圖、右資料、回產品頁按鈕。
// 參數從產品頁透過 sessionStorage 傳入（不用 querystring）。
// ── 目前為「外觀展示」：不論從哪個產品點進來，都先顯示同一份寫死的內容（DETAIL）。
//    之後可把 DETAIL 換成依 incoming 取得的真實資料來源。──
const DETAIL = {
  category: '蛋糕 · Whole Cakes',
  name: '抹茶紅豆生乳蛋糕',
  tag: '招牌',
  blurb: '宇治抹茶配自家熬煮紅豆，夾入輕盈的植物性生乳餡。全素、無蛋無奶，茶香紮實而不甜膩。',
  ingredients: ['台灣米麩', '日本宇治抹茶', '有機豆漿生乳餡', '北海道紅豆（自家熬煮）', '海藻糖', '冷壓椰子油'],
  allergens: '本產品不含蛋、奶、堅果可另外註記；製作環境含有堅果與小麥。',
  // 營養標示（每份／每 100 公克）— 目前為展示用寫死數值。
  nutrition: {
    serving: '100 公克',
    perPack: '約 4.5 份',
    rows: [
      { label: '熱量',      per: '285 大卡', per100: '285 大卡' },
      { label: '蛋白質',    per: '4.2 公克', per100: '4.2 公克' },
      { label: '脂肪',      per: '12.5 公克', per100: '12.5 公克' },
      { label: '飽和脂肪',  per: '6.1 公克', per100: '6.1 公克', indent: true },
      { label: '反式脂肪',  per: '0 公克', per100: '0 公克', indent: true },
      { label: '碳水化合物', per: '34.0 公克', per100: '34.0 公克' },
      { label: '糖',        per: '18.0 公克', per100: '18.0 公克', indent: true },
      { label: '鈉',        per: '95 毫克', per100: '95 毫克' },
    ],
  },
  sizes: [
    { label: '4 吋', serve: '2–3 人', price: 'NT$ 480' },
    { label: '6 吋', serve: '4–6 人', price: 'NT$ 680' },
    { label: '8 吋', serve: '8–10 人', price: 'NT$ 980' },
  ],
  notes: ['全素・無蛋無奶', '冷藏保存，建議 2 日內享用', '需提前 5 天於 LINE 預約'],
};

function PageDetail() {
  const { isMobile } = useViewport();
  const pad = isMobile ? '0 24px' : '0 48px';

  // 讀回上一頁傳來的選取商品（目前僅供日後接資料用；外觀展示一律顯示 DETAIL）。
  const incoming = React.useMemo(() => readSelectedProduct(), []);
  const [activeSize, setActiveSize] = React.useState(1); // 預設 6 吋
  const d = DETAIL;

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Nav active="products" />

      <section style={{ flex: 1, padding: isMobile ? '24px 0 56px' : '40px 0 96px' }}>
        <div style={{ maxWidth: 1180, margin: '0 auto', padding: pad }}>

          {/* 麵包屑 / 返回 */}
          <button onClick={() => { window.location.href = PAGES.products.href; }} style={{
            display: 'inline-flex', alignItems: 'center', gap: 8,
            background: 'none', border: 'none', padding: 0,
            color: 'var(--muted)', fontSize: 14, marginBottom: isMobile ? 20 : 28,
          }}>
            <span style={{ display: 'inline-flex', transform: 'rotate(180deg)' }}><IconArrow size={15} /></span>
            返回產品菜單
          </button>

          <div style={{
            display: 'grid',
            gridTemplateColumns: isMobile ? '1fr' : '1fr 1fr',
            gap: isMobile ? 28 : 56, alignItems: 'start',
          }}>
            {/* 左：圖片 */}
            <div style={{ position: isMobile ? 'static' : 'sticky', top: 96 }}>
              <ProductImage label="PRODUCT PHOTO" src={d.image} ratio="1 / 1" radius={10} />
              {d.tag && (
                <span style={{
                  display: 'inline-block', marginTop: 16,
                  background: 'var(--accent-soft)', color: 'var(--accent)',
                  padding: '5px 14px', borderRadius: 999,
                  fontSize: 12, letterSpacing: '0.06em', fontWeight: 600,
                }}>{d.tag}</span>
              )}
            </div>

            {/* 右：詳細資料 */}
            <div>
              <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 14 }}>— {d.category}</div>
              <h1 style={{ fontFamily: 'var(--font-title)', fontWeight: 500, fontSize: isMobile ? 32 : 44, lineHeight: 1.18, margin: 0, color: 'var(--fg)' }}>{d.name}</h1>
              <p style={{ fontSize: isMobile ? 15 : 16, lineHeight: 1.9, color: 'var(--muted)', marginTop: 18 }}>{d.blurb}</p>

              {/* 尺寸 / 價格 */}
              <div style={{ marginTop: 32 }}>
                <SectionLabel>尺寸與價格</SectionLabel>
                <div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginTop: 14 }}>
                  {d.sizes.map((s, i) => {
                    const on = i === activeSize;
                    return (
                      <button key={i} onClick={() => setActiveSize(i)} style={{
                        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                        padding: isMobile ? '14px 18px' : '16px 22px', borderRadius: 10,
                        border: on ? '1.5px solid var(--accent)' : '1px solid var(--line)',
                        background: on ? 'var(--accent-soft)' : 'var(--surface)',
                        textAlign: 'left', transition: 'all .2s', width: '100%',
                      }}>
                        <span style={{ display: 'flex', alignItems: 'baseline', gap: 12 }}>
                          <span style={{ fontFamily: 'var(--font-title)', fontSize: 19, fontWeight: 500, color: 'var(--fg)' }}>{s.label}</span>
                          <span style={{ fontSize: 13, color: 'var(--muted)' }}>{s.serve}</span>
                        </span>
                        <span style={{ fontSize: 17, fontWeight: 700, color: 'var(--accent)' }}>{s.price}</span>
                      </button>
                    );
                  })}
                </div>
              </div>

              {/* 成分 + 熱量 */}
              <div style={{ marginTop: 34 }}>
                <SectionLabel>成分</SectionLabel>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8, marginTop: 14 }}>
                  {d.ingredients.map((g, i) => (
                    <span key={i} style={{
                      padding: '7px 14px', borderRadius: 999,
                      border: '1px solid var(--line)', background: 'var(--surface)',
                      fontSize: 13.5, color: 'var(--fg)',
                    }}>{g}</span>
                  ))}
                </div>
                <p style={{ fontSize: 12.5, lineHeight: 1.7, color: 'var(--muted)', marginTop: 14 }}>{d.allergens}</p>
              </div>

              <div style={{ marginTop: 34 }}>
                <SectionLabel>營養標示</SectionLabel>
                <NutritionTable n={d.nutrition} />
              </div>

              {/* 備註 */}
              <ul style={{ listStyle: 'none', padding: 0, margin: '26px 0 0', display: 'flex', flexDirection: 'column', gap: 9 }}>
                {d.notes.map((n, i) => (
                  <li key={i} style={{ display: 'flex', alignItems: 'center', gap: 10, fontSize: 13.5, color: 'var(--muted)' }}>
                    <span style={{ color: 'var(--accent)', display: 'inline-flex' }}><IconLeaf size={15} /></span>{n}
                  </li>
                ))}
              </ul>

              {/* CTA */}
              <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', marginTop: 32 }}>
                <a href={CONTACT.lineUrl} style={{ display: 'inline-flex', alignItems: 'center', gap: 8, padding: '14px 26px', borderRadius: 999, background: '#06C755', color: '#fff', fontSize: 15, fontWeight: 500, textDecoration: 'none' }}><IconLine size={16} /> LINE 詢問下單</a>
                <button onClick={() => { window.location.href = PAGES.products.href; }} style={{ display: 'inline-flex', alignItems: 'center', gap: 8, padding: '14px 26px', borderRadius: 999, border: '1px solid var(--line)', background: 'var(--bg)', color: 'var(--fg)', fontSize: 15, fontWeight: 500 }}>
                  <span style={{ display: 'inline-flex', transform: 'rotate(180deg)' }}><IconArrow size={14} /></span>
                  回產品頁
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}

function SectionLabel({ children }) {
  return (
    <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.16em', color: 'var(--muted)', textTransform: 'uppercase', borderBottom: '1px solid var(--line)', paddingBottom: 10 }}>{children}</div>
  );
}

// 營養標示表：依台灣食品標示格式（每份 / 每 100 公克）。
function NutritionTable({ n }) {
  const cell = { padding: '9px 14px', fontSize: 14, color: 'var(--fg)' };
  const numCell = { ...cell, textAlign: 'right', fontVariantNumeric: 'tabular-nums', color: 'var(--muted)' };
  const head = { ...cell, fontWeight: 600, color: 'var(--fg)' };
  return (
    <div style={{ marginTop: 14, border: '1px solid var(--line)', borderRadius: 10, overflow: 'hidden', background: 'var(--surface)' }}>
      {/* 份量資訊 */}
      <div style={{ padding: '14px 14px 12px', borderBottom: '2px solid var(--fg)' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 14, color: 'var(--fg)' }}>
          <span>每一份量</span><span style={{ color: 'var(--muted)' }}>{n.serving}</span>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 14, color: 'var(--fg)', marginTop: 6 }}>
          <span>本包裝含</span><span style={{ color: 'var(--muted)' }}>{n.perPack}</span>
        </div>
      </div>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ borderBottom: '1px solid var(--line)' }}>
            <th style={{ ...cell, textAlign: 'left' }}></th>
            <th style={{ ...head, textAlign: 'right' }}>每份</th>
            <th style={{ ...head, textAlign: 'right' }}>每 100 公克</th>
          </tr>
        </thead>
        <tbody>
          {n.rows.map((r, i) => (
            <tr key={i} style={{ borderBottom: i < n.rows.length - 1 ? '1px solid var(--line)' : 'none' }}>
              <td style={{ ...cell, paddingLeft: r.indent ? 30 : 14, color: r.indent ? 'var(--muted)' : 'var(--fg)', fontWeight: r.indent ? 400 : 500 }}>{r.label}</td>
              <td style={numCell}>{r.per}</td>
              <td style={numCell}>{r.per100}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

window.PageDetail = PageDetail;
