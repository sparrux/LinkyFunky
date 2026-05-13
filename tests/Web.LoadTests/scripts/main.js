import http from 'k6/http';
import { sleep, check } from 'k6';

const baseUrl = __ENV.services__web__http__0;
console.log('Base URL: ' + baseUrl);

export const options = {
    stages: [
        { duration: '30s', target: 50 },  // Ramp-up
        { duration: '2m', target: 50 },   // Steady
        { duration: '30s', target: 0 },   // Ramp-down
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500']
    },
  };

export function setup() {
    let shortcuts = [];
    
    for (let i = 0; i < 9; i++) {

        const createRes = http.post(
            `${baseUrl}/shortcuts`, 
            JSON.stringify({ 
                longUrl: 'https://google.com/' 
            }),
            {
              headers: { 'Content-Type': 'application/json' }
            }
        );

        if (createRes.status !== 200) {
            throw new Error('Failed to create shortcut');
        }

        const payload = createRes.json();

        console.log('Setup shortcut response: ' + payload.shortCode);
        shortcuts.push(payload.shortCode);
    }

    return { shortcuts };
}

export default function(data) {
    const randomShortcut = data.shortcuts[Math.floor(Math.random() * data.shortcuts.length)];

    const redirectRes = http.get(`${baseUrl}/r/${randomShortcut}`, { redirects: 0 });

    check(redirectRes, {
        'status is 302': (res) => res.status === 302,
    });

    sleep(0.5);
}
