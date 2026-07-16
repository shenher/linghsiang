// site-shared.jsx — 拎香焙室 多頁網站共用模組
// 資料、圖示、共用元件（Nav / Footer / ProductImage）、響應式 hook。
// 所有頁面 (首頁 / 關於 / 產品 / 下單) 都載入這支檔案。

// ─── 可替換素材 ───────────────────────────────────────────────────────────
// 把以下網址換成你自己的照片 / QRCode 即可。留空字串會顯示提示用佔位圖。
const HERO_CAKE_IMAGE = 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=2400&q=80';
const LINE_QR_IMAGE   = '';                                   // LINE 加好友 QRCode (PNG)
// ─────────────────────────────────────────────────────────────────────────

// 商標圖檔（已去除透明邊距）
const LOGO = {
  mark: '/Pic/logo/logo-mark.png' /* base64 已解碼落地 */,
  hGold: '/Pic/logo/logo-h-gold.png' /* base64 已解碼落地 */,
  hWhite: '/Pic/logo/logo-h-white.png' /* base64 已解碼落地 */,
  vGold: '/Pic/logo/logo-v-gold.png' /* base64 已解碼落地 */,
};

const BRAND = {
  zh: '拎香焙室',
  en: 'Ling Hsiang Bakery',
  tagline: '全素・接單烘焙・小爐慢做',
  intro: '以米麩、堅果與當令果物，一爐一爐慢慢烘。沒有現貨，每一份都是收到訂單後才為你動手做的。',
  established: 'EST · 2024',
};

const CONTACT = {
  phone: '0912-345-678',
  phoneTel: '+886912345678',
  address: '台北市大安區○○路 00 號',
  addressNote: '（實際地址請以 LINE 私訊或 Google 地圖為準）',
  ig: 'linghsiang.bakery',
  igUrl: 'https://www.instagram.com/linghsiang.bakery',
  fb: 'linghsiangbakery',
  fbUrl: 'https://www.facebook.com/linghsiangbakery/',
  lineId: '@linghsiang',
  lineUrl: 'https://line.me/R/ti/p/@linghsiang',
  mapsUrl: 'https://maps.app.goo.gl/4T3ousv4cYA8TWBH7',
  mapEmbed: 'https://maps.google.com/maps?q=%E5%8F%B0%E5%8C%97%E5%B8%82%E5%A4%A7%E5%AE%89%E5%8D%80&t=&z=15&ie=UTF8&iwloc=&output=embed',
};

// 開店理念（關於頁）
const PHILOSOPHY = [
  '拎香焙室是一間以「全素」為核心的小烘焙坊。我們相信好吃的甜點不一定要靠蛋與奶——以台灣米、堅果與當令果物，也能做出溫潤紮實的味道。',
  '我們不囤現貨。每一份蛋糕、餅乾，都是收到訂單後才開始備料、進爐，讓你拿到的永遠是最新鮮的那一爐。',
  '希望吃素的人、對蛋奶過敏的人，或只是想吃得單純一點的你，都能在這裡輕鬆享用一塊像樣的甜點。',
];

// 下單流程（下單頁）— 以 LINE 為主
const ORDER_FLOW = [
  { n: '01', title: '加入 LINE 好友', desc: '掃描下方 QRCode 或點按鈕，加入官方 LINE。' },
  { n: '02', title: '私訊告知需求', desc: '告訴我品項、份量與想取貨的日期，請提前 5 天預約。' },
  { n: '03', title: '確認與付訂', desc: '一起確認口味與取貨日，匯款訂金後正式排入烘焙排程。' },
  { n: '04', title: '到店取貨', desc: '完成後 LINE 通知，到店自取，當日新鮮最好吃。' },
];

