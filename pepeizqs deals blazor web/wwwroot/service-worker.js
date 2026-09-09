// v1.0.14

const CACHE_NAME = 'pepesdeals-v1';
const STATIC_CACHE = [
    '/',
    '/manifest.json',
    '/favicons/favicon-192x192.png',
    '/favicons/favicon-512x512.png'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(STATIC_CACHE);
            })
            .then(() => {
                return self.skipWaiting();
            })
            .catch(err => {
                console.error('Error en install:', err);
            })
    );
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames
                    .filter(name => name !== CACHE_NAME)
                    .map(name => caches.delete(name))
            );
        }).then(() => self.clients.claim())
    );
});

self.addEventListener('fetch', event => {
    const { request } = event;
    const url = new URL(request.url);

    const esMiDominio = url.hostname.includes('pepe.deals');

    if (!esMiDominio) {
        event.respondWith(fetch(request));
        return;
    }

    if (request.destination === 'image') {
        event.respondWith(
            caches.match(request).then(response => {
                return response || fetch(request).then(fetchResponse => {
                    if (fetchResponse.status === 200) {
                        return caches.open(CACHE_NAME).then(cache => {
                            cache.put(request, fetchResponse.clone());
                            return fetchResponse;
                        });
                    }
                    return fetchResponse;
                }).catch(() => {
                    return caches.match(request);
                });
            })
        );
        return;
    }

    event.respondWith(
        fetch(request)
            .then(response => {
                if (response.status === 200 && request.method === 'GET') {
                    const responseClone = response.clone();
                    caches.open(CACHE_NAME).then(cache => {
                        cache.put(request, responseClone);
                    });
                }
                return response;
            })
            .catch(() => {
                if (request.method === 'GET') {
                    return caches.match(request);
                }
                throw new Error('Network request failed');
            })
    );
});

self.addEventListener('push', function (event) {
    const data = event.data ? JSON.parse(event.data.text()) : {};

    const icono = data.icon ? `https://pepe.deals${data.icon}` : 'https://pepe.deals/favicon.ico';

    const options = {
        body: " ",
        icon: icono,
        tag: 'notification', 
        requireInteraction: false,
        data: {
            url: data.url || '/' 
        }
    };

    event.waitUntil(
        self.registration.showNotification(data.title || 'pepe deals', options)
    );
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    const urlToOpen = event.notification.data.url || '/';

    event.waitUntil(
        clients.matchAll({ type: 'window' }).then(function (clientList) {
            for (let i = 0; i < clientList.length; i++) {
                const client = clientList[i];
                if (client.url === urlToOpen && 'focus' in client) {
                    return client.focus();
                }
            }

            return clients.openWindow(urlToOpen);
        })
    );
});