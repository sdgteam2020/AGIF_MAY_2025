const getSecretKey = () => $('#spnhdns').text().trim() || "";

class LoginManager {
    constructor(options = {}) {
        this.isLockedOut = options.isLockedOut || false;
        this.lockoutEnd = options.lockoutEnd && options.lockoutEnd !== 'null' ?
            new Date(options.lockoutEnd) : null;
        this.autoRefreshTimeout = options.autoRefreshTimeout || null;
        this.countdownInterval = null;
        this.init();
    }

    init() {
        this.setupAutoFill();
        this.setupCountdownTimer();
        this.setupFormSubmission();
        this.setupAutoRefresh();
    }

    setupAutoFill() {
        if (!this.isLockedOut) {
            $("#UserName").val("");
            $("#Password").val("");
        }
    }

    setupCountdownTimer() {
        if (this.isLockedOut && this.lockoutEnd && !isNaN(this.lockoutEnd.getTime())) {
            this.updateCountdown();
            this.countdownInterval = setInterval(() => this.updateCountdown(), 1000);
        }
    }

    updateCountdown() {
        const now = new Date();
        const timeLeft = this.lockoutEnd - now;

        if (timeLeft > 0) {
            const hours = Math.floor(timeLeft / (1000 * 60 * 60));
            const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);

            let display = `${hours > 0 ? hours + 'h ' : ''}${minutes > 0 ? minutes + 'm ' : ''}${seconds}s`;
            $('#countdown').text(display);
        } else {
            this.handleLockoutExpired();
        }
    }

    handleLockoutExpired() {
        if (this.countdownInterval) clearInterval(this.countdownInterval);
        $('#countdown').text('Expired - Please refresh');
        $('#loginBtn').prop('disabled', false).html('<i class="fas fa-sign-in-alt me-2"></i>Login');
        $('.alert-danger').hide();
        $('input').prop('disabled', false);
        this.isLockedOut = false;
    }

    setupFormSubmission() {
        $('#loginForm').on('submit', (e) => {
            e.preventDefault();

            const btn = $('#loginBtn');
            const userNameInput = $("#UserName");
            const passwordInput = $("#Password");

            if (btn.prop('disabled')) return;

            btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Signing in...');

            try {
                const username = userNameInput.val();
                const password = passwordInput.val();

                const encryptedUsername = encryptData(username);
                const encryptedPassword = encryptData(password);

                if (encryptedUsername && encryptedPassword) {
                    userNameInput.val(encryptedUsername);
                    passwordInput.val(encryptedPassword);

                    e.target.submit();
                } else {
                    throw new Error("Encryption returned empty result");
                }
            } catch (err) {
                console.error("Login encryption error:", err);
                btn.prop('disabled', false).html('<i class="fas fa-sign-in-alt me-2"></i>Login');
                alert("Security error. Please refresh and try again.");
            }
        });
    }

    setupAutoRefresh() {
        if (this.autoRefreshTimeout) {
            setTimeout(() => location.reload(), this.autoRefreshTimeout);
        }
    }
}

//function encryptData(plainText) {
//    const secretKey = getSecretKey(); // Call the helper
//    if (!secretKey) {
//        console.error("Encryption Key Missing");
//        return "";
//    }

//    const key = CryptoJS.enc.Utf8.parse(secretKey);
//    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16));

//    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
//        iv: iv,
//        mode: CryptoJS.mode.CBC,
//        padding: CryptoJS.pad.Pkcs7
//    });

//    return encrypted.toString();
//}
function encryptData(plainText) {
    const secretKey = getSecretKey(); // Ensure this matches what C# expects
    if (!secretKey) {
        console.error("Encryption Key Missing");
        return "";
    }
    const key = CryptoJS.enc.Base64.parse(secretKey);
    const iv = CryptoJS.lib.WordArray.random(16);

    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    const combinedData = iv.clone().concat(encrypted.ciphertext);

    return CryptoJS.enc.Base64.stringify(combinedData);
}

$(document).ready(() => {
    const configEl = document.getElementById("loginConfig");
    if (configEl) {
        const loginConfig = {
            isLockedOut: configEl.dataset.isLockedOut === "true",
            lockoutEnd: configEl.dataset.lockoutEnd !== "null" ? configEl.dataset.lockoutEnd : null,
            autoRefreshTimeout: configEl.dataset.autoRefreshTimeout !== "null" ? parseInt(configEl.dataset.autoRefreshTimeout) : null
        };
        new LoginManager(loginConfig);
    }

    $("input, textarea").on("paste", (e) => e.preventDefault());
});