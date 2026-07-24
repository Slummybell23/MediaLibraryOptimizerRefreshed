async function handle(res) {
  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `API returned ${res.status}`)
  }
  return res.status === 204 ? null : res.json()
}

export function getJson(url) {
  return fetch(url).then(handle)
}

export function postJson(url, body) {
  return fetch(url, {
    method: 'POST',
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  }).then(handle)
}
