// admin-product-detail.js — 後台產品 Detail：動態列新增 / 刪除 + 索引重排
// MVC 集合模型繫結需要連續索引（Sizes[0]、Sizes[1]…），每次增刪後全部重編。
(function () {
    'use strict';

    // 依容器重編列索引：把 name="Xxx[n].Prop" 的 n 換成目前列序
    function reindex(container) {
        container.querySelectorAll('[data-row]').forEach(function (row, i) {
            row.querySelectorAll('[name]').forEach(function (input) {
                input.name = input.name.replace(/\[\d+|\[__i__/, '[' + i);
            });
        });
    }

    // 新增列：複製 template、填入索引
    document.querySelectorAll('.js-add-row').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var container = document.getElementById(btn.getAttribute('data-target'));
            var template = document.getElementById(btn.getAttribute('data-template'));
            container.appendChild(template.content.cloneNode(true));
            reindex(container);
        });
    });

    // 刪除列（事件委派：涵蓋動態新增的列）
    document.addEventListener('click', function (e) {
        var removeBtn = e.target.closest('.js-remove-row');
        if (!removeBtn) return;
        var row = removeBtn.closest('[data-row]');
        var container = row.parentElement;
        row.remove();
        reindex(container);
    });
})();
