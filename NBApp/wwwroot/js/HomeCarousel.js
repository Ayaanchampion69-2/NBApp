// Directions link — reads the address from the data-address attribute
// on the link itself, so the markup stays the single source of truth.
const directionsLink = document.getElementById('directions-link');
if (directionsLink) {
    const shopAddress = directionsLink.dataset.address;
    directionsLink.href = `https://www.google.com/maps/dir/?api=1&destination=${encodeURIComponent(shopAddress)}`;
}

// Carousel
let current = 0;
const total = 5;
const track = document.getElementById('carousel-track');
const dotsWrap = document.getElementById('carousel-dots');
let timer;

const dotClass = (active) => active
    ? 'w-5 h-1.5 rounded-full bg-indigo-700 transition-all'
    : 'w-1.5 h-1.5 rounded-full bg-stone-300 transition-all';

for (let i = 0; i < total; i++) {
    const d = document.createElement('button');
    d.setAttribute('aria-label', 'Go to slide ' + (i + 1));
    d.className = dotClass(i === 0);
    d.onclick = () => goTo(i);
    dotsWrap.appendChild(d);
}

function render() {
    track.style.transform = `translateX(-${current * 100}%)`;
    [...dotsWrap.children].forEach((d, i) => {
        d.className = dotClass(i === current);
    });
}

function moveCarousel(dir) {
    current = (current + dir + total) % total;
    render();
    resetTimer();
}

function goTo(i) {
    current = i;
    render();
    resetTimer();
}

function resetTimer() {
    clearInterval(timer);
    timer = setInterval(() => moveCarousel(1), 4500);
}

render();
resetTimer();