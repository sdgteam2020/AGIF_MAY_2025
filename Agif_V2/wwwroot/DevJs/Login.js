// Login page functionality
class LoginManager {
    constructor(options = {}) {
        this.isLockedOut = options.isLockedOut || false;
        // FIXED: Better date parsing and validation
        this.lockoutEnd = options.lockoutEnd && options.lockoutEnd !== 'null' ?
            new Date(options.lockoutEnd) : null;
        this.autoRefreshTimeout = options.autoRefreshTimeout || null;
        this.init();
    }

    init() {
        $(document).ready(() => {
            this.setupAutoFill();
            this.setupCountdownTimer();
            this.setupFormSubmission();
            this.setupAutoRefresh();
        });
    }

    setupAutoFill() {
        // Auto-fill for development (remove in production)
        if (!this.isLockedOut) {
            $("#UserName").val("admin");
            $("#Password").val("Admin123!");
        }
    }

    setupCountdownTimer() {
        // FIXED: Added validation and debug logging
        if (this.isLockedOut && this.lockoutEnd && !isNaN(this.lockoutEnd.getTime())) {
            console.log('Setting up countdown timer for:', this.lockoutEnd);
            this.updateCountdown();
            this.countdownInterval = setInterval(() => this.updateCountdown(), 1000);
        } else {
            console.log('Countdown not initialized:', {
                isLockedOut: this.isLockedOut,
                lockoutEnd: this.lockoutEnd,
                isValidDate: this.lockoutEnd ? !isNaN(this.lockoutEnd.getTime()) : false
            });
        }
    }

    updateCountdown() {
        if (!this.lockoutEnd || isNaN(this.lockoutEnd.getTime())) {
            console.log('Invalid lockout end date');
            return;
        }

        const now = new Date();
        const timeLeft = this.lockoutEnd - now;
        

        if (timeLeft > 0) {
            const hours = Math.floor(timeLeft / (1000 * 60 * 60));
            const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);

            let display = '';
            if (hours > 0) display += hours + 'h ';
            if (minutes > 0) display += minutes + 'm ';
            display += seconds + 's';

            // FIXED: Added check if countdown element exists
            const countdownElement = $('#countdown');
            if (countdownElement.length > 0) {
                countdownElement.text(display);
            }
        } else {
            this.handleLockoutExpired();
        }
    }

    handleLockoutExpired() {
        
        if (this.countdownInterval) {
            clearInterval(this.countdownInterval);
        }

        $('#countdown').text('Expired - Please refresh page');
        $('#loginBtn').prop('disabled', false).html('<i class="fas fa-sign-in-alt me-2"></i>Login');
        $('.alert-danger').hide();
        $('input').prop('disabled', false);

        // Update lockout state
        this.isLockedOut = false;
    }

    //setupFormSubmission() {
    //    $('#loginForm').submit((e) => {
    //        const btn = $('#loginBtn');
    //        if (!btn.prop('disabled')) {
    //            btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Signing in...');
    //        }
    //    });
    //}
    setupFormSubmission() {
        $('#loginForm').submit((e) => {
            e.preventDefault();

            const btn = $('#loginBtn');
            const userNameInput = $("#UserName");
            const passwordInput = $("#Password");
            const configEl = document.getElementById("loginConfig");
            const publicKey = configEl ? configEl.dataset.publicKey : null;

            // Validate public key exists and is in PEM format
            if (!publicKey || !publicKey.includes('-----BEGIN PUBLIC KEY-----')) {
                console.error("Invalid or missing encryption key");
                alert("Security configuration error. Please refresh the page.");
                return;
            }

            if (!btn.prop('disabled')) {
                btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Signing in...');

                try {
                    // Initialize JSEncrypt
                    const encrypt = new JSEncrypt();
                    encrypt.setPublicKey(publicKey);

                    // Get plain text values
                    const username = userNameInput.val();
                    const password = passwordInput.val();

                    console.log('Attempting to encrypt credentials...');

                    // Encrypt values
                    const encryptedUser = encrypt.encrypt(username);
                    const encryptedPass = encrypt.encrypt(password);

                    console.log('Encryption result:', {
                        userEncrypted: !!encryptedUser,
                        passEncrypted: !!encryptedPass
                    });

                    // Verify encryption succeeded
                    if (encryptedUser && encryptedPass) {
                        // Update form fields with encrypted values
                        userNameInput.val(encryptedUser);
                        passwordInput.val(encryptedPass);

                        console.log('Submitting encrypted form...');

                        // Submit the form
                        e.currentTarget.submit();
                    } else {
                        // Encryption failed
                        console.error('Encryption failed');
                        btn.prop('disabled', false).html('<i class="fas fa-sign-in-alt me-2"></i>Login');
                        alert("Encryption failed. Please try again or refresh the page.");
                    }
                } catch (err) {
                    console.error("Login encryption error:", err);
                    btn.prop('disabled', false).html('<i class="fas fa-sign-in-alt me-2"></i>Login');
                    alert("An error occurred. Please refresh the page and try again.");
                }
            }
        });
    }
    setupAutoRefresh() {
        if (this.autoRefreshTimeout) {
            setTimeout(() => {
                location.reload();
            }, this.autoRefreshTimeout);
        }
    }
}

$(document).ready(() => {
    const configEl = document.getElementById("loginConfig");
    if (configEl) {
        const loginConfig = {
            isLockedOut: configEl.dataset.isLockedOut === "true",
            lockoutEnd: configEl.dataset.lockoutEnd !== "null" ? new Date(configEl.dataset.lockoutEnd) : null,
            autoRefreshTimeout: configEl.dataset.autoRefreshTimeout !== "null" ? parseInt(configEl.dataset.autoRefreshTimeout) : null
        };

        new LoginManager(loginConfig);
    } else {
        console.warn("loginConfig element not found.");
    }
});
$("input, textarea").on("paste", function (e) {
    e.preventDefault();
});              