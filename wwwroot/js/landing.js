// wwwroot/js/landing.js
window.nellaLanding = {
  init: function () {
    document.documentElement.classList.add('page-loaded');
    const input = document.querySelector('.landing-root .input');
    if (input) { try { input.focus({ preventScroll: true }); } catch {} }
  }
};
