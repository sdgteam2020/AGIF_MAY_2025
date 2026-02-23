document.addEventListener('DOMContentLoaded', function () {
    initializeImageSlider();
});

function initializeImageSlider() {
    const slides = document.querySelector('.slides');
    const dots = document.querySelectorAll('.dot');

    if (!slides || !dots.length) {
        console.warn('Slider elements not found');
        return;
    }

    let index = 0;
    const totalSlides = dots.length;
    const slideInterval = 3000; // 3 seconds

    
    function showSlide(i) {
        index = (i + totalSlides) % totalSlides; // Ensure circular navigation
        slides.style.transform = `translateX(${-index * 100}%)`;

        updateActiveDot(index);
    }

    function updateActiveDot(activeIndex) {
        dots.forEach((dot, i) => {
            if (i === activeIndex) {
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });
    }
    
    function startAutoPlay() {
        return setInterval(() => {
            showSlide(index + 1);
        }, slideInterval);
    }
    
    function stopAutoPlay(intervalId) {
        clearInterval(intervalId);
    }

    showSlide(0);

    let autoPlayInterval = startAutoPlay();

    dots.forEach((dot, i) => {
        dot.addEventListener('click', function () {
            stopAutoPlay(autoPlayInterval);

            showSlide(i);

            autoPlayInterval = startAutoPlay();
        });
    });

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
