// page-home.jsx — 首頁：精簡單屏 Hero（不含 about 以下長內容）

// 字體 Tweaks：本機已安裝時直接套用原字體；未安裝時退回 Huninn（圓體網頁字型）
const HOME_TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "titleFont": "標準字",
  "bodyFont": "標準字"
}/*EDITMODE-END*/;
const HOME_FONT_STACKS = {
  '標準字': {
    title: '"Noto Serif TC", serif',
    body: '"Noto Sans TC", "Inter", sans-serif',
  },
  '源泉圓體': {
    title: '"GenSenRounded2 TW Web", "GenSenRounded2 TW M", "GenSenRounded TW M", "源泉圓體", "Huninn", "Noto Sans TC", sans-serif',
    body: '"GenSenRounded2 TW Web", "GenSenRounded2 TW M", "GenSenRounded TW R", "源泉圓體", "Huninn", "Noto Sans TC", sans-serif',
  },
  '文鼎白玉書體': {
    title: '"AR Mochi TC", "AR Mochi", "文鼎白玉書體", "文鼎白玉书体H16C90_B", "Huninn", "Noto Sans TC", sans-serif',
    body: '"AR Mochi TC", "AR Mochi", "文鼎白玉書體", "文鼎白玉书体H16C90_B", "Huninn", "Noto Sans TC", sans-serif',
  },
};
const HOME_FONT_OPTIONS = ['標準字', '源泉圓體', '文鼎白玉書體'];

