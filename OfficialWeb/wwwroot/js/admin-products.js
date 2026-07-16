// admin-products.js — 後台產品 Main：新增 / 刪除 Modal + AJAX
// 防偽 token 取自 _AdminLayout 的 #af-token 隱藏表單，放進 RequestVerificationToken header。
(function () {
    'use strict';

    function token() {
        var input = document.querySelector('#af-token input[name="__RequestVerificationToken"]');
        return input ? input.value : '';
    }

    function post(url, body) {
        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token(),
            },
            body: body ? new URLSearchParams(body).toString() : '',
        });
    }

    // ── 新增 ──
    var createSubmit = document.getElementById('createSubmit');
    if (createSubmit) {
        createSubmit.addEventListener('click', function () {
            var errorBox = document.getElementById('createError');
            errorBox.classList.add('d-none');

            post('/Admin/CreateProduct', {
                Name: document.getElementById('createName').value.trim(),
                Category: document.getElementById('createCategory').value.trim(),
                Tag: document.getElementById('createTag').value.trim(),
                CanDeliver: document.getElementById('createDeliver').checked,
                CanPickup: document.getElementById('createPickup').checked,
            }).then(function (res) {
                return res.json().then(function (data) { return { ok: res.ok, data: data }; });
            }).then(function (r) {
                if (r.ok && r.data.ok) {
                    location.reload();
                } else {
                    errorBox.textContent = (r.data && r.data.message) || '新增失敗';
                    errorBox.classList.remove('d-none');
                }
            }).catch(function () {
                errorBox.textContent = '連線失敗，請重試';
                errorBox.classList.remove('d-none');
            });
        });
    }

    // ── 刪除 ──
    var deleteId = null;
    var deleteModalEl = document.getElementById('deleteModal');

    document.querySelectorAll('.js-delete').forEach(function (btn) {
        btn.addEventListener('click', function () {
            deleteId = btn.getAttribute('data-id');
            document.getElementById('deleteName').textContent = btn.getAttribute('data-name');
            bootstrap.Modal.getOrCreateInstance(deleteModalEl).show();
        });
    });

    var deleteConfirm = document.getElementById('deleteConfirm');
    if (deleteConfirm) {
        deleteConfirm.addEventListener('click', function () {
            if (!deleteId) return;
            post('/Admin/DeleteProduct/' + deleteId).then(function (res) {
                if (res.ok) location.reload();
                else alert('刪除失敗');
            }).catch(function () { alert('連線失敗，請重試'); });
        });
    }
})();
