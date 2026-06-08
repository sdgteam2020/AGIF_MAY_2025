let isOpen = false;
let isSending = false;
let welcomeShown = false;
let lastUserQuery = "";
let heartbeatTimer = null;

const dlg = document.getElementById('asdcChat');
const toggleBtn = document.getElementById('asdcChatToggle');
const closeBtn = document.getElementById('asdcClose');
const chatBox = document.getElementById('asdcChatBox');
const inputEl = document.getElementById('asdcQuery');
const sendBtn = document.getElementById('asdcSend');
const loadingEl = document.getElementById('asdcLoading');

function focusableElements(root) {
    return Array.from(root.querySelectorAll(
        'button, [href], input, textarea, select, [tabindex]:not([tabindex="-1"])'
    )).filter(el => !el.disabled && el.offsetParent !== null);
}

function openChat() {
    if (isOpen) return;
    dlg.hidden = false;
    toggleBtn.setAttribute('aria-expanded', 'true');
    isOpen = true;

    if (!welcomeShown) {
        showWelcome();
        welcomeShown = true;
    }

    if (!heartbeatTimer) {
        heartbeatTimer = setInterval(() => {
            sendQuery('hi', true);
        }, 240000);
    }

    const f = focusableElements(dlg);
    if (f.length) f[0].focus();

    requestAnimationFrame(() => {
        chatBox.scrollTop = chatBox.scrollHeight;
    });

    document.addEventListener('keydown', onEsc, { capture: true });
    dlg.addEventListener('keydown', trapFocus);
}

function closeChat() {
    if (!isOpen) return;
    dlg.hidden = true;
    toggleBtn.setAttribute('aria-expanded', 'false');
    isOpen = false;

    if (heartbeatTimer) {
        clearInterval(heartbeatTimer);
        heartbeatTimer = null;
    }

    toggleBtn.focus();

    document.removeEventListener('keydown', onEsc, { capture: true });
    dlg.removeEventListener('keydown', trapFocus);
}

function onEsc(e) {
    if (e.key === 'Escape') {
        closeChat();
        e.preventDefault();
    }
}

function trapFocus(e) {
    if (e.key !== 'Tab') return;
    const f = focusableElements(dlg);
    if (!f.length) return;
    const first = f[0];
    const last = f[f.length - 1];
    if (e.shiftKey && document.activeElement === first) {
        last.focus();
        e.preventDefault();
    } else if (!e.shiftKey && document.activeElement === last) {
        first.focus();
        e.preventDefault();
    }
}

$(document).ready(function () {
    $.ajax({
        url: '/Home/UpdateHitCounter', // Controller Action
        type: 'GET',
        success: function (response) {
            console.log(response);
            $('#today').text('Today: ' + response.todayCount);
            $('#monthly').text('Monthly: ' + response.monthlyCount);
            $('#total').text('Total: ' + (response.totalCount + 171222));
        },
        error: function () {
            console.log('Error loading hit counter');
        }
    });

});


$("#ViewLog").on('click', function () {
    window.location.href = '/Home/LogViewer'
})
$("#AnalyticsDashBoard").on('click', function () {
    window.location.href = '/Home/AnalyticsDashBoard'
})
function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}