// 產品分類：推薦 / 蛋糕 / 餅乾。每項皆有 size 與 price，可放真實圖片 (image)。
const PRODUCT_CATEGORIES = [
  {
    key: 'pick', title: '推薦', en: 'Our Picks',
    blurb: '不知道從哪開始？這幾款最受歡迎。',
    items: [
      { name: '抹茶紅豆生乳蛋糕', size: '6 吋', price: 'NT$ 680', tag: '招牌', image: '' },
      { name: '焦糖海鹽巧克力塔', size: '單顆', price: 'NT$ 110', tag: null, image: '' },
      { name: '伯爵奶茶餅乾', size: '6 入', price: 'NT$ 180', tag: null, image: '' },
      { name: '莓果優格生乳酪', size: '6 吋', price: 'NT$ 720', tag: '季節', image: '' },
      { name: '黑芝麻米吐司', size: '一條', price: 'NT$ 240', tag: null, image: '' },
      { name: '檸檬糖霜餅乾', size: '6 入', price: 'NT$ 160', tag: null, image: '' },
      { name: '桂圓核桃磅蛋糕', size: '6 吋', price: 'NT$ 580', tag: null, image: '' },
      { name: '抹茶夏威夷豆餅乾', size: '6 入', price: 'NT$ 190', tag: '熱賣', image: '' },
    ],
  },
  {
    key: 'cake', title: '蛋糕', en: 'Whole Cakes',
    blurb: '4 吋至 6 吋手作蛋糕，提前 5 天預約。',
    items: [
      { name: '抹茶紅豆生乳蛋糕', size: '4 / 6 吋', price: 'NT$ 680 起', tag: '招牌', image: '' },
      { name: '巧克力堅果磅蛋糕', size: '6 吋', price: 'NT$ 580', tag: null, image: '' },
      { name: '香草檸檬戚風', size: '6 吋', price: 'NT$ 520', tag: null, image: '' },
      { name: '莓果優格生乳酪', size: '4 / 6 吋', price: 'NT$ 720 起', tag: '季節', image: '' },
      { name: '伯爵奶茶戚風', size: '6 吋', price: 'NT$ 560', tag: null, image: '' },
      { name: '桂圓核桃磅蛋糕', size: '6 吋', price: 'NT$ 580', tag: null, image: '' },
      { name: '焦糖蘋果蛋糕', size: '6 吋', price: 'NT$ 620', tag: '季節', image: '' },
      { name: '黑糖薑味蛋糕', size: '6 吋', price: 'NT$ 540', tag: null, image: '' },
    ],
  },
  {
    key: 'cookie', title: '餅乾', en: 'Cookies',
    blurb: '小盒裝・適合自用或送禮，常溫可放數日。',
    items: [
      { name: '伯爵奶茶餅乾', size: '6 入', price: 'NT$ 180', tag: '熱賣', image: '' },
      { name: '檸檬糖霜餅乾', size: '6 入', price: 'NT$ 160', tag: null, image: '' },
      { name: '抹茶夏威夷豆餅乾', size: '6 入', price: 'NT$ 190', tag: null, image: '' },
      { name: '黑芝麻杏仁餅乾', size: '6 入', price: 'NT$ 170', tag: null, image: '' },
      { name: '蔓越莓燕麥餅乾', size: '6 入', price: 'NT$ 160', tag: null, image: '' },
      { name: '可可榛果餅乾', size: '6 入', price: 'NT$ 185', tag: null, image: '' },
      { name: '海鹽巧克力餅乾', size: '6 入', price: 'NT$ 185', tag: '季節', image: '' },
      { name: '椰香雪球餅乾', size: '8 入', price: 'NT$ 150', tag: null, image: '' },
    ],
  },
];

// ─── 響應式 hook ─────────────────────────────────────────────────────────
const MOBILE_BP = 760;
function useViewport() {
  const [w, setW] = React.useState(
    typeof window !== 'undefined' ? window.innerWidth : 1280
  );
  React.useEffect(() => {
    const cb = () => setW(window.innerWidth);
    window.addEventListener('resize', cb);
    return () => window.removeEventListener('resize', cb);
  }, []);
  return { w, isMobile: w < MOBILE_BP };
}

