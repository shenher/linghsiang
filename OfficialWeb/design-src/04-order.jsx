// page-order.jsx — 下單：下單流程 + LINE QRCode + LINE ID + 加好友按鈕
function PageOrder() {
  const { isMobile } = useViewport();
  const pad = isMobile ? '0 24px' : '0 48px';

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Nav active="order" />

      {/* 標題 */}
      <section style={{ padding: isMobile ? '44px 0 8px' : '80px 0 16px', textAlign: 'center' }}>
        <div style={{ maxWidth: 720, margin: '0 auto', padding: pad }}>
          <img src={LOGO.mark} alt="拎香焙室商標" style={{ width: isMobile ? 46 : 56, height: 'auto', display: 'block', margin: '0 auto 18px' }} />
          <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 16 }}>— HOW TO ORDER · 下單</div>
          <h1 style={{ fontFamily: 'var(--font-title)', fontWeight: 500, fontSize: isMobile ? 36 : 56, lineHeight: 1.15, margin: 0, color: 'var(--fg)' }}>下單，從一句<span style={{ color: 'var(--accent)' }}>LINE</span> 開始</h1>
          <p style={{ fontSize: isMobile ? 15 : 16, lineHeight: 1.9, color: 'var(--muted)', marginTop: 20 }}>
            沒有現貨、皆為接單製作。跟著下面四步，最慢 24 小時內回覆你。
          </p>
        </div>
      </section>

      {/* 下單流程 */}
      <section style={{ padding: isMobile ? '24px 0 8px' : '40px 0 16px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto', padding: pad }}>
          <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : 'repeat(4, 1fr)', gap: isMobile ? 14 : 24 }}>
            {ORDER_FLOW.map((f, i) => (
              <div key={i} style={{ padding: isMobile ? '20px 22px' : '28px 24px', background: 'var(--surface)', borderRadius: 8, border: '1px solid var(--line)', display: isMobile ? 'flex' : 'block', gap: 16, alignItems: 'flex-start' }}>
                <div style={{ fontFamily: '"Cormorant Garamond", serif', fontStyle: 'italic', fontSize: 32, color: 'var(--accent)', lineHeight: 1 }}>{f.n}</div>
                <div>
                  <h3 style={{ fontFamily: 'var(--font-title)', fontSize: 18, fontWeight: 500, margin: isMobile ? '0 0 8px' : '12px 0 10px', color: 'var(--fg)' }}>{f.title}</h3>
                  <p style={{ fontSize: 13, lineHeight: 1.8, color: 'var(--muted)', margin: 0 }}>{f.desc}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* LINE 主要下單卡 */}
      <section style={{ padding: isMobile ? '40px 0 56px' : '64px 0 96px' }}>
        <div style={{ maxWidth: 880, margin: '0 auto', padding: pad }}>
          <div style={{
            position: 'relative',
            borderRadius: 14, border: '1px solid var(--line)', background: 'var(--surface)',
            padding: isMobile ? '36px 24px 32px' : '44px',
            display: 'grid',
            gridTemplateColumns: isMobile ? '1fr' : '240px 1fr',
            gap: isMobile ? 28 : 44, alignItems: 'center',
          }}>
            <span style={{
              position: 'absolute', top: -13, left: isMobile ? 24 : 44,
              background: '#06C755', color: '#fff', fontFamily: '"DM Mono", monospace',
              fontSize: 10, letterSpacing: '0.16em', padding: '5px 12px', borderRadius: 999,
            }}>PRIMARY · 下單請走這裡</span>

            {/* QRCode */}
            <div style={{ display: 'flex', justifyContent: 'center' }}>
              <div style={{
                width: isMobile ? 220 : 240, height: isMobile ? 220 : 240,
                background: '#fff', border: '1px solid var(--line)', borderRadius: 10,
                display: 'flex', alignItems: 'center', justifyContent: 'center', overflow: 'hidden',
              }}>
                {LINE_QR_IMAGE ? (
                  <img src={LINE_QR_IMAGE} alt="LINE 加好友 QRCode" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                ) : (
                  <div style={{
                    width: '100%', height: '100%',
                    background: 'repeating-linear-gradient(45deg, transparent 0, transparent 10px, rgba(0,0,0,0.06) 10px, rgba(0,0,0,0.06) 11px), #f6f4ee',
                    display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 10,
                    color: 'rgba(0,0,0,0.55)', fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.08em', textAlign: 'center', padding: 16,
                  }}>
                    <IconLine size={30} color="#06C755" />
                    <div>LINE QRCODE</div>
                    <div style={{ fontSize: 9, opacity: 0.7 }}>請替換 LINE_QR_IMAGE</div>
                  </div>
                )}
              </div>
            </div>

            {/* 文字 + 按鈕 */}
            <div style={{ textAlign: isMobile ? 'center' : 'left' }}>
              <div style={{ display: 'inline-flex', alignItems: 'center', gap: 8, fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.16em', color: '#06C755' }}>
                <IconLine size={14} color="#06C755" /> LINE
              </div>
              <div style={{ fontFamily: 'var(--font-title)', fontSize: isMobile ? 24 : 28, fontWeight: 500, marginTop: 10, color: 'var(--fg)' }}>掃 QRCode 加好友 · 直接下單</div>
              <div style={{ fontSize: 14, color: 'var(--muted)', marginTop: 12, lineHeight: 1.8 }}>
                LINE ID：<span style={{ color: 'var(--fg)', fontFamily: '"DM Mono", monospace', fontWeight: 500 }}>{CONTACT.lineId}</span><br/>
                告訴我品項、份量與想取貨的日期即可。
              </div>
              <a href={CONTACT.lineUrl} style={{
                display: 'inline-flex', alignItems: 'center', gap: 8, marginTop: 22,
                padding: '14px 28px', borderRadius: 999, background: '#06C755', color: '#fff',
                fontSize: 15, fontWeight: 600, textDecoration: 'none',
              }}><IconLine size={17} /> 點此加入 LINE 好友 <IconArrow size={15} /></a>
            </div>
          </div>

          {/* 說明：以 LINE 為主 */}
          <div style={{
            marginTop: 20, padding: isMobile ? '18px 20px' : '20px 24px',
            borderRadius: 10, background: 'var(--accent-soft)', color: 'var(--fg)',
            display: 'flex', gap: 14, alignItems: 'flex-start',
          }}>
            <IconLeaf size={18} color="var(--accent)" />
            <div style={{ fontSize: 14, lineHeight: 1.8 }}>
              <strong>現在下單以 LINE 為主，回覆最快。</strong><br/>
              <span style={{ color: 'var(--muted)' }}>Facebook 與 Instagram 主要發布最新出爐與店休消息；如需訂購，仍請加 LINE 私訊，確認更完整也更快。</span>
            </div>
          </div>

          {/* 其他管道 */}
          <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : '1fr 1fr', gap: 14, marginTop: 16 }}>
            <a href={CONTACT.igUrl} style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '18px 22px', borderRadius: 10, border: '1px solid var(--line)', background: 'var(--surface)', color: 'var(--fg)', textDecoration: 'none' }}>
              <IconIG size={20} color="var(--accent)" />
              <div style={{ flex: 1 }}>
                <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 10, letterSpacing: '0.14em', color: 'var(--muted)' }}>INSTAGRAM</div>
                <div style={{ fontSize: 15, fontWeight: 500, marginTop: 3 }}>@{CONTACT.ig}</div>
              </div>
              <IconArrow size={15} color="var(--muted)" />
            </a>
            <a href={CONTACT.fbUrl} style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '18px 22px', borderRadius: 10, border: '1px solid var(--line)', background: 'var(--surface)', color: 'var(--fg)', textDecoration: 'none' }}>
              <IconFB size={20} color="var(--accent)" />
              <div style={{ flex: 1 }}>
                <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 10, letterSpacing: '0.14em', color: 'var(--muted)' }}>FACEBOOK</div>
                <div style={{ fontSize: 15, fontWeight: 500, marginTop: 3 }}>{CONTACT.fb}</div>
              </div>
              <IconArrow size={15} color="var(--muted)" />
            </a>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
window.PageOrder = PageOrder;
