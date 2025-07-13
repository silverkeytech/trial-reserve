importScripts('https://www.gstatic.com/firebasejs/10.1.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.1.0/firebase-messaging-compat.js');

// Your Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyCYp9ZVNexzuelfKJJbqXtgXykllY0T0T0",
    authDomain: "reserve-a452c.firebaseapp.com",
    projectId: "reserve-a452c",
    storageBucket: "reserve-a452c.appspot.com",
    messagingSenderId: "700503073311",
    appId: "1:700503073311:web:73df40335ffc75f7eeab2a"
};

// Initialize Firebase
firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

self.addEventListener('push', function (event) {
    console.log('Push event received:', event);
    const data = event.data.json();
    console.log('Push data:', data);

    const notificationTitle = data.notification?.title || 'No Title';
    const notificationOptions = {
        body: data.notification?.body || 'No Body',
        data: {
            url: data.data?.url || '/'
        }
    };

    event.waitUntil(
        self.registration.showNotification(notificationTitle, notificationOptions)
    );
});

self.addEventListener('notificationclick', function (event) {
    console.log('Notification click event:', event);
    event.notification.close();
    event.waitUntil(
        clients.openWindow(event.notification.data.url)
    );
});