// ─── 圖示 ────────────────────────────────────────────────────────────────
const IconLine = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill={color}>
    <path d="M12 3C6.5 3 2 6.6 2 11c0 4 3.6 7.3 8.5 7.9.3.1.8.2.9.5.1.3.1.7 0 1l-.1.9c0 .3-.2 1 .9.5s5.7-3.4 7.7-5.8C21.3 14.4 22 12.8 22 11c0-4.4-4.5-8-10-8zM8 13.4H6c-.2 0-.3-.1-.3-.3v-4c0-.2.1-.3.3-.3s.3.1.3.3v3.7H8c.2 0 .3.1.3.3s-.1.3-.3.3zm1.4-.3c0 .2-.1.3-.3.3s-.3-.1-.3-.3v-4c0-.2.1-.3.3-.3s.3.1.3.3v4zm4.5 0c0 .1-.1.3-.2.3h-.4l-2-2.7v2.4c0 .2-.1.3-.3.3s-.3-.1-.3-.3v-4c0-.1.1-.3.2-.3h.4l2 2.7V9.1c0-.2.1-.3.3-.3s.3.1.3.3v4zm3.1-2.3c.2 0 .3.1.3.3s-.1.3-.3.3h-1.6v1.1H17c.2 0 .3.1.3.3s-.1.3-.3.3h-1.9c-.2 0-.3-.1-.3-.3V9.1c0-.2.1-.3.3-.3H17c.2 0 .3.1.3.3s-.1.3-.3.3h-1.6v1.1H17z"/>
  </svg>
);
const IconIG = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <rect x="3" y="3" width="18" height="18" rx="5" /><circle cx="12" cy="12" r="4" /><circle cx="17.5" cy="6.5" r="0.8" fill={color} />
  </svg>
);
const IconFB = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill={color}><path d="M14 9V7.5c0-.7.3-1 1-1h2V3h-3c-2.3 0-4 1.5-4 4v2H7v3.5h3V21h4v-8.5h2.7l.3-3.5H14z" /></svg>
);
const IconPin = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <path d="M12 21s-7-7-7-12a7 7 0 0114 0c0 5-7 12-7 12z" /><circle cx="12" cy="9" r="2.5" />
  </svg>
);
const IconPhone = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <path d="M5 4h4l2 5-2.5 1.5a11 11 0 005 5L15 13l5 2v4a2 2 0 01-2 2A16 16 0 013 6a2 2 0 012-2z" />
  </svg>
);
const IconClock = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" />
  </svg>
);
const IconLeaf = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <path d="M5 19c0-9 7-14 15-14 0 9-5 15-14 15-1 0-1 0-1-1z" /><path d="M5 19c4-4 7-6 11-7" />
  </svg>
);
const IconArrow = ({ size = 16, color = 'currentColor' }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <path d="M5 12h14M13 6l6 6-6 6" />
  </svg>
);

// ─── 共用元件 ────────────────────────────────────────────────────────────
function Placeholder({ label, ratio = '1 / 1', radius = 0, style = {} }) {
  return (
    <div className="ph" style={{ aspectRatio: ratio, borderRadius: radius, width: '100%', ...style }}>
      <span className="ph-label">{label}</span>
    </div>
  );
}

function ProductImage({ label, src, ratio = '1 / 1', radius = 4 }) {
  if (src) {
    return (
      <div style={{ aspectRatio: ratio, borderRadius: radius, width: '100%', overflow: 'hidden' }}>
        <img src={src} alt={label} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} />
      </div>
    );
  }
  return <Placeholder label={label} ratio={ratio} radius={radius} />;
}

const PAGES = {
  home:     { label: '首頁', href: '1-首頁.html' },
  about:    { label: '關於', href: '2-關於.html' },
  products: { label: '產品', href: '3-產品.html' },
  order:    { label: '下單', href: '4-下單.html' },
};

// 產品詳細頁（非導覽項目，不放進 Nav / Footer）。
const DETAIL_HREF = '5-產品詳細.html';
const DETAIL_STORE_KEY = 'lh.selectedProduct';

// 從產品頁點某一項時呼叫：把選到的商品資料存進 sessionStorage（不用 querystring），
// 再導向詳細頁。詳細頁讀回這份資料即可（目前外觀為寫死內容，之後可換成真實資料來源）。
function goToDetail(payload) {
  try { sessionStorage.setItem(DETAIL_STORE_KEY, JSON.stringify(payload || {})); } catch (e) {}
  window.location.href = DETAIL_HREF;
}
function readSelectedProduct() {
  try { return JSON.parse(sessionStorage.getItem(DETAIL_STORE_KEY) || 'null'); } catch (e) { return null; }
}

