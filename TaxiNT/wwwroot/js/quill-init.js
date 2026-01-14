import { initializeApp } from "https://www.gstatic.com/firebasejs/9.23.0/firebase-app.js";
import { getMessaging, getToken, isSupported } from "https://www.gstatic.com/firebasejs/9.23.0/firebase-messaging.js";

export async function getFcmToken(firebaseConfig, vapidKey) {
    const supported = await isSupported();
    if (!supported) return null;

    // Đăng ký service worker đúng file firebase-messaging-sw.js
    const reg = await navigator.serviceWorker.register("/firebase-messaging-sw.js");

    const permission = await Notification.requestPermission();
    if (permission !== "granted") return null;

    const app = initializeApp(firebaseConfig);
    const messaging = getMessaging(app);

    const token = await getToken(messaging, {
        vapidKey,
        serviceWorkerRegistration: reg
    });

    return token || null;
}
