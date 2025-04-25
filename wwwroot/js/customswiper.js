//SwiperJS: effect Ds ảnh sử dụng Function cho môi trường hosting 
window.callSwiperJSEffect = function () {
    var swiper5 = new Swiper(".swiperJsEffect", {
        slidesPerView: "auto",
        loop: true,
        centeredSliders: true,
        speed: 2000,
        allowTouchMove: false,
        disableOnInteraction: false,
        autoplay: {
            delay: 3000,
        },
        grabCursor: false,
        effect: "creative",
        creativeEffect: {
            prev: {
                translate: ["120%", 0, -500],
            },
            next: {
                translate: ["-120%", 0, -500],
            },
        },
    });
};

window.onload = function () {
    window.callSwiperJSEffect();
};