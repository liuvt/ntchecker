importScripts('https://www.gstatic.com/firebasejs/9.23.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.23.0/firebase-messaging-compat.js');

firebase.initializeApp({
    apiKey: "AIzaSyBUPBcneMJmSguSsjH3otgKK5X-6i598o4",
    authDomain: "checker-bill.firebaseapp.com",
    projectId: "checker-bill",
    storageBucket: "checker-bill.firebasestorage.app",
    messagingSenderId: "188088233825",
    appId: "1:188088233825:web:69e1db71846b7b7d941f20",
    measurementId: "G-LLFXJHDJ1J"
});

const messaging = firebase.messaging();

// Nhận push khi app ở background
messaging.onBackgroundMessage((payload) => {
    const title = payload?.notification?.title || "Đã có phiếu Checker";

    const options = {
        body: payload?.notification?.body || "Phiếu checker đã được cập nhật vào xem ngay!",
        icon: "/imgs/customers/logo-315x315.png",
        data: payload?.fcmOptions?.link || "/"
    };

    self.registration.showNotification(title, options);
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();
    const url = event.notification?.data || "/";
    event.waitUntil(clients.openWindow(url));
});