// 導覽列。variant='light' 用於首頁（疊在深色 Hero 上，白字）；'solid' 用於內頁。
function Nav({ active, variant = 'solid' }) {
  const { isMobile } = useViewport();
  const light = variant === 'light';
  const links = ['about', 'products', 'order'];

  const headerStyle = light
    ? { position: 'absolute', top: 0, left: 0, right: 0, zIndex: 20, color: '#fff', background: 'transparent' }
    : { position: 'sticky', top: 0, zIndex: 20, color: 'var(--fg)',
        background: 'color-mix(in srgb, var(--surface) 88%, transparent)',
        backdropFilter: 'blur(10px)', WebkitBackdropFilter: 'blur(10px)',
        borderBottom: '1px solid var(--line)' };

  return (
    <header style={{
      ...headerStyle,
      display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      padding: isMobile ? '14px 20px' : '20px 48px',
    }}>
      <a href={PAGES.home.href} style={{ display: 'flex', alignItems: 'center', textDecoration: 'none' }}>
        <img
          src={light ? LOGO.hWhite : LOGO.hGold}
          alt={BRAND.zh + ' ' + BRAND.en}
          style={{
            height: isMobile ? 24 : 34, width: 'auto', display: 'block',
            filter: light ? 'drop-shadow(0 1px 10px rgba(0,0,0,0.4))' : 'none',
          }}
        />
      </a>

      <nav style={{ display: 'flex', gap: isMobile ? 22 : 36, alignItems: 'center' }}>
        {links.map((k) => {
          const isActive = active === k;
          const base = light ? 'rgba(255,255,255,0.85)' : 'var(--muted)';
          const on = light ? '#fff' : 'var(--accent)';
          return (
            <a key={k} href={PAGES[k].href} style={{
              fontFamily: 'var(--font-body)',
              fontSize: isMobile ? 15 : 15,
              fontWeight: isActive ? 600 : 400,
              color: isActive ? on : base,
              textDecoration: 'none',
              textShadow: light ? '0 1px 8px rgba(0,0,0,0.35)' : 'none',
              paddingBottom: 3,
              borderBottom: isActive ? `2px solid ${on}` : '2px solid transparent',
            }}>{PAGES[k].label}</a>
          );
        })}
      </nav>
    </header>
  );
}

function Footer() {
  const { isMobile } = useViewport();
  return (
    <footer style={{
      borderTop: '1px solid var(--line)', background: 'var(--bg)',
      padding: isMobile ? '32px 24px' : '40px 48px',
      display: 'flex', flexDirection: isMobile ? 'column' : 'row',
      gap: isMobile ? 18 : 24,
      justifyContent: 'space-between', alignItems: isMobile ? 'flex-start' : 'center',
      color: 'var(--muted)',
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
        <img src={LOGO.mark} alt="" style={{ width: 44, height: 44, display: 'block' }} />
        <div>
        <div style={{ fontFamily: 'var(--font-brand)', fontSize: 18, fontWeight: 600, color: 'var(--fg)' }}>{BRAND.zh}</div>
        <div style={{ fontFamily: '"DM Mono", monospace', fontSize: 11, letterSpacing: '0.1em', marginTop: 6 }}>
          {BRAND.established} · 全素・接單烘焙
        </div>
        </div>
      </div>
      <nav style={{ display: 'flex', gap: 20, flexWrap: 'wrap' }}>
        {Object.keys(PAGES).map((k) => (
          <a key={k} href={PAGES[k].href} style={{ fontSize: 13, color: 'var(--muted)', textDecoration: 'none' }}>{PAGES[k].label}</a>
        ))}
      </nav>
      <div style={{ display: 'flex', gap: 14, alignItems: 'center' }}>
        <a href={CONTACT.igUrl} aria-label="Instagram" style={{ color: 'var(--muted)' }}><IconIG size={18} /></a>
        <a href={CONTACT.fbUrl} aria-label="Facebook" style={{ color: 'var(--muted)' }}><IconFB size={18} /></a>
        <a href={CONTACT.lineUrl} aria-label="LINE" style={{ color: 'var(--muted)' }}><IconLine size={18} /></a>
      </div>
    </footer>
  );
}

// 渲染包裝：設定 body 底色並掛載頁面
function mountPage(PageComponent) {
  React.useEffect && null;
  const root = ReactDOM.createRoot(document.getElementById('root'));
  root.render(<PageComponent />);
}

Object.assign(window, {
  HERO_CAKE_IMAGE, LINE_QR_IMAGE,
  LOGO, BRAND, CONTACT, PHILOSOPHY, ORDER_FLOW, PRODUCT_CATEGORIES, PAGES,
  useViewport, MOBILE_BP,
  IconLine, IconIG, IconFB, IconPin, IconPhone, IconClock, IconLeaf, IconArrow,
  Placeholder, ProductImage, Nav, Footer, mountPage,
  DETAIL_HREF, DETAIL_STORE_KEY, goToDetail, readSelectedProduct,
});
