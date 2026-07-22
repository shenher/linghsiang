// home.js — 首頁進場動畫：掛載後延遲 60ms 加上 .in 觸發階梯式淡入
(function () {
    'use strict';
    setTimeout(function () {
        document.querySelectorAll('.hm-fade, .hm-mask').forEach(function (el) {
            el.classList.add('in');
        });
    }, 60);
})();
