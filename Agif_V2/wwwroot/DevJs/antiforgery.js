/**
 * Antiforgery Token Management
 * Automatically adds CSRF token to AJAX requests and form submissions
 */

(function () {
    'use strict';

    // Get CSRF token from meta tag
    function getAntiforgeryToken() {
        const token = document.querySelector('meta[name="csrf-token"]');
        return token ? token.getAttribute('content') : null;
    }

    // Add token to AJAX request headers
    function addTokenToAjax() {
        const token = getAntiforgeryToken();
        if (!token) {
            console.warn('Antiforgery token not found in meta tag');
            return;
        }

        // jQuery AJAX setup
        if (typeof jQuery !== 'undefined') {
            $(document).ajaxSend(function (event, jqxhr, settings) {
                if (!settings.headers) {
                    settings.headers = {};
                }
                settings.headers['X-CSRF-TOKEN'] = token;
            });
        }

        // Fetch API interceptor
        if (typeof fetch !== 'undefined') {
            const originalFetch = window.fetch;
            window.fetch = function (...args) {
                const [resource, options = {}] = args;
                const method = (options.method || 'GET').toUpperCase();

                // Add token to POST, PUT, PATCH, DELETE requests
                if (['POST', 'PUT', 'PATCH', 'DELETE'].includes(method)) {
                    if (!options.headers) {
                        options.headers = {};
                    }
                    options.headers['X-CSRF-TOKEN'] = token;
                }

                return originalFetch.apply(this, args);
            };
        }
    }

    // Add token to form submissions
    function addTokenToForms() {
        const token = getAntiforgeryToken();
        if (!token) {
            console.warn('Antiforgery token not found in meta tag');
            return;
        }

        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            // Skip forms with method="GET"
            const method = (form.method || 'POST').toUpperCase();
            if (method === 'GET') {
                return;
            }

            // Check if token field already exists
            let tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
            if (!tokenInput) {
                tokenInput = document.createElement('input');
                tokenInput.type = 'hidden';
                tokenInput.name = '__RequestVerificationToken';
                tokenInput.value = token;
                form.appendChild(tokenInput);
            }
        });
    }

    // Handle AJAX error responses for antiforgery failures
    function setupErrorHandling() {
        if (typeof jQuery !== 'undefined') {
            $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
                if (jqxhr.status === 400 && jqxhr.responseText === 'Invalid antiforgery token.') {
                    console.error('Antiforgery token validation failed');
                    // Optionally redirect to login or show error
                    if (typeof showErrorMessage === 'function') {
                        showErrorMessage('Session expired. Please refresh the page.');
                    }
                }
            });
        }
    }

    // Public API
    window.AntiforgeryToken = {
        getToken: getAntiforgeryToken,
        init: function () {
            addTokenToAjax();
            addTokenToForms();
            setupErrorHandling();
        }
    };

    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', window.AntiforgeryToken.init);
    } else {
        window.AntiforgeryToken.init();
    }
})();