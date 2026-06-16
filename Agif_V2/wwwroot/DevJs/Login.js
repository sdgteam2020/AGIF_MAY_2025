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
        this.setupPasswordHandling();
    }

    setupPasswordHandling() {
        const passwordInput = $("#Password");
        const errorSpan = $("[data-valmsg-for='Password']");

        passwordInput.on("input", function () {
            let value = $(this).val();
            let hasSpace = /\s/.test(value);

            // 🚫 Remove spaces AFTER detecting
            let cleaned = value.replace(/\s+/g, '');

            // ⚠️ Handle length
            let isTooLong = cleaned.length > 60;

            if (isTooLong) {
                cleaned = cleaned.substring(0, 60);
            }

            // 🧠 Show messages (priority)
            //if (hasSpace && isTooLong) {
            //    errorSpan.text("No spaces allowed and max 60 characters.");
            //} else if (hasSpace) {
            //    errorSpan.text("Spaces are not allowed.");
            //} else if (isTooLong) {
            //    errorSpan.text("Password cannot exceed 60 characters.");
            //} else {
            //    errorSpan.text("");
            //}

            // ✅ Update value
            $(this).val(cleaned);
        });
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
            console.log(1);

            if (btn.prop('disabled')) return;

            btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Signing in...');

            try {
                const username = userNameInput.val();
                const password = passwordInput.val();
                console.log(2);
                const encryptedUsername = encryptData(username);
                const encryptedPassword = encryptData(password);
                console.log(3);
                if (encryptedUsername && encryptedPassword) {
                    userNameInput.val(encryptedUsername);
                    passwordInput.val(encryptedPassword);
                    console.log(4);
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

function encryptData(plainText) {
    const secretKey = getSecretKey(); // Ensure this matches what C# expects
    if (!secretKey) {
        return "";
    }
    const key = CryptoJS.enc.Base64.parse(secretKey);
    const iv = CryptoJS.lib.WordArray.random(16);

    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });
    const cipherText = encrypted.ciphertext;
    const hmac = CryptoJS.HmacSHA256(iv.clone().concat(cipherText), key);

    const finalData = iv.clone().concat(cipherText).concat(hmac);

    return CryptoJS.enc.Base64.stringify(finalData);
}


$('.form-email').on("keypress", function (e) {
    const keyCode = e.which;

    if ((keyCode >= 65 && keyCode <= 90) ||  // A-Z
        (keyCode >= 97 && keyCode <= 122) ||  // a-z
        (keyCode >= 48 && keyCode <= 57) ||   // 0-9
        (keyCode == 64) ||                    // '@' symbol (keyCode 64)
        (keyCode == 46) ||                    // '.' symbol (keyCode 46)
        (keyCode == 95)) {                   // '_' symbol (keyCode 95)
        return true;
    } else {
        showErrorMessage('Only Alphabets, Numbers, @, . and _ are allowed');
        return false; // Block the keypress
    }
});
function showErrorMessage(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert" id="validationerrormessage">
            <i class="lni lni-cross-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    $('body').append(alertHtml);

    setTimeout(function () {
        $('.alert-danger').fadeOut(300, function () {
            $(this).remove();
        });
    }, 2000);
}


$('.form-email').on("keypress", function (e) {
    const keyCode = e.which;

    if ((keyCode >= 65 && keyCode <= 90) ||  // A-Z
        (keyCode >= 97 && keyCode <= 122) ||  // a-z
        (keyCode >= 48 && keyCode <= 57) ||   // 0-9
        (keyCode == 64) ||                    // '@' symbol (keyCode 64)
        (keyCode == 46) ||                    // '.' symbol (keyCode 46)
        (keyCode == 95)) {                   // '_' symbol (keyCode 95)
        return true;
    } else {
        showErrorMessage('Only Alphabets, Numbers, @, . and _ are allowed');
        return false; // Block the keypress
    }
});
function showErrorMessage(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert" id="validationerrormessage">
            <i class="lni lni-cross-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    $('body').append(alertHtml);

    setTimeout(function () {
        $('.alert-danger').fadeOut(300, function () {
            $(this).remove();
        });
    }, 2000);
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

var message = $("#tempMessage").val();

if (message) {
    Swal.fire({
        icon: 'warning',
        title: 'Error',
        text: message,
        confirmButtonText: 'OK'
    });
}