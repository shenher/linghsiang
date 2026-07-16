// products.js — 產品頁分類 tab 前端過濾（不重新載頁）
// tab 的 data-category 為空字串代表「全部」；卡片依 data-category 顯示/隱藏。
(function () {
    'use strict';
    var tabs = document.querySelectorAll('.products-tab');
    var cards = document.querySelectorAll('.product-card');

    tabs.forEach(function (tab) {
        tab.addEventListener('click', function () {
            var category = tab.getAttribute('data-category');

            tabs.forEach(function (t) {
                var on = t === tab;
                t.classList.toggle('active', on);
                t.setAttribute('aria-selected', on ? 'true' : 'false');
            });

            cards.forEach(function (card) {
                var show = category === '' || card.getAttribute('data-category') === category;
                card.style.display = show ? '' : 'none';
            });
        });
    });
})();
