// Chat state flags
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

// Get all focusable and visible elements within a root element (used for keyboard trap)
function focusableElements(root) {
    return Array.from(root.querySelectorAll(
        'button, [href], input, textarea, select, [tabindex]:not([tabindex="-1"])'
    )).filter(el => !el.disabled && el.offsetParent !== null);
}

// Open the chat dialog, show welcome message and start heartbeat pings
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
        // Periodically send a lightweight 'hi' to keep the session alive
        heartbeatTimer = setInterval(() => {
            sendQuery('hi', true);
        }, 240000);
    }

    const f = focusableElements(dlg);
    if (f.length) f[0].focus();

    requestAnimationFrame(() => {
        chatBox.scrollTop = chatBox.scrollHeight;
    });

    // Attach keyboard handlers for Escape and focus trapping
    document.addEventListener('keydown', onEsc, { capture: true });
    dlg.addEventListener('keydown', trapFocus);
}

// Close the chat dialog and clean up timers/handlers
function closeChat() {
    if (!isOpen) return;
    dlg.hidden = true;
    toggleBtn.setAttribute('aria-expanded', 'false');
    isOpen = false;

    if (heartbeatTimer) {
        // Stop periodic heartbeat when chat is closed
        clearInterval(heartbeatTimer);
        heartbeatTimer = null;
    }

    toggleBtn.focus();

    // Remove keyboard handlers
    document.removeEventListener('keydown', onEsc, { capture: true });
    dlg.removeEventListener('keydown', trapFocus);
}

// Close chat when Escape key is pressed
function onEsc(e) {
    if (e.key === 'Escape') {
        closeChat();
        e.preventDefault();
    }
}

// Keep focus trapped inside the chat dialog for accessibility
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

// Update hit counters on page load
$(document).ready(function () {
    $.ajax({
        url: '/Home/UpdateHitCounter', // Controller Action
        type: 'GET',
        success: function (response) {
            $('#today').text('Today: ' + response.todayCount);
            $('#monthly').text('Monthly: ' + response.monthlyCount);
            $('#total').text('Total: ' + (response.totalCount + 171222));
        },
        error: function () {
            console.log('Error loading hit counter');
        }
    });

});

// Navigation helpers for header buttons
$("#ViewLog").on('click', function () {
    window.location.href = '/Home/LogViewer'
})
$("#AnalyticsDashBoard").on('click', function () {
    window.location.href = '/Home/AnalyticsDashBoard'
})

// Return anti-forgery token to include in AJAX requests
function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}
