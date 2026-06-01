import http from 'k6/http';
import { sleep, check } from 'k6';

// Don't use http requests. Only https.
const baseUrl = __ENV.services__web__https__0;
console.log('Base URL: ' + baseUrl);

export const options = {
    stages: [
        { duration: '30s', target: 150 },  // Ramp-up
        { duration: '2m', target: 150 },   // Steady
        { duration: '30s', target: 0 },   // Ramp-down
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: [
            'p(50)<10',
            'p(95)<20',
        ],
    },
  };

export function setup() {
    
    const tokenPayload = http.get(`${baseUrl}/reg`);
    const accessToken = tokenPayload.json('accessToken');
    
    console.log(tokenPayload);
    console.log('Access Token is ' + accessToken !== undefined);
    
    let shortcuts = [];
    
    for (let i = 0; i < 9; i++) {

        const createRes = http.post(
            `${baseUrl}/shortcuts`, 
            JSON.stringify({ 
                longUrl: 'https://google.com/' 
            }),
            {
                headers: { 
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + accessToken
                },
            }
        );

        if (createRes.status !== 200) {
            throw new Error(`Failed to create shortcut. Status is ${createRes.status}`);
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

    sleep(0.3 + Math.random() * 0.7);
}