function PageHome() {
  const { isMobile } = useViewport();
  const [tw, setTweak] = useTweaks(HOME_TWEAK_DEFAULTS);
  const [shown, setShown] = React.useState(false);
  React.useEffect(() => { const t = setTimeout(() => setShown(true), 60); return () => clearTimeout(t); }, []);
  React.useEffect(() => {
    const r = document.documentElement.style;
    const tf = HOME_FONT_STACKS[tw.titleFont] || HOME_FONT_STACKS['標準字'];
    const bf = HOME_FONT_STACKS[tw.bodyFont] || HOME_FONT_STACKS['標準字'];
    r.setProperty('--font-brand', tf.title);
    r.setProperty('--font-title', tf.title);
    r.setProperty('--font-body', bf.body);
  }, [tw.titleFont, tw.bodyFont]);
  React.useEffect(() => {
    if (document.getElementById('home-anim')) return;
    const s = document.createElement('style');
    s.id = 'home-anim';
    s.textContent = `
      .hm-fade{opacity:0;transform:translateX(-44px);transition:opacity 1.1s cubic-bezier(.22,.61,.36,1),transform 1.1s cubic-bezier(.22,.61,.36,1);}
      .hm-fade.in{opacity:1;transform:translateX(0);}
      .hm-mask{display:block;overflow:hidden;}
      .hm-mask > span{display:block;transform:translateY(112%);transition:transform 1.2s cubic-bezier(.22,.61,.36,1);}
      .hm-mask.in > span{transform:translateY(0);}
      @keyframes hm-zoom{from{transform:scale(1.06)}to{transform:scale(1)}}
      .hm-hero-img{animation:hm-zoom 2.4s cubic-bezier(.22,.61,.36,1) both;}
      @media (prefers-reduced-motion: reduce){
        .hm-fade,.hm-mask > span{opacity:1;transform:none;transition:none;}
        .hm-hero-img{animation:none;}
      }`;
    document.head.appendChild(s);
  }, []);
  const d = (ms) => ({ transitionDelay: `${ms}ms` });
  const cls = `hm-fade ${shown ? 'in' : ''}`;

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Nav active="home" variant="light" />

      <section style={{
        position: 'relative', flex: 1,
        minHeight: isMobile ? '92vh' : '100vh',
        display: 'flex', alignItems: 'center', justifyContent: 'flex-start',
        textAlign: 'left', color: '#fff', overflow: 'hidden',
        padding: isMobile ? '120px 28px 64px' : '140px 96px 80px',
      }}>
        <div className="hm-hero-img" style={{
          position: 'absolute', inset: 0, zIndex: 0,
          backgroundImage: `url("${HERO_CAKE_IMAGE}")`,
          backgroundSize: 'cover', backgroundPosition: 'center',
        }} />
        <div style={{
          position: 'absolute', inset: 0, zIndex: 1,
          background: 'linear-gradient(90deg, rgba(20,14,10,0.62) 0%, rgba(20,14,10,0.34) 55%, rgba(20,14,10,0.12) 100%), linear-gradient(180deg, rgba(20,14,10,0.28) 0%, rgba(20,14,10,0.05) 45%, rgba(20,14,10,0.42) 100%)',
        }} />

        <div style={{ position: 'relative', zIndex: 2, maxWidth: 640 }}>{/* 靠左內容 */}
          <div className={cls} style={{
            ...d(0), display: 'inline-flex', alignItems: 'center', gap: 8,
            padding: '6px 14px', borderRadius: 999,
            background: 'rgba(255,255,255,0.14)', border: '1px solid rgba(255,255,255,0.28)',
            backdropFilter: 'blur(6px)', WebkitBackdropFilter: 'blur(6px)',
            fontSize: 12, letterSpacing: '0.1em', marginBottom: isMobile ? 26 : 34,
          }}>
            <IconLeaf size={13} /> 全素・接單烘焙・無蛋無奶
          </div>

          <h1 className={`hm-mask ${shown ? 'in' : ''}`} style={{
            fontFamily: 'var(--font-brand)', fontWeight: 500,
            fontSize: isMobile ? 64 : 116, letterSpacing: '0.08em', lineHeight: 1.08,
            margin: 0, textShadow: '0 2px 24px rgba(0,0,0,0.4)',
          }}><span style={{ transitionDelay: '180ms', fontSize: isMobile ? 64 : 90 }}>{BRAND.zh}</span></h1>

          <div className={cls} style={{
            ...d(360), fontFamily: '"Cormorant Garamond", serif', fontStyle: 'italic',
            fontSize: isMobile ? 17 : 22, color: 'rgba(255,255,255,0.85)',
            marginTop: 16, letterSpacing: '0.06em',
          }}>{BRAND.en}</div>

          <p className={cls} style={{
            ...d(520), maxWidth: 540, margin: isMobile ? '28px 0 0' : '36px 0 0',
            fontFamily: 'var(--font-title)', fontSize: isMobile ? 15 : 18,
            lineHeight: 1.9, color: 'rgba(255,255,255,0.92)',
          }}>{BRAND.intro}</p>

          <div className={cls} style={{
            ...d(700), display: 'flex', gap: 12, justifyContent: 'flex-start',
            flexWrap: 'wrap', marginTop: isMobile ? 32 : 42,
          }}>
            <a href={CONTACT.lineUrl} style={{
              display: 'inline-flex', alignItems: 'center', gap: 8,
              padding: '14px 26px', borderRadius: 999,
              background: '#06C755', color: '#fff', fontSize: 15, fontWeight: 500,
            }}><IconLine size={16} /> LINE 下單</a>
            <a href={PAGES.products.href} style={{
              display: 'inline-flex', alignItems: 'center', gap: 8,
              padding: '14px 26px', borderRadius: 999,
              border: '1px solid rgba(255,255,255,0.5)', background: 'rgba(255,255,255,0.08)',
              color: '#fff', fontSize: 15, fontWeight: 500,
              backdropFilter: 'blur(6px)', WebkitBackdropFilter: 'blur(6px)',
            }}>看看產品 <IconArrow size={14} /></a>
          </div>

          <div className={cls} style={{
            ...d(880), display: 'flex', gap: isMobile ? 14 : 24, justifyContent: 'flex-start',
            flexWrap: 'wrap', marginTop: isMobile ? 36 : 56,
            fontFamily: '"DM Mono", monospace', fontSize: 11,
            color: 'rgba(255,255,255,0.78)', letterSpacing: '0.14em',
          }}>
            {['提前 5 天預約', '全素無蛋無奶', '可備註過敏原', '僅自取'].map((h, i, a) => (
              <React.Fragment key={i}>
                <span>{h}</span>{i < a.length - 1 && <span style={{ opacity: .5 }}>·</span>}
              </React.Fragment>
            ))}
          </div>
        </div>
      </section>

      {typeof TweaksPanel === 'function' && (
        <TweaksPanel>
          <TweakSection label="字體" />
          <TweakRadio label="標題字體" value={tw.titleFont} options={HOME_FONT_OPTIONS}
                      onChange={(v) => setTweak('titleFont', v)} />
          <TweakRadio label="內文字體" value={tw.bodyFont} options={HOME_FONT_OPTIONS}
                      onChange={(v) => setTweak('bodyFont', v)} />
          <div style={{ fontSize: 10, lineHeight: 1.6, color: 'rgba(41,38,27,.55)' }}>
            文鼎白玉書體需本機已安裝該字型；未安裝時以粉圓體（Huninn）預覽圓體效果。
          </div>
        </TweaksPanel>
      )}
    </div>
  );
}
window.PageHome = PageHome;
