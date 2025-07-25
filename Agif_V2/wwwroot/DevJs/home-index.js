/**
 * Home Index Page JavaScript
 * Handles image slider functionality and navigation
 */

document.addEventListener('DOMContentLoaded', function () {
    // Initialize slider functionality
    initializeImageSlider();
});

/**
 * Initialize the image slider with auto-play and dot navigation
 */
function initializeImageSlider() {
    const slides = document.querySelector('.slides');
    const dots = document.querySelectorAll('.dot');

    // Check if slider elements exist
    if (!slides || !dots.length) {
        console.warn('Slider elements not found');
        return;
    }

    let index = 0;
    const totalSlides = dots.length;
    const slideInterval = 3000; // 3 seconds

    /**
     * Show slide at specified index
     * @param {number} i - Index of slide to show
     */
    function showSlide(i) {
        index = (i + totalSlides) % totalSlides; // Ensure circular navigation
        slides.style.transform = `translateX(${-index * 100}%)`;

        // Update active dot
        updateActiveDot(index);
    }

    /**
     * Update active dot indicator
     * @param {number} activeIndex - Index of active dot
     */
    function updateActiveDot(activeIndex) {
        dots.forEach((dot, i) => {
            if (i === activeIndex) {
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });
    }

    /**
     * Start auto-play functionality
     */
    function startAutoPlay() {
        return setInterval(() => {
            showSlide(index + 1);
        }, slideInterval);
    }

    /**
     * Stop auto-play functionality
     * @param {number} intervalId - Interval ID to clear
     */
    function stopAutoPlay(intervalId) {
        clearInterval(intervalId);
    }

    // Initialize first slide
    showSlide(0);

    // Start auto-play
    let autoPlayInterval = startAutoPlay();

    // Add click event listeners to dots
    dots.forEach((dot, i) => {
        dot.addEventListener('click', function () {
            // Stop current auto-play
            stopAutoPlay(autoPlayInterval);

            // Show clicked slide
            showSlide(i);

            // Restart auto-play
            autoPlayInterval = startAutoPlay();
        });
    });

    // Optional: Pause auto-play on hover
    const sliderContainer = document.querySelector('.slider');
    if (sliderContainer) {
        sliderContainer.addEventListener('mouseenter', function () {
            stopAutoPlay(autoPlayInterval);
        });

        sliderContainer.addEventListener('mouseleave', function () {
            autoPlayInterval = startAutoPlay();
        });
    }
}