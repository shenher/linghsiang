// page-about.jsx — 關於：開店理念 + Google Map + 聯絡方式(IG/FB/電話/地址)
function PageAbout() {
  const { isMobile } = useViewport();
  const pad = isMobile ? '0 24px' : '0 48px';

  const contactCards = [
    { icon: <IconLine size={20} color="#06C755" />, kbl: 'LINE', main: CONTACT.lineId, sub: '下單以 LINE 為主，回覆最快', href: CONTACT.lineUrl },
    { icon: <IconIG size={20} color="var(--accent)" />, kbl: 'INSTAGRAM', main: '@' + CONTACT.ig, sub: '出爐通知與日常分享', href: CONTACT.igUrl },
    { icon: <IconFB size={20} color="var(--accent)" />, kbl: 'FACEBOOK', main: CONTACT.fb, sub: '最新消息與店休公告', href: CONTACT.fbUrl },
    { icon: <IconPhone size={20} color="var(--accent)" />, kbl: '電話', main: CONTACT.phone, sub: '營業時間來電', href: 'tel:' + CONTACT.phoneTel },
  ];

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Nav active="about" />

      {/* 開店理念 */}
      <section style={{ padding: isMobile ? '48px 0 8px' : '88px 0 16px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto', padding: pad }}>
          <img src={LOGO.mark} alt="拎香焙室商標" style={{ width: 48, height: 'auto', display: 'block', marginBottom: 20 }} />
          <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 18 }}>— ABOUT · 開店理念</div>
          <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : '1.1fr 0.9fr', gap: isMobile ? 32 : 64, alignItems: 'center' }}>
            <div>
              <h1 style={{
                fontFamily: 'var(--font-title)', fontWeight: 500,
                fontSize: isMobile ? 34 : 48, lineHeight: 1.3, margin: 0, color: 'var(--fg)',
              }}>一爐一爐<br/>慢慢烤的全素烘焙坊</h1>
              {PHILOSOPHY.map((p, i) => (
                <p key={i} style={{ fontSize: isMobile ? 15 : 16, lineHeight: 2, color: 'var(--muted)', marginTop: i === 0 ? 26 : 16 }}>{p}</p>
              ))}
              <div style={{ display: 'flex', gap: isMobile ? 24 : 36, marginTop: 32, flexWrap: 'wrap' }}>
                {[{ k: '100%', v: '全素配方' }, { k: '5 天', v: '提前預約' }, { k: '當日', v: '新鮮現烤' }].map((s, i) => (
                  <div key={i}>
                    <div style={{ fontFamily: 'var(--font-title)', fontSize: 28, fontWeight: 500, color: 'var(--accent)' }}>{s.k}</div>
                    <div style={{ fontSize: 12, color: 'var(--muted)', marginTop: 4 }}>{s.v}</div>
                  </div>
                ))}
              </div>
            </div>
            <Placeholder label="工作檯 4:5" ratio="4 / 5" radius={6} />
          </div>
        </div>
      </section>

      {/* Google Map 位置 */}
      <section style={{ padding: isMobile ? '40px 0' : '72px 0', background: 'var(--surface)', borderTop: '1px solid var(--line)', borderBottom: '1px solid var(--line)', marginTop: isMobile ? 40 : 64 }}>
        <div style={{ maxWidth: 1100, margin: '0 auto', padding: pad }}>
          <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 14 }}>— VISIT · 位置</div>
          <h2 style={{ fontFamily: 'var(--font-title)', fontWeight: 500, fontSize: isMobile ? 28 : 40, margin: '0 0 28px', color: 'var(--fg)' }}>店面位置</h2>
          <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : '1fr 1fr', gap: isMobile ? 24 : 48, alignItems: 'stretch' }}>
            <div style={{ borderRadius: 8, overflow: 'hidden', border: '1px solid var(--line)', minHeight: isMobile ? 260 : 360, background: 'var(--bg)' }}>
              <iframe
                title="Google Map"
                src={CONTACT.mapEmbed}
                style={{ width: '100%', height: '100%', minHeight: isMobile ? 260 : 360, border: 0, display: 'block' }}
                loading="lazy"
                referrerPolicy="no-referrer-when-downgrade"
              ></iframe>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
              <div style={{ display: 'flex', gap: 14, alignItems: 'flex-start' }}>
                <IconPin size={20} color="var(--accent)" />
                <div>
                  <div style={{ fontSize: isMobile ? 16 : 18, color: 'var(--fg)', fontWeight: 500 }}>{CONTACT.address}</div>
                  <div style={{ fontSize: 13, color: 'var(--muted)', marginTop: 6 }}>{CONTACT.addressNote}</div>
                  <a href={CONTACT.mapsUrl} style={{
                    display: 'inline-flex', alignItems: 'center', gap: 6, marginTop: 16,
                    padding: '10px 18px', borderRadius: 999, background: 'var(--accent)', color: '#fff',
                    fontSize: 14, fontWeight: 500, textDecoration: 'none',
                  }}>在 Google Maps 開啟 <IconArrow size={14} /></a>
                </div>
              </div>
              <div style={{ display: 'flex', gap: 14, alignItems: 'flex-start', marginTop: 28 }}>
                <IconClock size={20} color="var(--accent)" />
                <div>
                  <div style={{ fontSize: 14, color: 'var(--fg)', fontWeight: 500, marginBottom: 8 }}>營業時間</div>
                  {[{ day: '週四 ─ 週六', time: '13:00 — 19:00' }, { day: '週二・週三', time: '預約取貨' }, { day: '週日・週一', time: '休息・烘焙日' }].map((h, i) => (
                    <div key={i} style={{ display: 'flex', justifyContent: 'space-between', gap: 32, padding: '4px 0', fontSize: 13, color: 'var(--muted)' }}>
                      <span>{h.day}</span><span style={{ color: 'var(--fg)' }}>{h.time}</span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 聯絡方式 */}
      <section style={{ padding: isMobile ? '48px 0 64px' : '80px 0 96px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto', padding: pad }}>
          <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.2em', color: 'var(--accent)', marginBottom: 14 }}>— CONTACT · 聯絡方式</div>
          <h2 style={{ fontFamily: 'var(--font-title)', fontWeight: 500, fontSize: isMobile ? 28 : 40, margin: '0 0 28px', color: 'var(--fg)' }}>找得到我們</h2>
          <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : 'repeat(2, 1fr)', gap: 16 }}>
            {contactCards.map((c, i) => (
              <a key={i} href={c.href} style={{
                display: 'flex', alignItems: 'center', gap: 18,
                padding: '22px 24px', borderRadius: 8,
                border: '1px solid var(--line)', background: 'var(--surface)',
                color: 'var(--fg)', textDecoration: 'none',
              }}>
                {c.icon}
                <div style={{ flex: 1 }}>
                  <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 10, letterSpacing: '0.16em', color: 'var(--muted)' }}>{c.kbl}</div>
                  <div style={{ fontSize: 17, fontWeight: 500, marginTop: 4 }}>{c.main}</div>
                  <div style={{ fontSize: 12, color: 'var(--muted)', marginTop: 3 }}>{c.sub}</div>
                </div>
                <IconArrow size={16} color="var(--muted)" />
              </a>
            ))}
          </div>
          <div style={{ display: 'flex', gap: 12, alignItems: 'flex-start', marginTop: 22, padding: '16px 18px', borderRadius: 8, background: 'var(--accent-soft)', color: 'var(--fg)' }}>
            <IconPin size={18} color="var(--accent)" />
            <div style={{ fontSize: 14, lineHeight: 1.7 }}>
              <strong>地址</strong>：{CONTACT.address}<br/>
              <span style={{ color: 'var(--muted)', fontSize: 13 }}>僅供自取，暫不提供寄送；出發前請先 LINE 私訊確認。</span>
            </div>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
window.PageAbout = PageAbout;
